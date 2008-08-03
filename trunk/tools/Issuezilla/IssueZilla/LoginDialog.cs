using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace IssueZilla
{
    public partial class LoginDialog : Form
    {
        public LoginDialog(IWin32Window parent)
        {
            InitializeComponent();
            this.parent = parent;
        }

        public string UserName
        {
            get { return this.usernameTextBox.Text; }
        }

        public string PassWord
        {
            get { return this.passwordTextBox.Text; }   
        }



        private IWin32Window parent;
    }
}