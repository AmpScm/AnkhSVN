using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Reflection;
using System.Collections;

namespace Fines.IssueZillaLib
{
    public partial class long_desc
    {
        public string formatted_desc
        {
            get
            {
                return String.Join( Environment.NewLine, this.thetext.Split( '\n' ) );
            }
        }
    }

    public partial class issue
    {
        private IssueState state = IssueState.New;

        public IssueState State
        {
            get { return this.state; }
            set { this.state = value; }
        }

        private string knobField;

        /// <remarks/>
        public string knob
        {
            get
            {
                return this.knobField;
            }
            set
            {
                if ( ( this.knobField != value ) )
                {
                    this.knobField = value;
                    this.RaisePropertyChanged( "knob" );
                }
            }
        }


        public bool ContainsText( string searchText )
        {
            if ( ContainsText(this.short_desc, searchText ))
            {
                return true;
            }

            if ( ContainsText(this.reporter, searchText ))
            {
                return true;
            }

            if ( ContainsText( this.target_milestone, searchText ) ) 
            {
                return true;
            }

            if ( ContainsText(this.issue_id.ToString(), searchText) )
            {
                return true;
            }

            foreach(long_desc desc in this.long_desc)
            {
                if ( ContainsText( desc.who, searchText ) || ContainsText( desc.thetext, searchText ) )
                {
                    return true;
                }
            }
            return false;
        }

        private static bool ContainsText( string text, string textToSearchFor )
        {
            return text.IndexOf( textToSearchFor, StringComparison.OrdinalIgnoreCase ) >= 0;
        }


        public NameValueCollection GetNameValues()
        {
            PropertyInfo[] properties = this.GetType().GetProperties( BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance );
            NameValueCollection collection = new NameValueCollection();
            foreach ( PropertyInfo prop in properties )
            {
                object value = prop.GetValue( this, null );
                if ( value != null && !(value is ICollection) )
                {
                    collection.Add( prop.Name, value.ToString() );
                }
            }

            return collection;
        }
    }
}
