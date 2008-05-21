using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Ankh.Ids;
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

            int hr = CommandMapper.QueryStatus(Context, cCmds, prgCmds, pCmdText);


            return hr;
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
				return VSConstants.E_NOTIMPL;
			}
			
			object argIn = null;

			if(pvaIn != IntPtr.Zero)
				argIn = Marshal.GetObjectForNativeVariant(pvaIn);

			CommandEventArgs args = new CommandEventArgs(
				(AnkhCommand)nCmdID, 
				Context,
				argIn,
				(OLECMDEXECOPT)nCmdexecopt == OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER,
				(OLECMDEXECOPT)nCmdexecopt == OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER);

           if (!CommandMapper.Execute(args.Command, args))
                return (int)OLEConstants.OLECMDERR_E_DISABLED;

			if (pvaOut != IntPtr.Zero)
			{
				Marshal.GetNativeVariantForObject(args.Result, pvaOut);
			}

			return VSConstants.S_OK;
		}

		IContext _context;
		public IContext AnkhContext
		{
			get { return _context ?? (_context = GetService<IContext>()); }
		}

        public T GetService<T>()
            where T : class
        {
            return (T)GetService(typeof(T));
        }

        public T GetService<T>(Type serviceType)
            where T : class
        {
            return (T)GetService(serviceType);
        }

		Ankh.Commands.CommandMapper _mapper;

		public Ankh.Commands.CommandMapper CommandMapper
		{
			get
			{
                return _mapper ?? (_mapper = _runtime.CommandMapper);
			}
		}		
	}
}
