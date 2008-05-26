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

            int n = 0;
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                n++;

                if (n > 1)
                {
                    e.Enabled = false;
                    return;
                }
            }

            if (n == 0)
                e.Enabled = false;
        }

        /// <see cref="Ankh.Commands.ICommandHandler.OnExecute" />
        public void OnExecute(CommandEventArgs e)
        {
            List<SvnItem> svnItems = new List<SvnItem>();

            // TODO: Check for solution and/or project selection to use the folder instead of the file
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                svnItems.Add(item);
            }

            using (MergeWizardDialog dialog = new MergeWizardDialog())
            {
                dialog.GetWizard().MergeUtils = new MergeUtils(e.Context);
                dialog.GetWizard().MergeTarget = svnItems[0];                

                DialogResult result;

                IUIService uiService = e.GetService<IUIService>();

                // TODO: Use
                //result = uiService.ShowDialog(dialog);

                result = dialog.ShowDialog(uiService.GetDialogOwnerWindow());

                if (result == DialogResult.OK)
                    MessageBox.Show("AnkhSVN merge functionality is not complete and is a work in progress.  " +
                        "This feature may not work as expected.  " +
                        "Please check the website for a newer build or if you have the latest, contact " +
                        "the developers for an estimated delivery date.", "AnkhSVN Merge", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
            }
        }

        #endregion
    }
}
