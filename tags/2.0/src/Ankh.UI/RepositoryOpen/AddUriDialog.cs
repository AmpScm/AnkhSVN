using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.RepositoryOpen
{
	public partial class AddUriDialog : Form
	{
		public AddUriDialog()
		{
			InitializeComponent();
		}

		internal string UrlText
		{
			get { return urlBox.Text; }
		}
	}
}
