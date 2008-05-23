using System;
using System.Collections.Generic;
using System.Text;

using Ankh.Commands;
using Ankh.Ids;

using WizardFramework;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Ankh.UI.MergeWizard.Commands
{
    [Command(AnkhCommand.ItemMerge)]
    class Merge : ICommandHandler
    {
        #region ICommandHandler Members

        /// <see cref="Ankh.Commands.ICommandHandler.OnUpdate" />
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            if (!e.State.SccProviderActive)
            {
                e.Visible = e.Enabled = false;
                return;
            }

            List<SvnItem> items = new List<SvnItem>();

            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                items.Add(item);
            }

            if (items.Count == 1)
                return;

            e.Enabled = false;
        }

        /// <see cref="Ankh.Commands.ICommandHandler.OnExecute" />
        public void OnExecute(CommandEventArgs e)
        {
            List<SvnItem> svnItems = new List<SvnItem>();

            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                svnItems.Add(item);
            }

            using (MergeWizardDialog dialog = new MergeWizardDialog())
            {
                if (dialog is MergeWizardDialog)
                {
                    ((MergeWizardDialog)dialog).GetWizard().MergeUtils = new MergeUtils(e.Context);
                    ((MergeWizardDialog)dialog).GetWizard().MergeTarget = svnItems[0];
                }


                DialogResult result;

                IUIService uiService = e.GetService<IUIService>();

                // TODO: Use
                //result = uiService.ShowDialog(dialog);

                result = dialog.ShowDialog(uiService.GetDialogOwnerWindow());

                if (result == DialogResult.OK)
                    MessageBox.Show("AnkhSVN merge functionality is not complete and is a work in progress.  " +
                        "Until Ankh 2.0 releases at the end of May, this feature may not work as expected.  " +
                        "Please check the website for a newer build or if you have the latest, contact " +
                        "the developers for an estimated delivery date.", "AnkhSVN Merge", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
            }
        }

        #endregion
    }
}
