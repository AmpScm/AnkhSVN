// $Id$
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Ankh.UI;
using AnkhSvn.Ids;
using SharpSvn;

namespace Ankh.Commands
{
	/// <summary>
	/// A command that updates an item.
	/// </summary>
	[VSNetCommand(AnkhCommand.UpdateItem,
		"UpdateItem",
		 Text = "&Update...",
		 Tooltip = "Updates local items with changes from the Subversion repository.",
		 Bitmap = ResourceBitmaps.Update),
		 VSNetItemControl("", Position = 3)]
	public class UpdateItem : CommandBase
	{
		#region Implementation of ICommand

		public override void OnUpdate(CommandUpdateEventArgs e)
		{
            foreach (string file in e.Selection.GetSelectedFiles(true))
            {
                return;
            }
            e.Enabled = false;
		}

        public override void OnExecute(CommandEventArgs e)
        {
            // save all files
            SaveAllDirtyDocuments(e.Context);

            using (e.Context.StartOperation("Updating"))
            {

                List<SvnItem> files = new List<SvnItem>(e.Selection.GetSelectedSvnItems(true));

                // we assume by now that all items are working copy resources.                
                UpdateRunner runner = new UpdateRunner(e.Context, files);
                if (!runner.MaybeShowUpdateDialog())
                    return;

                e.Context.UIShell.RunWithProgressDialog(runner, "Updating");
            }
        }

		#endregion

		#region UpdateVisitor
		private class UpdateRunner : IProgressWorker
		{
			public UpdateRunner(IContext context, IList resources)
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
					d.GetPathInfo += new ResolvingPathInfoHandler(SvnItem.GetPathInfo);
					d.Items = this.resources;
					d.CheckedItems = this.resources;
					d.Recursive = true;

					if (!CommandBase.Shift)
					{
						if (d.ShowDialog(this.Context.HostWindow) != DialogResult.OK)
							return false;
					}

					depth = d.Recursive ? SvnDepth.Infinity : SvnDepth.Empty;
					this.resources = d.CheckedItems;
					this.revision = d.Revision;
				}

				// the user hasn't cancelled the update
				return true;
			}

			/// <summary>
			/// The actual updating happens here.
			/// </summary>
			public void Work(IContext context)
			{
				string[] paths = SvnItem.GetPaths(this.resources);
				SvnUpdateArgs args = new SvnUpdateArgs();
				args.Notify += new EventHandler<SvnNotifyEventArgs>(OnNotificationEventHandler);
				args.Revision = revision;
				args.Depth = depth;
				args.IgnoreExternals = false;
				context.Client.Update(paths, args);

				if (this.conflictsOccurred)
					context.ConflictManager.NavigateTaskList();
			}

			/// <summary>
			///  Handlke event for onNotification that conflicts occurred from update
			/// </summary>
			/// <param name="taskItem"></param>
			/// <param name="navigateHandled"></param>
			private void OnNotificationEventHandler(Object sender, SvnNotifyEventArgs args)
			{
				if (args.ContentState == SvnNotifyState.Conflicted)
				{
					this.Context.ConflictManager.AddTask(args.Path);
					this.conflictsOccurred = true;
				}
			}

			private IList resources;
			private SvnRevision revision;
			private SvnDepth depth;
			private bool conflictsOccurred = false;
			private IContext context;
		}
		#endregion
	}
}