using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using AnkhSvn.Ids;
using Ankh.VS;

namespace Ankh.UI.PendingChanges.Commands
{
    [Command(AnkhCommand.SolutionSwitchComboFill)]
    [Command(AnkhCommand.SolutionSwitchCombo)]
    class PendingChangeLocationCombo : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            IAnkhSolutionSettings settings = e.Context.GetService<IAnkhSolutionSettings>();

            if (settings == null || settings.ProjectRootUri == null)
                e.Enabled = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            // For combobox processing we have 4 cases
            
            if (e.Command == AnkhCommand.SolutionSwitchComboFill)
                OnExecuteFill(e); // Fill the list of options
            else if (e.Argument != null)
            {
                string value = e.Argument as string;

                if (value != null)
                    OnExecuteSet(e); // When the user selected a value
                else
                    OnExecuteFilter(e); // Keyboard hook filter (selected in ctc)
            }
            else
                OnExecuteGet(e); // Get the current value
        }

        void OnExecuteFill(CommandEventArgs e)
        {
            e.Result = new string[] { "http://subversion.lan/project/trunk", "http://subversion.lan/project/branches/other" };        
        }

        string _currentValue = "http://subversion.lan/project/trunk";
        void OnExecuteSet(CommandEventArgs e)
        {
            _currentValue = (string)e.Argument ?? "";
        }

        void OnExecuteGet(CommandEventArgs e)
        {
            IAnkhSolutionSettings settings = e.Context.GetService<IAnkhSolutionSettings>();

            if (settings != null)
            {
                Uri uri = settings.ProjectRootUri;

                if (uri != null)
                    e.Result = uri.ToString();
            }
            else
                e.Result = _currentValue; 
        }

        void OnExecuteFilter(CommandEventArgs e)
        {
            // Not called on us; but empty handler would tell: pass through
        }
    }
}
