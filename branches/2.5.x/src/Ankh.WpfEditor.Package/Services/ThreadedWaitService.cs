using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

using Ankh.UI;

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

                if (!VSErr.Succeeded(factory.CreateInstance(out _dlg2)))
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
