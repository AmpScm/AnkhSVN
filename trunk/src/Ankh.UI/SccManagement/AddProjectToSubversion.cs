using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ankh.VS;

namespace Ankh.UI.SccManagement
{
    public partial class AddProjectToSubversion : AddToSubversion
    {
        public AddProjectToSubversion()
        {
            InitializeComponent();
        }

        private void AddProjectToSubversion_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.Cancel)
                return; // Always allow cancel

            IAnkhSolutionSettings ss = Context.GetService<IAnkhSolutionSettings>();

            if (RepositoryAddUrl == null)
            {
                // Error on this is handled in base
                return;
            }

            // Error if the RepositoryAddUrl is below the url of the projectroot
            if (ss.ProjectRootUri.IsBaseOf(RepositoryAddUrl))
            {
                e.Cancel = true;

                errorProvider1.SetError(repositoryTree, "Please select a location that is not below the solution binding path, or move the project to a directory below the solution binding path on disk");
                return;
            }
        }
    }
}
