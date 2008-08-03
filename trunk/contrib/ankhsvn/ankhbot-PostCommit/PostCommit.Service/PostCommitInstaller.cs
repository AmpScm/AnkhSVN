using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;

namespace PostCommit.Service
{
    [RunInstaller( true )]
    public partial class PostCommitInstaller : Installer
    {
        public PostCommitInstaller()
        {
            InitializeComponent();
        }
    }
}