using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.Controls
{
	public partial class StatusPanelTestForm : Form
	{
		public StatusPanelTestForm()
		{
			InitializeComponent();
		}

		public static void TestForm()
		{
			using (StatusPanelTestForm f = new StatusPanelTestForm())
			{
				f.ShowDialog();
			}
		}
	}
}
