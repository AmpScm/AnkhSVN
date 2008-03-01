using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using AnkhSvn.Ids;
using Microsoft.VisualStudio;
using System.Globalization;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using OLEConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using Ankh.Commands;
using Ankh;
using VSConstants = Microsoft.VisualStudio.VSConstants;

namespace Ankh.VSPackage
{
	// The command routing at package level
	public partial class AnkhSvnPackage : IOleCommandTarget
	{
		/// <summary>
		/// Queries the status of a command handled by this package
		/// </summary>
		/// <param name="pguidCmdGroup">The guid of the Command.</param>
		/// <param name="cCmds">The number of commands.</param>
		/// <param name="prgCmds">The Commands to update.</param>
		/// <param name="pCmdText">Command text update pointer.</param>
		/// <returns></returns>
		public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
		{
			if ((prgCmds == null))
				return VSConstants.E_INVALIDARG;

			Debug.Assert(cCmds == 1, "Multiple commands"); // Should never happen in VS

			if (pguidCmdGroup != AnkhId.CommandSetGuid)
			{
				// Filter out commands that are not defined by this package
				return (int)OLEConstants.OLECMDERR_E_NOTSUPPORTED;
			}

			TextQueryType textQuery = TextQueryType.None;
			string oldText = null;

			if (pCmdText != IntPtr.Zero)
			{
				// VS Want's some text from us for either the statusbar or the command text
				OLECMDTEXTF textType = GetFlags(pCmdText);

				switch (textType)
				{
					case OLECMDTEXTF.OLECMDTEXTF_NAME:
						textQuery = TextQueryType.Name;
						break;
					case OLECMDTEXTF.OLECMDTEXTF_STATUS:
						textQuery = TextQueryType.Status;
						break;
				}

				oldText = GetText(pCmdText);
			}

			CommandUpdateEventArgs updateArgs = new CommandUpdateEventArgs((AnkhCommand)prgCmds[0].cmdID, AnkhContext, textQuery, oldText);

			OLECMDF cmdf = OLECMDF.OLECMDF_SUPPORTED;

			if (CommandMapper.PerformUpdate(updateArgs.Command, updateArgs))
			{
				if (updateArgs.Enabled)
					cmdf |= OLECMDF.OLECMDF_ENABLED;

				if (updateArgs.Latched)
					cmdf |= OLECMDF.OLECMDF_LATCHED;

				if (updateArgs.Ninched)
					cmdf |= OLECMDF.OLECMDF_NINCHED;
			}

			if (textQuery != TextQueryType.None)
			{
				SetText(pCmdText, updateArgs.Text ?? updateArgs.Command.ToString());
			}

			prgCmds[0].cmdf = (uint)cmdf;

			return VSConstants.S_OK;
		}


		public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
		{
			if(pguidCmdGroup != AnkhId.CommandSetGuid)
			{
				return (int)OLEConstants.OLECMDERR_E_UNKNOWNGROUP;
			}

			if((OLECMDEXECOPT)nCmdexecopt == OLECMDEXECOPT.OLECMDEXECOPT_SHOWHELP)
			{
				// Informally confirmed by MS: Never raised by VS (Office only)
				return VSConstants.S_OK;
			}
			
			// Ok, now make sure we have the command enabled and it's somebody just running commands
			// at random.

			CommandUpdateEventArgs updateArgs = new CommandUpdateEventArgs((AnkhCommand)nCmdID, AnkhContext);

			if (!CommandMapper.PerformUpdate(updateArgs.Command, updateArgs) || !updateArgs.Enabled)
				return (int)OLEConstants.OLECMDERR_E_DISABLED; // Command is disabled, don't process any further
			
			object argIn = null;

			if(pvaIn != IntPtr.Zero)
				Marshal.GetObjectForNativeVariant(pvaIn);

			CommandEventArgs args = new CommandEventArgs(
				(AnkhCommand)nCmdID, 
				AnkhContext,
				argIn,
				(OLECMDEXECOPT)nCmdexecopt == OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER,
				(OLECMDEXECOPT)nCmdexecopt == OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER);

			// TODO: enable something like
			// AnkhCommands.PerformCommand(args.Command, args);
			CommandMapper.Execute(args.Command, args);

			if (args.Result != null && pvaOut != IntPtr.Zero)
			{
				Marshal.GetNativeVariantForObject(args.Result, pvaOut);
			}

			return VSConstants.S_OK;
		}

		AnkhContext _context;
		public AnkhContext AnkhContext
		{
			get
			{
				if (_context == null)
					_context = new AnkhContext(this);

				return _context;
			}
		}

		Ankh.Commands.Mapper.CommandMapper _mapper;

		public Ankh.Commands.Mapper.CommandMapper CommandMapper
		{
			get
			{
				if (_mapper == null)
					_mapper = new Ankh.Commands.Mapper.CommandMapper();

				return _mapper;
			}
		}

		private void Temporary_OnCommand(CommandEventArgs e)
		{
			// Show a Message Box to prove we were here
			Microsoft.VisualStudio.Shell.Interop.IVsUIShell uiShell = (IVsUIShell)GetService(typeof(Microsoft.VisualStudio.Shell.Interop.SVsUIShell));
			Guid clsid = Guid.Empty;
			int result;
			Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
					   0,
					   ref clsid,
					   "AnkhSvn",
					   string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback() for {1}", this.ToString(), e.Command),
					   string.Empty,
					   0,
					   OLEMSGBUTTON.OLEMSGBUTTON_OK,
					   OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
					   OLEMSGICON.OLEMSGICON_INFO,
					   0,        // false
					   out result));
		}


		#region // Interop code from: VS2008SDK\VisualStudioIntegration\Common\Source\CSharp\Project\Misc\NativeMethods.cs

		/// <summary>
		/// Gets the flags of the OLECMDTEXT structure
		/// </summary>
		/// <param name="pCmdTextInt">The structure to read.</param>
		/// <returns>The value of the flags.</returns>
		static OLECMDTEXTF GetFlags(IntPtr pCmdTextInt)
		{
			Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT pCmdText = (Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT)Marshal.PtrToStructure(pCmdTextInt, typeof(Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT));

			if ((pCmdText.cmdtextf & (int)OLECMDTEXTF.OLECMDTEXTF_NAME) != 0)
				return OLECMDTEXTF.OLECMDTEXTF_NAME;

			if ((pCmdText.cmdtextf & (int)OLECMDTEXTF.OLECMDTEXTF_STATUS) != 0)
				return OLECMDTEXTF.OLECMDTEXTF_STATUS;

			return OLECMDTEXTF.OLECMDTEXTF_NONE;
		}

		/// <include file='doc\NativeMethods.uex' path='docs/doc[@for="OLECMDTEXTF.SetText"]/*' />
		/// <devdoc>
		/// Accessing the text of this structure is very cumbersome.  Instead, you may
		/// use this method to access an integer pointer of the structure.
		/// Passing integer versions of this structure is needed because there is no
		/// way to tell the common language runtime that there is extra data at the end of the structure.
		/// </devdoc>
		/// <summary>
		/// Sets the text inside the structure starting from an integer pointer.
		/// </summary>
		/// <param name="pCmdTextInt">The integer pointer to the position where to set the text.</param>
		/// <param name="text">The text to set.</param>
		static void SetText(IntPtr pCmdTextInt, string text)
		{
			Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT pCmdText = (Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT)Marshal.PtrToStructure(pCmdTextInt, typeof(Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT));
			char[] menuText = text.ToCharArray();

			// Get the offset to the rgsz param.  This is where we will stuff our text
			//
			IntPtr offset = Marshal.OffsetOf(typeof(Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT), "rgwz");
			IntPtr offsetToCwActual = Marshal.OffsetOf(typeof(Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT), "cwActual");

			// The max chars we copy is our string, or one less than the buffer size,
			// since we need a null at the end.
			//
			int maxChars = Math.Min((int)pCmdText.cwBuf - 1, menuText.Length);

			Marshal.Copy(menuText, 0, (IntPtr)((long)pCmdTextInt + (long)offset), maxChars);

			// append a null character
			Marshal.WriteInt16((IntPtr)((long)pCmdTextInt + (long)offset + maxChars * 2), 0);

			// write out the length
			// +1 for the null char
			Marshal.WriteInt32((IntPtr)((long)pCmdTextInt + (long)offsetToCwActual), maxChars + 1);
		}

		/// <devdoc>
		/// Accessing the text of this structure is very cumbersome.  Instead, you may
		/// use this method to access an integer pointer of the structure.
		/// Passing integer versions of this structure is needed because there is no
		/// way to tell the common language runtime that there is extra data at the end of the structure.
		/// </devdoc>
		static string GetText(IntPtr pCmdTextInt)
		{
			Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT pCmdText = (Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT)Marshal.PtrToStructure(pCmdTextInt, typeof(Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT));

			// Get the offset to the rgsz param.
			//
			IntPtr offset = Marshal.OffsetOf(typeof(Microsoft.VisualStudio.OLE.Interop.OLECMDTEXT), "rgwz");

			// Punt early if there is no text in the structure.
			//
			if (pCmdText.cwActual == 0)
			{
				return String.Empty;
			}

			char[] text = new char[pCmdText.cwActual - 1];

			Marshal.Copy((IntPtr)((long)pCmdTextInt + (long)offset), text, 0, text.Length);

			StringBuilder s = new StringBuilder(text.Length);
			s.Append(text);
			return s.ToString();
		}
		#endregion
	}
}
