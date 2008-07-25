using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Scc;
using SharpSvn;
using System.ComponentModel;
using Ankh.Configuration;

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
            args.Conflict += new EventHandler<SvnConflictEventArgs>(new Handler(this, synch).OnConflict);
        }

        #endregion

        class Handler : AnkhService
        {
            ISynchronizeInvoke _synchronizer;
            MergeConflictHandler _currentMergeConflictHandler;

            public Handler(IAnkhServiceProvider context, ISynchronizeInvoke synchronizer)
                : base(context)
            {
                _synchronizer = synchronizer;
            }

            public void OnConflict(object sender, SvnConflictEventArgs e)
            {
                if (_synchronizer != null && _synchronizer.InvokeRequired)
                {
                    // If needed marshall the call to the UI thread

                    e.Detach(); // Make this instance thread safe!

                    _synchronizer.Invoke(new EventHandler<SvnConflictEventArgs>(OnConflict), new object[] { sender, e });
                    return;
                }

                AnkhConfig config = GetService<IAnkhConfigurationService>().Instance;

                if (config.InteractiveMergeOnConflict)
                {
                    // Only call interactive merge if the user opted in on it
                    if (_currentMergeConflictHandler == null)
                        _currentMergeConflictHandler = CreateMergeConflictHandler();

                    _currentMergeConflictHandler.OnConflict(e);
                }
            }

            private MergeConflictHandler CreateMergeConflictHandler()
            {
                MergeConflictHandler mergeConflictHandler = new MergeConflictHandler(Context);
                mergeConflictHandler.PromptOnBinaryConflict = true;
                mergeConflictHandler.PromptOnTextConflict = true;
                return mergeConflictHandler;
            }
        }
    }
}
