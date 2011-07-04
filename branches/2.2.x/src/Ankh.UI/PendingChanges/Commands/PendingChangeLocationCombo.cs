// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
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
                cs.PostExecCommand(AnkhCommand.SolutionSwitchDialog, new Uri(value));
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
