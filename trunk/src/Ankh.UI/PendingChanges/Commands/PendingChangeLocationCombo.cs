using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;
using Ankh.VS;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace Ankh.UI.PendingChanges.Commands
{
    [Command(AnkhCommand.SolutionSwitchComboFill)]
    [Command(AnkhCommand.SolutionSwitchCombo)]
    class PendingChangeLocationCombo : ICommandHandler, IComponent
    {
        ISite _site;
        IAnkhSolutionSettings _settings;
        #region IComponent Members

        event EventHandler IComponent.Disposed
        {
            add { }
            remove { }
        }

        public ISite Site
        {
            get { return _site; }
            set
            {
                _site = value;
            }
        }

        public void Dispose()
        {
            
        }

        #endregion


        public void OnUpdate(CommandUpdateEventArgs e)
        {
            if (ProjectRootUri == null)
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

        protected IAnkhSolutionSettings SolutionSettings
        {
            get
            {
                if (_settings == null && _site != null)
                    _settings = (IAnkhSolutionSettings)_site.GetService(typeof(IAnkhSolutionSettings));

                return _settings;
            }
        }


        protected Uri ProjectRootUri
        {
            get
            {
                if (SolutionSettings != null)
                    return SolutionSettings.ProjectRootUri;

                return null;
            }
        }


        void OnExecuteFill(CommandEventArgs e)
        {
            if (ProjectRootUri != null)
                e.Result = new string[] { ProjectRootUri.ToString(), "Other..." };
        }

        void OnExecuteSet(CommandEventArgs e)
        {
            string value = (string)e.Argument;

            IAnkhCommandService cs = e.GetService<IAnkhCommandService>();

            if (value != null && value == "Other...")
                cs.PostExecCommand(AnkhCommand.SolutionSwitchDialog);
            else
                cs.PostExecCommand(AnkhCommand.SolutionSwitchDialog, value);
        }

        void OnExecuteGet(CommandEventArgs e)
        {
            if (ProjectRootUri != null)
                e.Result = ProjectRootUri.ToString();
        }

        void OnExecuteFilter(CommandEventArgs e)
        {
            // Not called on us; but empty handler would tell: pass through
        }
    }
}
