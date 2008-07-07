#region Copyright And Revision History

/*---------------------------------------------------------------------------

	Copyright © 2006 Bill Menees.  All rights reserved.
	Bill@Menees.com

	$History: VisualStudioInvoker.cs $
	
	*****************  Version 1  *****************
	User: Bill         Date: 7/08/06    Time: 1:58p
	Created in $/CSharp/Menees/Classes

-----------------------------------------------------------------------------*/

#endregion

#region Using Directives

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;
using System.Globalization;

#endregion

namespace Ankh.Diff
{
	/// <summary>
	/// This class is used to open a file in Visual Studio at a specific line number.
	/// Using Utilities.ShellExecute, it's very easy to open a file that's associated
	/// with Visual Studio.  But to force a file open in Visual Studio and then
	/// jump to a specific line number is a lot more work.
	/// <para/>
	/// The bulk of the code for this class was shamelessly pulled from FxCop 1.35's
	/// Microsoft.FxCop.UI.FxCopUI class (in FxCop.exe) using Reflector and the
	/// FileDisassembler add-in.
	/// </summary>
	public static class VisualStudioInvoker
	{
		#region Public Methods

		/// <summary>
		/// Opens a file in Visual Studio.
		/// </summary>
		/// <param name="strFileName">The full path to a file to open.</param>
		/// <param name="strFileLineNumber">The 1-based line number to go to in the file.</param>
		/// <returns>True if it was successful.  False if the file couldn't be opened in Visual Studio.</returns>
		public static bool OpenFile(string strFileName, string strFileLineNumber)
		{
			try
			{
				//Use late-bound COM to open the file in Visual Studio
				//so we can jump to a specific line number.  This also
				//allows us to reuse an open instance of VS.
				//
				//We could execute Visual Studio by command-line and
				//run the GotoLn command (like MegaBuild does), but that
				//requires starting a new instance of VS for each file opened.
				object oDTE = LaunchInVisualStudio(strFileName, strFileLineNumber);
				return oDTE != null;
			}
			catch (COMException)
			{
				return false;
			}
		}

		#endregion

		#region Private Methods

		private static object LateCall(object obj, string strMethod, params object[] args)
		{
			return LateInvoke(obj, BindingFlags.InvokeMethod, strMethod, args);
		}

		private static object LateGet(object obj, string strProperty)
		{
			return LateInvoke(obj, BindingFlags.GetProperty, strProperty, new object[0]);
		}

		private static object LateInvoke(object obj, BindingFlags eFlags, string strMethod, params object[] args)
		{
			object obj1;
			try
			{
				obj1 = obj.GetType().InvokeMember(strMethod, eFlags, null, obj, args, CultureInfo.InvariantCulture);
			}
			catch (TargetInvocationException exception1)
			{
				throw exception1.InnerException;
			}
			return obj1;
		}

		private static object LateSet(object obj, string strProperty, object value)
		{
			return LateInvoke(obj, BindingFlags.SetProperty, strProperty, new object[] { value });
		}

		private static object LaunchInVisualStudio(string strFileAndDirectory, string strLine)
		{
			try
			{
				object obj1 = Marshal.GetActiveObject("VisualStudio.DTE");
				LaunchInVisualStudio(obj1, strFileAndDirectory, strLine);
				return obj1;
			}
			catch (COMException)
			{
				System.Type type1 = System.Type.GetTypeFromProgID("VisualStudio.DTE");
				if (type1 == null)
				{
					throw;
				}
				object obj2 = Activator.CreateInstance(type1);
				Marshal.GetIUnknownForObject(obj2);
				LaunchInVisualStudio(obj2, strFileAndDirectory, strLine);
				return obj2;
			}
		}

		private static void LaunchInVisualStudio(object dte, string strFileAndDirectory, string strLine)
		{
			LateCall(dte, "ExecuteCommand", new object[] { "File.OpenFile", "\"" + strFileAndDirectory + "\"" });
			LateCall(dte, "ExecuteCommand", new object[] { "Edit.Goto", strLine });
			BringVSForward(dte);
		}

		private static void BringVSForward(object dte)
		{
			object obj1 = LateGet(dte, "MainWindow");
			IntPtr ptr1 = (IntPtr)((int)LateGet(obj1, "HWnd"));
			if (NativeMethods.IsIconic(ptr1))
			{
				NativeMethods.ShowWindowAsync(ptr1, 9);
			}
			NativeMethods.SetForegroundWindow(ptr1);
			Thread.Sleep(1000);
			LateCall(obj1, "Activate", new object[0]);
			LateSet(obj1, "Visible", true);
		}

		#endregion

		#region Private Types

		private static class NativeMethods
		{
			[return: MarshalAs(UnmanagedType.Bool)]
			[DllImport("user32.dll")]
			public static extern bool IsIconic(IntPtr hWnd);

			[return: MarshalAs(UnmanagedType.Bool)]
			[DllImport("user32.dll")]
			public static extern bool SetForegroundWindow(IntPtr hWnd);

			[return: MarshalAs(UnmanagedType.Bool)]
			[DllImport("user32.dll")]
			public static extern bool ShowWindowAsync(IntPtr hWnd, int iCmdShow);
		}

		#endregion
	}
}
