// $Id$
using System;
using NSvn;
using NSvn.Common;
using NSvn.Core;
using System.IO;
using System.Collections;
using System.Text;

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

    internal class RenamableVisitor : LocalResourceVisitorBase
    {
        public bool Renamable = false;
        public override void VisitWorkingCopyFile(WorkingCopyFile file)
        {   
            if ( file.Status.TextStatus == StatusKind.Normal &&
                (file.Status.PropertyStatus == StatusKind.Normal ||
                file.Status.PropertyStatus == StatusKind.None ) )
                this.Renamable = true;
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
        public bool IsUnversioned
        {
            get
            { 
                return !this.workingCopyResourceFound && 
                    this.unversionedResourceFound; 
            }
        }   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        public override void VisitWorkingCopyResource( WorkingCopyResource r )
        {
            this.workingCopyResourceFound = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        public override void VisitUnversionedResource( UnversionedResource r )
        {
            this.unversionedResourceFound = true;
        }


        private bool workingCopyResourceFound = false;
        private bool unversionedResourceFound = false;
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

    internal class DiffVisitor : LocalResourceVisitorBase
    {
        //        public string DiffFile
        //        {
        //            get
        //            {
        //                this.stream.Close();
        //                return this.GenerateHtmlFile();
        //            }
        //        }

        public string Diff
        {
            get{ return Encoding.Default.GetString( this.stream.ToArray() ); }
        }


        public DiffVisitor()
        {
            this.stream = new MemoryStream();           
        }

        public override void VisitWorkingCopyFile(NSvn.WorkingCopyFile file)
        {
            if ( file.Status.TextStatus != StatusKind.Normal || 
                (file.Status.PropertyStatus != StatusKind.Normal && 
                file.Status.PropertyStatus != StatusKind.None ) )
            {
                file.Diff( this.stream, Stream.Null );
            }
        }

        private MemoryStream stream;
    }

    internal class ConflictedVisitor : LocalResourceVisitorBase
    {    
        public override void VisitWorkingCopyFile(WorkingCopyFile file)
        {
            if ( file.Status.TextStatus == StatusKind.Conflicted )
                conflicted++;
            else
                nonConflicted++;
        }

        public bool Conflicted
        {
            get{ return conflicted > 0 && nonConflicted == 0; }
        }

        private int conflicted = 0;
        private int nonConflicted = 0;
    }

    internal class IsRepositoryFileVisitor : IRepositoryResourceVisitor
    {
        public bool IsFile
        {
            get{ return this.fileFound && !this.dirFound; }
        }

        public void VisitFile(RepositoryFile file)
        {
            this.fileFound = true;
        }

        public void VisitDirectory(RepositoryDirectory directory)
        {
            this.dirFound = true;
        }

        private bool fileFound = false, dirFound = false;
    }

}
