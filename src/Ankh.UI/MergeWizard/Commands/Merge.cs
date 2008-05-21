using System;
using System.Collections.Generic;
using System.Text;

using Ankh.Commands;
using Ankh.Ids;

using WizardFramework;
using System.Windows.Forms;

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

            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsVersioned)
                    return;
            }
            e.Enabled = false;
        }

        /// <see cref="Ankh.Commands.ICommandHandler.OnExecute" />
        public void OnExecute(CommandEventArgs e)
        {
            WizardDialog dialog = new MergeWizardDialog();
            DialogResult result;

            result = dialog.ShowDialog();
            
            if (result == DialogResult.OK)
                MessageBox.Show("AnkhSVN merge functionality is not complete and is a work in progress.  " +
                    "Until Ankh 2.0 releases at the end of May, this feature may not work.  " +
                    "Please check the website for a newer build or if you have the latest, contact " +
                    "the developers for an estimated delivery date.", "AnkhSVN Merge", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
        }

        #endregion
    }
}
