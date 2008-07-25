using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Scc;
using SharpSvn;

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
            args.Conflict += new EventHandler<SvnConflictEventArgs>(OnConflict);
        }

        #endregion

        MergeConflictHandler _currentMergeConflictHandler;
        private void OnConflict(object sender, SvnConflictEventArgs args)
        {
            if (_currentMergeConflictHandler == null)
            {
                _currentMergeConflictHandler = CreateMergeConflictHandler();
            }
            
            _currentMergeConflictHandler.OnConflict(args);
        }

        private MergeConflictHandler CreateMergeConflictHandler()
        {
            MergeConflictHandler mergeConflictHandler = new MergeConflictHandler();
            mergeConflictHandler.PromptOnBinaryConflict = true;
            mergeConflictHandler.PromptOnTextConflict = true;
            return mergeConflictHandler;
        }
    }
}
