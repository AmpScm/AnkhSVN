using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ankh.UI.Services;

namespace Ankh.UI.SvnLog
{
    public partial class EditLogMessageDialog : Form
    {
        public EditLogMessageDialog()
        {
            InitializeComponent();
        }

        public string LogMessage
        {
            get { return logMessageEditor1.Text; }
            set { logMessageEditor1.Text = value; }
        }

        IAnkhServiceProvider _context;
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set
            {
                _context = value;
                logMessageEditor1.Init(_context);
            }
        }
    }
}
