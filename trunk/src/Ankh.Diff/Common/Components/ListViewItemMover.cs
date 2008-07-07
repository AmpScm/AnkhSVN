#region Copyright And Revision History

/*---------------------------------------------------------------------------

	Copyright © 2003 Bill Menees.  All rights reserved.
	Bill@Menees.com

	$History: ListViewItemMover.cs $
	
	*****************  Version 2  *****************
	User: Bill         Date: 11/06/05   Time: 12:58p
	Updated in $/CSharp/Menees/Components
	Moving the item now also ensures that it is visible.

-----------------------------------------------------------------------------*/

#endregion

#region Using Directives

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

#endregion

namespace Ankh.Diff
{
	/// <summary>
	/// Used to interactively moves list view items up and down.
	/// </summary>
	public class ListViewItemMover : System.Windows.Forms.UserControl
	{
		#region Private Data Members

        private Ankh.Diff.NonSelectButton btnUp;
        private Ankh.Diff.NonSelectButton btnDown;
		private System.ComponentModel.IContainer components;
		private ListView m_ListView;
		private EventHandler m_SelectedIndexChangedEventHandler;

		#endregion

		#region Constructor and Dispose

		public ListViewItemMover()
		{
			components = null;
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.btnUp = new Ankh.Diff.NonSelectButton();
            this.btnDown = new Ankh.Diff.NonSelectButton();
			this.SuspendLayout();
			// 
			// btnUp
			// 
			this.btnUp.Dock = System.Windows.Forms.DockStyle.Top;
			this.btnUp.Font = new System.Drawing.Font("Symbol", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
			this.btnUp.Location = new System.Drawing.Point(0, 0);
			this.btnUp.Name = "btnUp";
			this.btnUp.Size = new System.Drawing.Size(24, 23);
			this.btnUp.TabIndex = 0;
			this.btnUp.Text = "­";
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			// 
			// btnDown
			// 
			this.btnDown.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.btnDown.Font = new System.Drawing.Font("Symbol", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
			this.btnDown.Location = new System.Drawing.Point(0, 45);
			this.btnDown.Name = "btnDown";
			this.btnDown.Size = new System.Drawing.Size(24, 23);
			this.btnDown.TabIndex = 1;
			this.btnDown.Text = "¯";
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			// 
			// ListViewItemMover
			// 
			this.Controls.Add(this.btnDown);
			this.Controls.Add(this.btnUp);
			this.Name = "ListViewItemMover";
			this.Size = new System.Drawing.Size(24, 68);
			this.Load += new System.EventHandler(this.ListViewItemMover_Load);
			this.ResumeLayout(false);

		}
		#endregion

		#region Public Properties

		[DefaultValue(null)]
		public ListView ListView
		{
			get
			{
				return m_ListView;
			}
			set
			{
				if (m_ListView != value)
				{
					if (m_ListView != null)
					{
						m_ListView.SelectedIndexChanged -= m_SelectedIndexChangedEventHandler;
					}

					m_ListView = value;

					if (m_ListView != null)
					{
						if (m_SelectedIndexChangedEventHandler == null)
						{
							m_SelectedIndexChangedEventHandler = new EventHandler(ListSelectedIndexChanged);
						}

						m_ListView.SelectedIndexChanged += m_SelectedIndexChangedEventHandler;
					}
				}
			}
		}

		#endregion

		#region Public Methods

		public void UpdateControlStates()
		{
			ListViewItem Item = SelectedItem;
			btnUp.Enabled = Item != null && Item.Index > 0;
			btnDown.Enabled = Item != null && Item.Index < (m_ListView.Items.Count-1);
		}

		#endregion

		#region Public Events

		public event EventHandler ItemMovedUp;
		public event EventHandler ItemMovedDown;

		#endregion

		#region Private Methods

		private void ListSelectedIndexChanged(object sender, System.EventArgs e)
		{
			UpdateControlStates();
		}

		private ListViewItem SelectedItem
		{
			get
			{
				//Only work with single selection.  Moving multiple non-contiguous
				//items is too tricky to mess with since it is so rarely needed.
				if (m_ListView != null && m_ListView.SelectedItems.Count == 1)
				{
					return m_ListView.SelectedItems[0];
				}
				else
				{
					return null;
				}
			}
		}

		private void btnUp_Click(object sender, System.EventArgs e)
		{
			ListViewItem Item = SelectedItem;
			if (Item != null && Item.Index > 0)
			{
				MoveItem(Item, -1);
				if (ItemMovedUp != null)
				{
					ItemMovedUp(this, e);
				}
			}
		}

		private void btnDown_Click(object sender, System.EventArgs e)
		{
			ListViewItem Item = SelectedItem;
			if (Item != null && Item.Index < (m_ListView.Items.Count-1))
			{
				MoveItem(Item, +1);
				if (ItemMovedDown != null)
				{
					ItemMovedDown(this, e);
				}
			}		
		}

		private void MoveItem(ListViewItem Item, int iIndexOffset)
		{
			int iNewIndex = Item.Index + iIndexOffset;
			Item.Remove();
			m_ListView.Items.Insert(iNewIndex, Item);
			Item.Selected = true;
			m_ListView.ArrangeIcons();
			Item.EnsureVisible();
		}

		private void ListViewItemMover_Load(object sender, System.EventArgs e)
		{
			UpdateControlStates();
		}

		#endregion
	}
}
