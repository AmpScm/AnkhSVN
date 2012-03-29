using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ankh.UI;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.VS.Services
{
    [GlobalService(typeof(IAnkhThreadedWaitService), MaxVersion=VSInstance.VS2008)]
    sealed class ThreadedWaitService : AnkhService, IAnkhThreadedWaitService
    {
        public ThreadedWaitService(IAnkhServiceProvider context)
            : base(context)
        {
        }

        sealed class ThreadedWaitWrapper : IAnkhThreadedWaitDialog
        {
            readonly DateTime _start;
            readonly IAnkhServiceProvider _context;
            string _caption;
            string _message;
            IVsThreadedWaitDialog _dlg;

            public ThreadedWaitWrapper(IAnkhServiceProvider context, string caption, string message)
            {
                if (context == null)
                    throw new ArgumentNullException("context");

                _context = context;
                _caption = caption;
                _message = message;
                _start = DateTime.UtcNow + new TimeSpan(0,0,2);
            }

            public void Tick()
            {
                if (_dlg == null)
                {
                    if (_start > DateTime.UtcNow)
                        return;

                    IVsThreadedWaitDialog dlg = _context.GetService<IVsThreadedWaitDialog>(typeof(SVsThreadedWaitDialog));

                    if (dlg != null)
                    {
                        if (ErrorHandler.Succeeded(dlg.StartWaitDialog(_caption, _message, null, (uint)__VSTWDFLAGS.VSTWDFLAGS_TOPMOST, null, null)))
                            _dlg = dlg;
                    }
                }
                else
                {
                    int canceled;
                    _dlg.GiveTimeSlice(null, null, 0, out canceled);
                }
            }

            public void Dispose()
            {
                
                if (_dlg != null)
                    try
                    {
                        int canceled = 0;
                        _dlg.EndWaitDialog(ref canceled);
                    }
                    finally
                    {
                        _dlg = null;
                    }
            }
        }

        public Ankh.UI.IAnkhThreadedWaitDialog Start(string caption, string message)
        {
            return new ThreadedWaitWrapper(this, caption, message);
        }
    }
}
