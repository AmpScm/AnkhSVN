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
