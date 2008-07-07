#region Copyright And Revision History

/*---------------------------------------------------------------------------

	Copyright © 2003 Bill Menees.  All rights reserved.
	Bill@Menees.com

	$History: ApplicationExceptionHandler.cs $
	
	*****************  Version 2  *****************
	User: Bill         Date: 3/05/08    Time: 8:30p
	Updated in $/CSharp/Menees/Classes
	Added support for AppDomain.UnhandledException.

-----------------------------------------------------------------------------*/

#endregion

#region Using Directives

using System;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

#endregion

namespace Ankh.Diff
{
	/// <summary>
	/// Used to handle uncaught exceptions in an application.
	/// </summary>
	public static class ApplicationExceptionHandler
	{
        #region Public Methods

		/// <summary>
		/// Call this in Main before calling Application.Run().
		/// </summary>
		public static void Initialize()
		{
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException, false);
			Application.ThreadException += OnThreadException;
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
		}

		/// <summary>
		/// Begins a block where an exception should be rethrown
		/// rather than displayed in a message box.
		/// </summary>
		/// <remarks>
		/// These calls can be nested because they are reference
		/// counted internally.
		/// </remarks>
		public static void BeginRethrow()
		{
			Interlocked.Increment(ref s_iRethrow);
		}

		/// <summary>
		/// Ends a rethrow block begun by <see cref="BeginRethrow"/>
		/// </summary>
		public static void EndRethrow()
		{
			Interlocked.Decrement(ref s_iRethrow);
		}

		#endregion

        #region Private Methods

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;
			if (ex != null)
			{
				HandleException(ex);
			}
		}

		private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
		{
			HandleException(e.Exception);
		}

		private static void HandleException(Exception ex)
		{
			//In rare cases, Win32 wrapped controls will throw native exceptions,
			//and the Control.WndProcException method calls Application.OnThreadException
			//directly without letting the exception propagate up to the caller.
			//For known cases where that occurs (e.g. MegaBuild.MainForm.OnOutputAdded),
			//a Begin/EndRethrow block can be used to force the exception to be rethrown.
			if (s_iRethrow > 0)
			{
				throw ex;
			}
			else
			{
				int iShowing = Interlocked.Increment(ref s_iShowingError);
				try
				{
					//Only let the first thread in show a message box.
					if (iShowing == 1)
					{
						Utilities.ShowError(ex.Message);
					}
					else
					{
						//If a message box is currently displayed by another thread,
						//we'll just try to dump the message out via Trace.
						Trace.WriteLine("EXCEPTION: " + ex.Message);
					}
				}
				finally
				{
					Interlocked.Decrement(ref s_iShowingError);
				}
			}
		}

		#endregion

        #region Private Data Members

		private static int s_iShowingError = 0;
		private static int s_iRethrow = 0;

		#endregion
	}
}
