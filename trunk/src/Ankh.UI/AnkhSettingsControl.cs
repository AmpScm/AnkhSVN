using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ankh.Commands;
using AnkhSvn.Ids;

namespace Ankh.UI
{
    public partial class AnkhSettingsControl : UserControl
    {
        IAnkhServiceProvider _context;
        public AnkhSettingsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set { _context = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IAnkhCommandService cs = Context.GetService<IAnkhCommandService>();

            cs.DirectlyExecCommand(AnkhCommand.EditConfigFile, null);
        }
    }
}
