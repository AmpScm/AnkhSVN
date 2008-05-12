using System;
using System.Collections.Generic;
using System.Text;

using Ankh.Commands;
using Ankh.Ids;

using WizardFramework;
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
            WizardDialog dialog = new WizardDialog(new MergeWizard());
            dialog.TopMost = false; // Currently set in the wizard framework
            dialog.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

            IUIService uiService = e.GetService<IUIService>();

            if (uiService != null)
                uiService.ShowDialog(dialog);
            else
                dialog.ShowDialog();
        }

        #endregion
    }
}
