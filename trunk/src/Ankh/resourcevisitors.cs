using System;
using NSvn;
using NSvn.Common;
using NSvn.Core;
using System.Collections;

namespace Ankh
{
    /// <summary>
    /// A visitor that checks if the visitees are modified.
    /// </summary>
    internal class ModifiedVisitor : LocalResourceVisitorBase
    {
        public bool Modified = false;
        public override void VisitWorkingCopyResource(NSvn.WorkingCopyResource resource)
        {
            if ( resource.Status.TextStatus != StatusKind.Normal || 
                (resource.Status.PropertyStatus != StatusKind.Normal && 
                resource.Status.PropertyStatus != StatusKind.None ) )
                this.Modified = true;
        }
    }

    /// <summary>
    /// A visitor that checks if all the visitees are versioned.
    /// </summary>
    internal class VersionedVisitor : LocalResourceVisitorBase
    {
        public bool IsVersioned = true;
        public override void VisitUnversionedResource( UnversionedResource r )
        {
            this.IsVersioned = false;
        }
    }

    /// <summary>
    /// A visitor that checks if all the visitees are unversioned.
    /// </summary>
    internal class UnversionedVisitor : LocalResourceVisitorBase
    {
        public bool IsVersioned = false;
        public override void VisitWorkingCopyResource( WorkingCopyResource r )
        {
            this.IsVersioned = true;
        }
    }

    internal class ResourceGathererVisitor : LocalResourceVisitorBase
    {
        public ArrayList WorkingCopyResources = new ArrayList();
        public ArrayList UnversionedResources = new ArrayList();

        public override void VisitUnversionedResource(NSvn.UnversionedResource resource)
        {
            this.UnversionedResources.Add( resource );
        }

        public override void VisitWorkingCopyResource(NSvn.WorkingCopyResource resource)
        {
            this.WorkingCopyResources.Add( resource );            
        }
    }

    /// <summary>
    /// A visitor that adds visited item to the Working copy.
    /// </summary>
    internal class AddVisitor : LocalResourceVisitorBase
    {
        public override void VisitUnversionedResource(NSvn.UnversionedResource resource)
        {
            resource.Add( true );
        }
    }
}
