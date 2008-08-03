using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Fines.IssueZillaLib;

namespace IssueZilla
{
    public partial class FilterControl : UserControl
    {
        public event EventHandler FilterChanged;
        public event EventHandler SearchTextChanged;

        public FilterControl()
        {
            InitializeComponent();

            this.searchFont = this.searchTextBox.Font;
            this.regularFont = new Font( this.searchFont, FontStyle.Regular );

            this.filter = new Filter();
            this.filterBindingSource.DataSource = this.filter;

            this.statusComboBox.SelectedIndexChanged += new EventHandler( statusComboBox_SelectedIndexChanged );
        }

       

        public void SetFilterCriteria( FilterCriteria fc )
        {
            this.statusComboBox.DataSource = fc.AvailableStatuses;
        }

        public Filter Filter
        {
            get { return this.filter; }
        }

        public string SearchText
        {
            get { return this.searchTextBox.Text; }
        }

        void statusComboBox_SelectedIndexChanged( object sender, EventArgs e )
        {
            this.statusComboBox.DataBindings[ "SelectedItem" ].WriteValue();
            if ( this.FilterChanged != null )
            {
                this.FilterChanged( this, EventArgs.Empty );
            }
        }

        private void searchTextBox_Leave( object sender, EventArgs e )
        {
            if ( searchTextBox.Text == String.Empty )
            {
                this.isSearching = false;
                this.searchTextBox.Text = SearchTextDisplay;
                this.searchTextBox.Font = searchFont;
            }
        }

        private void searchTextBox_Enter( object sender, EventArgs e )
        {
            if ( !this.isSearching )
            {
                this.searchTextBox.Font = this.regularFont;
                this.searchTextBox.Text = String.Empty;
                this.isSearching = true;
            }
            else
            {
                this.searchTextBox.SelectAll();
            }
        }

        private void searchTextBox_TextChanged( object sender, EventArgs e )
        {
            if ( this.isSearching && this.SearchTextChanged != null )
            {
                this.SearchTextChanged( this, EventArgs.Empty );
            }
        }

        internal void FocusSearchTextBox()
        {
            this.searchTextBox.Select();
        }

        private Filter filter;
        private Font searchFont;
        private Font regularFont;
        private bool isSearching;
        private const string SearchTextDisplay = "Search";







        
    }
}
