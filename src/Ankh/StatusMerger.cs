using System;
using System.Text;

namespace Ankh
{
    /// <summary>
    /// Used for merging several NodeStatuses into a single NodeStatus.
    /// </summary>
    public struct StatusMerger
    {
        public void NewStatus( NodeStatus status )
        {
            if ( status.Kind == 0 )
            {
                // nothing
            }
            // these are always overridden
            else if ( this.currentStatus.Kind == NodeStatusKind.None || this.currentStatus.Kind == NodeStatusKind.Unversioned
                || this.currentStatus.Kind == 0 )
            {
                this.currentStatus.Kind = status.Kind;
            }
            else if ( status.Kind != NodeStatusKind.None &&
                status.Kind != NodeStatusKind.Ignored &&
                status.Kind != 0 )
            {
                if ( this.currentStatus.Kind == NodeStatusKind.Normal )
                {
                    this.currentStatus.Kind = status.Kind;
                }
                else if ( status.Kind != NodeStatusKind.Normal &&
                    this.currentStatus.Kind != status.Kind )
                {
                    this.currentStatus.Kind = NodeStatusKind.IndividualStatusesConflicting;
                }
            }

            if ( status.ReadOnly )
                this.currentStatus.ReadOnly = true;

            if ( status.Locked )
                this.currentStatus.Locked = true;

        }

        public NodeStatus CurrentStatus
        {
            get
            {
                return this.currentStatus;
            }
            set { this.currentStatus = value; }
        }
        private NodeStatus currentStatus;
    }

        
}
