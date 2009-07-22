// $Id$
//
// Copyright 2008 The AnkhSVN Project
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ankh.Scc;

namespace Ankh.UI.SccManagement
{
    public partial class SccEditEnlistment : VSContainerForm
    {
        readonly EnlistmentState _enlistState;
        protected SccEditEnlistment()
        {
            InitializeComponent();
        }

        public SccEditEnlistment(EnlistmentState state)
            : this()
        {
            if (state == null)
                throw new ArgumentNullException("state");
            _enlistState = state;

            projectLocationGroup.Enabled = state.NeedLocation;
            debugLocationBox.Enabled = state.NeedDebugLocation;

            projectLocationBrowse.Enabled = state.CanBrowseForLocation;
            debugLocationBrowse.Enabled = state.CanBrowseForDebugLocation;

            projectLocationBox.ReadOnly = state.AllowEditLocation;
            debugLocationBox.ReadOnly = state.AllowEditDebugLocation;

            projectLocationBox.Text = state.Location;
            debugLocationBox.Text = state.DebugLocation;
        }

        public EnlistmentState EnlistState
        {
            get { return _enlistState; }
        }

        private void projectLocationBrowse_Click(object sender, EventArgs e)
        {
            if (EnlistState != null)
            {
                EnlistState.InvokeBrowseLocation();

                if (EnlistState.Location != projectLocationBox.Text)
                    projectLocationBox.Text = EnlistState.Location;
            }
        }

        private void debugLocationBrowse_Click(object sender, EventArgs e)
        {
            if (EnlistState != null)
            {
                EnlistState.InvokeBrowseDebugLocation();

                if (EnlistState.DebugLocation != debugLocationBox.Text)
                    debugLocationBox.Text = projectLocationBox.Text;
            }
        }

        private void projectLocationBox_TextChanged(object sender, EventArgs e)
        {
            if (EnlistState == null)
                return;

            EnlistState.Location = projectLocationBox.Text;
        }

        private void debugLocationBox_TextChanged(object sender, EventArgs e)
        {
            if (EnlistState == null)
                return;

            EnlistState.DebugLocation = debugLocationBox.Text;
        }

        private void projectLocationBox_Validating(object sender, CancelEventArgs e)
        {
            if (EnlistState == null)
                return;

            if (EnlistState.Location != projectLocationBox.Text)
                e.Cancel = true;
        }

        private void debugLocationBox_Validating(object sender, CancelEventArgs e)
        {
            if (EnlistState == null)
                return;

            if (EnlistState.DebugLocation != debugLocationBox.Text)
                e.Cancel = true;
        }
    }
}
