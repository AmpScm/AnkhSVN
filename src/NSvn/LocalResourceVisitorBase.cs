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
        public void VisitUnversionedDirectory(NSvn.UnversionedDirectory dir)
        {
            this.VisitUnversionedResource( dir );
        }
        public void VisitUnversionedFile(NSvn.UnversionedFile file)
        {
            this.VisitUnversionedResource( file );        
        }
        public void VisitWorkingCopyDirectory(NSvn.WorkingCopyDirectory dir)
        {
            this.VisitWorkingCopyResource( dir );
        }
        public void VisitWorkingCopyFile(NSvn.WorkingCopyFile file)
        {
            this.VisitWorkingCopyResource( file );
        }    
        #endregion

        public void VisitWorkingCopyResource( WorkingCopyResource resource )
        {
            // empty
        }

        public void VisitUnversionedResource( UnversionedResource resource )
        {
            // empty
        }
    }
}
