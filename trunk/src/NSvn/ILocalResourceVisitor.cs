// $Id$
using System;

namespace NSvn
{
    /// <summary>
    /// A visitor for visiting local resources.
    /// </summary>
    public interface ILocalResourceVisitor
    {
        void VisitUnversionedDirectory( UnversionedDirectory dir );

        void VisitUnversionedFile( UnversionedFile file );

        void VisitWorkingCopyDirectory( WorkingCopyDirectory dir );

        void VisitWorkingCopyFile( WorkingCopyFile file );
    }
}
