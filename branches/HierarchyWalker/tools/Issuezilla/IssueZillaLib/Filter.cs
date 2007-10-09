using System;
using System.Collections.Generic;
using System.Text;
using Fines.Utils.Collections;

namespace Fines.IssueZillaLib
{
    public class Filter
    {
        public string SelectedStatus
        {
            get { return this.selectedStatus; }
            set { this.selectedStatus = value; }
        }

        public IList<issue> ApplyFilter( IList<issue> input )
        {
            if ( !String.IsNullOrEmpty(this.selectedStatus ))
            {
                return ListUtils.Filter(input, delegate(issue i)
                {
                    return i.issue_status.Equals(this.selectedStatus, StringComparison.OrdinalIgnoreCase);
                });
            }
            else
            {
                return input;
            }
        }

        private string selectedStatus;
    }
}
