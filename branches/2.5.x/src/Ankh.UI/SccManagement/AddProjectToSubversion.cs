// $Id$
//
// Copyright 2009 The AnkhSVN Project
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
using Ankh.VS;
using System.Diagnostics;

namespace Ankh.UI.SccManagement
{
    public partial class AddProjectToSubversion : AddToSubversion
    {
        public AddProjectToSubversion()
        {
            InitializeComponent();
        }

        protected override void ValidateAdd(object sender, CancelEventArgs e)
        {
            base.ValidateAdd(sender, e);

            if (e.Cancel)
                return;

            Debug.Assert(RepositoryAddUrl != null);

            IAnkhSolutionSettings ss = Context.GetService<IAnkhSolutionSettings>();

            // Error if the RepositoryAddUrl is below the url of the projectroot
            if (ss.ProjectRootUri.IsBaseOf(RepositoryAddUrl))
            {
                e.Cancel = true;

                errorProvider1.SetError(repositoryTree, "Please select a location that is not below the solution binding path, or move the project to a directory below the solution binding path on disk");
                return;
            }
        }

        [DefaultValue(true)]
        public bool MarkAsManaged
        {
            get { return markAsManaged.Checked; }
            set { markAsManaged.Checked = value; }
        }

        [DefaultValue(true)]
        public bool WriteCheckOutInformation
        {
            get { return writeUrlInSolution.Checked; }
            set { writeUrlInSolution.Checked = value; }
        }
    }
}
