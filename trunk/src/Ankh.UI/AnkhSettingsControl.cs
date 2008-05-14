﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ankh.Commands;
using Ankh.Ids;
using Microsoft.VisualStudio.Shell.Interop;
using System.Windows.Forms.Design;
using Ankh.UI.UITypeEditors;
using Ankh.Configuration;

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

		AnkhConfig _config;
		AnkhConfig Config
		{
			get
			{
				if (_config == null)
				{
					IAnkhConfigurationService configurationSvc = Context.GetService<IAnkhConfigurationService>();
					_config = configurationSvc.Instance;
				}
				return _config;
			}
		}

		public void LoadSettings()
		{
			txtDiffExePath.Text = Config.DiffExePath;
			txtMergeExePath.Text = Config.MergeExePath;
			cbDiffMergeManual.Checked = Config.ChooseDiffMergeManual;
		}


		public void SaveSettings()
		{
			Config.DiffExePath = txtDiffExePath.Text;
			Config.MergeExePath = txtMergeExePath.Text;
			Config.ChooseDiffMergeManual = cbDiffMergeManual.Checked;
		}

		private void btnDiffExePath_Click(object sender, EventArgs e)
		{
			DiffExeTypeEditor typeEditor = new DiffExeTypeEditor();
			txtDiffExePath.Text = (string)typeEditor.EditValue(Context, Config.DiffExePath);
		}

		private void btnMergePath_Click(object sender, EventArgs e)
		{
			MergeExeTypeEditor typeEditor = new MergeExeTypeEditor();
			txtMergeExePath.Text = (string)typeEditor.EditValue(Context, Config.MergeExePath);

		}
    }
}
