// $Id$
using System;

namespace Ankh.EventSinks
{
    /// <summary>
    /// Handles changes to the project and solution files.
    /// </summary>
    internal class ProjectFilesEventSink : EventSink
    {
        public ProjectFilesEventSink( AnkhContext context ) : base( context )
        {
            this.Context.ProjectFileWatcher.FileModified += 
                new FileModifiedDelegate(this.ProjectFileModified);
        }

        public override void Unhook()
        {
            this.Context.ProjectFileWatcher.FileModified -= 
                new FileModifiedDelegate(this.ProjectFileModified);
        }


        private void ProjectFileModified(object sender, FileModifiedEventArgs args)
        {
            SvnItem item = this.Context.StatusCache[ args.Filename ];
            if ( item != null )
                item.Refresh(this.Context.Client);
        }
    }
}
