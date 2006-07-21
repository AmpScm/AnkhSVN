using System;
using System.Text;

namespace Ankh
{
    /// <summary>
    /// Describes the status of a tree node.
    /// </summary>
    public struct NodeStatus
    {
        public NodeStatusKind Kind;
        public bool ReadOnly;
        public bool Locked;

        public NodeStatus( NodeStatusKind kind, bool readOnly, bool locked )
        {
            this.Kind = kind;
            this.ReadOnly = readOnly;
            this.Locked = locked;
        }

        public static bool operator ==( NodeStatus s1, NodeStatus s2 )
        {
            return s1.Kind == s2.Kind &&
                    s1.Locked == s2.Locked &&
                    s1.ReadOnly == s2.ReadOnly;
        }

        public static bool operator !=( NodeStatus s1, NodeStatus s2 )
        {
            return !( s1 == s2 );
        }

        public override bool Equals( object obj )
        {
            return this == (NodeStatus)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }



        public static readonly NodeStatus None = new NodeStatus( NodeStatusKind.None, false, false );
    }
}
