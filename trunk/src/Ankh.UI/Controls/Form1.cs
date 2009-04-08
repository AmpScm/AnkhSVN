using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.Controls
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		public static void TestForm()
		{
			using (Form1 f = new Form1())
			{
				f.ShowDialog();
			}
		}

		private void statusContainer_Click(object sender, EventArgs e)
		{

		}
	}
}
