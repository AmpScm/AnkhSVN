using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ankh.UI;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.WpfPackage.Services
{
    [GlobalService(typeof(IAnkhThreadedWaitService))]
    sealed class ThreadedWaitService : AnkhService, IAnkhThreadedWaitService
    {
        public ThreadedWaitService(IAnkhServiceProvider context)
            : base(context)
        {
        }

        sealed class ThreadedWaitWrapper : IAnkhThreadedWaitDialog
        {
            readonly IVsThreadedWaitDialog2 _dlg2;

            public ThreadedWaitWrapper(IVsThreadedWaitDialogFactory factory, string caption, string message)
            {
                if (factory == null)
                    return;

                if (!ErrorHandler.Succeeded(factory.CreateInstance(out _dlg2)))
                {
                    _dlg2 = null;
                    return;
                }

                _dlg2.StartWaitDialog(caption, message, null, null, null, 2, false, true);
            }

            public void Tick()
            {
            }

            public void Dispose()
            {
                int canceled;
                if (_dlg2 != null)
                    _dlg2.EndWaitDialog(out canceled);
            }
        }

        IVsThreadedWaitDialogFactory _factory;
        IVsThreadedWaitDialogFactory Factory
        {
            get { return _factory ?? (_factory = GetService<IVsThreadedWaitDialogFactory>(typeof(SVsThreadedWaitDialogFactory))); }
        }

        public Ankh.UI.IAnkhThreadedWaitDialog Start(string caption, string message)
        {
            return new ThreadedWaitWrapper(Factory, caption, message);
        }
    }
}
