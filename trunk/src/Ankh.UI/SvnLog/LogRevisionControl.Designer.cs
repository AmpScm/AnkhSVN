namespace Ankh.UI.SvnLog
{
    partial class LogRevisionControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.logRevisionControl1 = new Ankh.UI.SvnLog.LogRevisionView(this.components);
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // logRevisionControl1
            // 
            this.logRevisionControl1.AllowColumnReorder = true;
            this.logRevisionControl1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.logRevisionControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logRevisionControl1.FullRowSelect = true;
            this.logRevisionControl1.Location = new System.Drawing.Point(0, 0);
            this.logRevisionControl1.Name = "logRevisionControl1";
            this.logRevisionControl1.ProvideWholeListForSelection = false;
            this.logRevisionControl1.Size = new System.Drawing.Size(552, 324);
            this.logRevisionControl1.TabIndex = 0;
            this.logRevisionControl1.UseCompatibleStateImageBehavior = false;
            this.logRevisionControl1.View = System.Windows.Forms.View.Details;
            this.logRevisionControl1.VirtualMode = true;
            this.logRevisionControl1.ShowContextMenu += new System.EventHandler(this.logRevisionControl1_ShowContextMenu);
            this.logRevisionControl1.SelectedIndexChanged += new System.EventHandler(this.logRevisionControl1_SelectedIndexChanged);
            this.logRevisionControl1.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.logRevisionControl1_RetrieveVirtualItem);
            this.logRevisionControl1.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.logRevisionControl1_ItemSelectionChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Revision";
            this.columnHeader1.Width = 75;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Author";
            this.columnHeader2.Width = 73;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Date";
            this.columnHeader3.Width = 118;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Message";
            this.columnHeader4.Width = 300;
            // 
            // LogRevisionControlNew
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.logRevisionControl1);
            this.Name = "LogRevisionControlNew";
            this.Size = new System.Drawing.Size(552, 324);
            this.ResumeLayout(false);

        }

        #endregion

        private LogRevisionView logRevisionControl1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;

    }
}
