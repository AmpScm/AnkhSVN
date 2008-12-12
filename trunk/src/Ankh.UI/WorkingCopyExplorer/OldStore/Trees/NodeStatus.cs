// $Id$
//
// Copyright 2006-2008 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Text;

namespace Ankh
{
    /// <summary>
    /// Describes the status of a tree node.
    /// </summary>
    public struct NodeStatus
    {
        public readonly NodeStatusKind Kind;
        public readonly bool ReadOnly;
        public readonly bool Locked;

        public NodeStatus(NodeStatusKind kind, bool readOnly, bool locked)
        {
            this.Kind = kind;
            this.ReadOnly = readOnly;
            this.Locked = locked;
        }

        public static bool operator ==(NodeStatus s1, NodeStatus s2)
        {
            return s1.Kind == s2.Kind &&
                    s1.Locked == s2.Locked &&
                    s1.ReadOnly == s2.ReadOnly;
        }

        public static bool operator !=(NodeStatus s1, NodeStatus s2)
        {
            return !(s1 == s2);
        }

        public override bool Equals(object obj)
        {
            return this == (NodeStatus)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public NodeStatus Merge(NodeStatus status)
        {
            NodeStatusKind newKind = this.Kind;
            bool readOnly = this.ReadOnly;
            bool locked = this.Locked;

            if (status.Kind == 0)
            {
                // nothing
            }
            // these are always overridden
            else if (this.Kind == NodeStatusKind.None || this.Kind == NodeStatusKind.Unversioned
                || this.Kind == 0)
            {
                newKind = status.Kind;
            }
            else if (status.Kind != NodeStatusKind.None &&
                status.Kind != NodeStatusKind.Ignored &&
                status.Kind != 0)
            {
                if (this.Kind == NodeStatusKind.Normal)
                {
                    newKind = status.Kind;
                }
                else if (status.Kind != NodeStatusKind.Normal &&
                    this.Kind != status.Kind)
                {
                    newKind = NodeStatusKind.IndividualStatusesConflicting;
                }
            }

            if (status.ReadOnly)
                readOnly = true;

            if (status.Locked)
                locked = true;

            return new NodeStatus(newKind, readOnly, locked);
        }


        public static readonly NodeStatus None = new NodeStatus(NodeStatusKind.None, false, false);
    }
}
