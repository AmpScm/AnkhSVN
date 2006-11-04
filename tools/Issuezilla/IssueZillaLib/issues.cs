using System;
using System.Collections.Generic;
using System.Text;

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

    }
}
