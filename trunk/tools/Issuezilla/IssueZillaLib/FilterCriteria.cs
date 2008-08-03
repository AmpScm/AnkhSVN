using System;
using System.Collections.Generic;
using System.Text;

namespace Fines.IssueZillaLib
{
    public class FilterCriteria
    {
        public FilterCriteria( IList<issue> issues )
        {
            this.issues = issues;
            this.BuildCollections();
        }

        public IList<string> AvailableStatuses
        {
            get { return this.availableStatuses; }
        }

        private void BuildCollections()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach ( issue i in issues )
            {
                if ( !String.IsNullOrEmpty(i.issue_status) )
                {
                    dict[ i.issue_status ] = i.issue_status; 
                }
            }

            this.availableStatuses = new List<string>();
            this.availableStatuses.Add( String.Empty );

            this.availableStatuses.AddRange(dict.Keys);
        }

        private List<string> availableStatuses;
        private IList<issue> issues;
    }
}
