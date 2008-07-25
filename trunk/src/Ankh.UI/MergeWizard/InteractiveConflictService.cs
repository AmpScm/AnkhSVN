using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Scc;

namespace Ankh.UI.MergeWizard
{
    class InteractiveConflictService : AnkhService, IConflictHandler
    {
        public InteractiveConflictService(IAnkhServiceProvider context)
            : base(context)
        {
        }
        #region IConflictHandler Members

        public void RegisterConflictHandler(SharpSvn.SvnClientArgsWithConflict args, System.ComponentModel.ISynchronizeInvoke synch)
        {
            
        }

        #endregion
    }
}
