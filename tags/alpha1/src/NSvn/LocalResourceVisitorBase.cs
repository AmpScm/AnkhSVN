// $Id$
using System;

namespace NSvn
{
	/// <summary>
	/// Convenience base class for ILocalResourceVisitor implementations, stubbing out
	/// the operations.
	/// </summary>
	public class LocalResourceVisitorBase : ILocalResourceVisitor
	{	
        #region Implementation of ILocalResourceVisitor
        public virtual void VisitUnversionedDirectory(NSvn.UnversionedDirectory dir)
        {
            this.VisitUnversionedResource( dir );
        }
        public virtual void VisitUnversionedFile(NSvn.UnversionedFile file)
        {
            this.VisitUnversionedResource( file );        
        }
        public virtual void VisitWorkingCopyDirectory(NSvn.WorkingCopyDirectory dir)
        {
            this.VisitWorkingCopyResource( dir );
        }
        public virtual void VisitWorkingCopyFile(NSvn.WorkingCopyFile file)
        {
            this.VisitWorkingCopyResource( file );
        }    
        #endregion

        public virtual void VisitWorkingCopyResource( WorkingCopyResource resource )
        {
            // empty
        }

        public virtual void VisitUnversionedResource( UnversionedResource resource )
        {
            // empty
        }
    }
}
