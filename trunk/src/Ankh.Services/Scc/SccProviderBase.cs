using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Scc
{
    public abstract class SccProviderBase : AnkhService, IVsSccProvider
    {
        protected SccProviderBase(IAnkhServiceProvider context)
            : base(context)
        {

        }

        int IVsSccProvider.AnyItemsUnderSourceControl(out int pfResult)
        {
            if (AnyItemsUnderSourceControl())
                pfResult = 1;
            else
                pfResult = 0;

            return VSErr.S_OK;
        }

        public abstract bool AnyItemsUnderSourceControl();

        public abstract int SetActive();

        public abstract int SetInactive();
    }
}
