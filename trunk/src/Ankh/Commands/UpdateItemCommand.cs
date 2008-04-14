// $Id$
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using SharpSvn;

using Ankh.UI;
using AnkhSvn.Ids;
using Ankh.VS;
using Ankh.Selection;
using System.Collections.ObjectModel;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that updates an item.
    /// </summary>
    [Command(AnkhCommand.UpdateItem)]
    public class UpdateItem : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsVersioned)
                    return;
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();
            // save all files
            //SaveAllDirtyDocuments(e.Context);

            using (e.Context.BeginOperation("Updating"))
            {
				Collection<SvnItem> files = new Collection<SvnItem>();
                foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
                {
                    if (item.IsVersioned)
                        files.Add(item);
                }

                // we assume by now that all items are working copy resources.                
                UpdateRunner runner = new UpdateRunner(context, files);
                if (!runner.MaybeShowUpdateDialog())
                    return;

                e.GetService<IProgressRunner>().Run("Updating", runner.Work);
            }
        }

        #endregion

        #region UpdateVisitor
        private class UpdateRunner : IProgressWorker
        {
			public UpdateRunner(IContext context, Collection<SvnItem> resources)
            {
                this.context = context;
                this.resources = resources;
            }

            public IContext Context
            {
                get { return this.context; }
            }

            /// <summary>
            /// Show the update dialog if wanted.
            /// </summary>
            /// <returns></returns>
            public bool MaybeShowUpdateDialog()
            {
                this.depth = SvnDepth.Empty;
                this.revision = SvnRevision.Head;

                // We're using the update dialog no matter what to
                // take advantage of it's path processing capabilities.
                // This is the best way to ensure holding down Shift is
                // equivalent to accepting the default in the dialog.
                using (UpdateDialog d = new UpdateDialog())
                {
                    d.Context = Context;
                    d.GetPathInfo += new EventHandler<ResolvingPathEventArgs>(GetPathInfo);
                    d.Items = this.resources;
                    d.CheckedFilter = delegate{return true;};
                    d.Recursive = true;

                    if (!CommandBase.Shift)
                    {
                        IAnkhDialogOwner owner = Context.GetService<IAnkhDialogOwner>();

                        if (d.ShowDialog(owner.DialogOwner) != DialogResult.OK)
                            return false;
                    }

                    depth = d.Recursive ? SvnDepth.Infinity : SvnDepth.Empty;
                    this.resources = new List<SvnItem>(d.CheckedItems);
                    this.revision = d.Revision;
                }

                // the user hasn't cancelled the update
                return true;
            }

            public static void GetPathInfo(object sender, ResolvingPathEventArgs args)
            {
                SvnItem item = (SvnItem)args.Item;
                args.IsDirectory = item.IsDirectory;
                args.Path = item.FullPath;
            }


            /// <summary>
            /// The actual updating happens here.
            /// </summary>
            public void Work(object sender, ProgressWorkerArgs e)
            {
                ICollection<string> paths = SvnItem.GetPaths(this.resources);
                SvnUpdateArgs args = new SvnUpdateArgs();
                args.Notify += new EventHandler<SvnNotifyEventArgs>(OnNotificationEventHandler);
                args.Revision = revision;
                args.Depth = depth;
                args.IgnoreExternals = false;

                e.Client.Update(paths, args);

                if (this.conflictsOccurred)
                {
                    IAnkhTaskManager manager = e.Context.GetService<IAnkhTaskManager>();

                    manager.NavigateTaskList();
                }
            }

            /// <summary>
            ///  Handle event for onNotification that conflicts occurred from update
            /// </summary>
            /// <param name="taskItem"></param>
            /// <param name="navigateHandled"></param>
            private void OnNotificationEventHandler(Object sender, SvnNotifyEventArgs args)
            {
                if (args.ContentState == SvnNotifyState.Conflicted)
                {
                    IAnkhTaskManager manager = Context.GetService<IAnkhTaskManager>();

                    if (manager != null)
                        manager.AddConflictTask(args.Path);
                    this.conflictsOccurred = true;
                }
            }

			private ICollection<SvnItem> resources;
            private SvnRevision revision;
            private SvnDepth depth;
            private bool conflictsOccurred = false;
            private IContext context;
        }
        #endregion
    }
}