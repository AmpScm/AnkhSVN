using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using NSvn.Core;
using System.Text.RegularExpressions;

namespace Ankh.UI
{

	/// <summary>
	/// A control that allows the user to pick a revision.
	/// </summary>
	public class RevisionPicker : System.Windows.Forms.UserControl
	{
        public event EventHandler Changed;

		public RevisionPicker()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

            this.revisionTypeBox.Items.AddRange( new object[]{
                 RevisionChoice.Head,
                 RevisionChoice.Committed,
                 RevisionChoice.Base,
                 RevisionChoice.Previous,
                 RevisionChoice.Working } );
            this.dateRevisionChoice = new DateRevisionChoice( this.datePicker );
            this.revisionTypeBox.Items.Add( this.dateRevisionChoice ); 
		}

        /// <summary>
        /// Whether the control has a valid revision.
        /// </summary>
        public bool Valid
        {
            get
            { 
                if ( this.revisionTypeBox.SelectedItem == null )
                    return NUMBER.IsMatch( this.revisionTypeBox.Text ); 
                else
                    return true;
            }
        }

        /// <summary>
        /// The revision selected by the user.
        /// </summary>
        public Revision Revision
        {
            get
            {
                if ( this.Valid )
                {
                    if ( this.revisionTypeBox.SelectedItem == null )
                        return Revision.FromNumber( 
                            int.Parse( this.revisionTypeBox.Text ) );
                    else
                        return ((RevisionChoice)
                            this.revisionTypeBox.SelectedItem).Revision;
                }
                else
                    return null;
            }
        }

        public bool HeadEnabled
        {
            set{ this.Toggle( RevisionChoice.Head, value ); }
        }

        public bool CommittedEnabled
        {
            set{ this.Toggle( RevisionChoice.Committed, value ); }
        }

        public bool BaseEnabled
        {
            set{ this.Toggle( RevisionChoice.Base, value ); }
        }

        public bool PreviousEnabled
        {
            set{ this.Toggle( RevisionChoice.Previous, value ); }
        }

        public bool WorkingEnabled
        {
            set{ this.Toggle( RevisionChoice.Working, value ); }
        }

        public bool DateEnabled
        {
            set{ this.Toggle( this.dateRevisionChoice, value ); }
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.revisionTypeBox = new System.Windows.Forms.ComboBox();
            this.datePicker = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // revisionTypeBox
            // 
            this.revisionTypeBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right);
            this.revisionTypeBox.Location = new System.Drawing.Point(8, 0);
            this.revisionTypeBox.Name = "revisionTypeBox";
            this.revisionTypeBox.Size = new System.Drawing.Size(121, 21);
            this.revisionTypeBox.TabIndex = 0;
            this.revisionTypeBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.revisionTypeBox_KeyUp);
            this.revisionTypeBox.SelectionChangeCommitted += new System.EventHandler(this.revisionTypeBox_SelectionChangeCommitted);
            // 
            // datePicker
            // 
            this.datePicker.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.datePicker.Enabled = false;
            this.datePicker.Location = new System.Drawing.Point(136, 0);
            this.datePicker.Name = "datePicker";
            this.datePicker.TabIndex = 1;
            // 
            // RevisionPicker
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.datePicker,
                                                                          this.revisionTypeBox});
            this.Name = "RevisionPicker";
            this.Size = new System.Drawing.Size(344, 24);
            this.ResumeLayout(false);

        }
		#endregion

        /// <summary>
        /// Adds or removes a choice from the combo box.
        /// </summary>
        /// <param name="choice"></param>
        /// <param name="enabled"></param>
        private void Toggle( RevisionChoice choice, bool enabled )
        {
            if ( enabled && !this.revisionTypeBox.Items.Contains( choice ) )
                this.revisionTypeBox.Items.Add( choice );
            else if ( !enabled && this.revisionTypeBox.Items.Contains( choice ) )
                this.revisionTypeBox.Items.Remove( choice );
        }

        private void revisionTypeBox_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            this.OnChanged( EventArgs.Empty );
        }

        private void revisionTypeBox_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            this.OnChanged( EventArgs.Empty );
        }

        protected virtual void OnChanged( EventArgs args )
        {
            this.datePicker.Enabled = 
                this.revisionTypeBox.SelectedItem == this.dateRevisionChoice;

            if ( this.Changed != null )
                this.Changed( this, args );
        }


        /// <summary>
        /// Represents a revision type.
        /// </summary>
        private class RevisionChoice
        {
            public RevisionChoice( string name, Revision revision )
            {
                this.name = name;
                this.revision = revision;
            }

            public override string ToString()
            {
                return this.name;
            }

            public virtual Revision Revision
            {
                get{ return this.revision; }
            }

            public static readonly RevisionChoice Head = 
                new RevisionChoice( "Head", Revision.Head );
            public static readonly RevisionChoice Committed = 
                new RevisionChoice( "Committed", Revision.Committed );
            public static readonly RevisionChoice Base = 
                new RevisionChoice( "Base", Revision.Base );
            public static readonly RevisionChoice Working = 
                new RevisionChoice( "Working", Revision.Working );
            public static readonly RevisionChoice Previous = 
                new RevisionChoice( "Previous", Revision.Previous );


            private Revision revision;
            private string name;
        }

        /// <summary>
        /// Represents a date revision type.
        /// </summary>
        private class DateRevisionChoice : RevisionChoice
        {
            public DateRevisionChoice( DateTimePicker picker ) :
                base( "Date", null )
            {
                this.picker = picker;
            }

            public override Revision Revision
            {
                get{ return Revision.FromDate( this.picker.Value ); }
            }

            private DateTimePicker picker;
        }

        private static readonly Regex NUMBER = new Regex(@"\d+");
        private RevisionChoice dateRevisionChoice;
        private System.Windows.Forms.ComboBox revisionTypeBox;
        private System.Windows.Forms.DateTimePicker datePicker;
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        

        
	}
}
