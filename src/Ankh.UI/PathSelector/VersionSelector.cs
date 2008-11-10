// $Id$
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using System.Text.RegularExpressions;
using SharpSvn;

namespace Ankh.UI.PathSelector
{
    /// <summary>
    /// A control that allows the user to pick a revision.
    /// </summary>
    public partial class VersionSelector : System.Windows.Forms.UserControl
    {
        public event EventHandler Changed;

        public VersionSelector()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.revisionTypeBox.Items.AddRange( new object[]{
                                                                 RevisionChoice.Head,
                                                                 RevisionChoice.Committed,
                                                                 RevisionChoice.Base,
                                                                 RevisionChoice.Previous,
                                                                 RevisionChoice.Working } );
            this.revisionTypeBox.SelectedItem = RevisionChoice.Head;
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SvnRevision Revision
        {
            get
            {
                if ( this.Valid )
                {
                    if ( this.revisionTypeBox.SelectedItem == null )
                        return new SvnRevision(
                            int.Parse( this.revisionTypeBox.Text ) );
                    else
                        return ((RevisionChoice)
                            this.revisionTypeBox.SelectedItem).Revision;
                }
                else
                    return null;
            }
            set
            {
                switch( value.RevisionType )
                {
                    case SvnRevisionType.Number:
                        this.revisionTypeBox.SelectedItem = null;
                        this.revisionTypeBox.Text = value.Revision.ToString();
                        break;
                    case SvnRevisionType.Time:
                        this.datePicker.Value = value.Time;
                        this.revisionTypeBox.SelectedItem = this.dateRevisionChoice;
                        this.datePicker.Enabled = true;
                        break;
                    case SvnRevisionType.Base:
                        this.revisionTypeBox.SelectedItem = RevisionChoice.Base;
                        break;

                    case SvnRevisionType.Working:
                        this.revisionTypeBox.SelectedItem = RevisionChoice.Working;
                        break;

                    case SvnRevisionType.Committed:
                        this.revisionTypeBox.SelectedItem = RevisionChoice.Committed;
                        break;

                    case SvnRevisionType.Head:
                        this.revisionTypeBox.SelectedItem = RevisionChoice.Head;
                        break;

                    case SvnRevisionType.Previous:
                        this.revisionTypeBox.SelectedItem = RevisionChoice.Previous;
                        break;
                    default:
                        throw new ArgumentException( "Invalid revision" );
                }
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
            public RevisionChoice( string name, SvnRevision revision )
            {
                this.name = name;
                this.revision = revision;
            }

            public override string ToString()
            {
                return this.name;
            }

            public virtual SvnRevision Revision
            {
                get{ return this.revision; }
            }

            public static readonly RevisionChoice Head = 
                new RevisionChoice( "Head", SvnRevision.Head );
            public static readonly RevisionChoice Committed =
                new RevisionChoice("Committed", SvnRevision.Committed);
            public static readonly RevisionChoice Base =
                new RevisionChoice("Base", SvnRevision.Base);
            public static readonly RevisionChoice Working =
                new RevisionChoice("Working", SvnRevision.Working);
            public static readonly RevisionChoice Previous =
                new RevisionChoice("Previous", SvnRevision.Previous);


            private SvnRevision revision;
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

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public override SvnRevision Revision
            {
                get{ return new SvnRevision(this.picker.Value ); }
            }

            private DateTimePicker picker;
        }

        private static readonly Regex NUMBER = new Regex(@"\d+");
        private RevisionChoice dateRevisionChoice;
    }
}
