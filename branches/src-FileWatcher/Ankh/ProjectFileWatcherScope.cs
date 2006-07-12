using System;
using System.Text;
using System.Globalization;

namespace Ankh
{
    /// <summary>
    /// A class that can be used to monitor for changes to project files in a given scope.
    /// </summary>
    public class ProjectFileWatcherScope : IDisposable
    {
        public ProjectFileWatcherScope( IContext context )
        {
            this.context = context;
            this.context.FileWatcher.FileModified += new FileModifiedDelegate( HandleFileModified );
        }

        public void Dispose()
        {
            if ( !this.abandoned && this.projectFilesTouched )
            {
                //this.context.ReloadSolutionIfNecessary();
            }

            // important, otherwise this instance would stay alive forever.
            this.context.FileWatcher.FileModified -= new FileModifiedDelegate( HandleFileModified );
        }

        public void Abandon()
        {
            this.abandoned = true;
        }

        private void HandleFileModified( object sender, FileModifiedEventArgs args )
        {
            // TODO: Perhaps replace with more robust detection of project files
            if ( args.Filename.EndsWith( "proj", true, CultureInfo.InvariantCulture ) )
            {
                projectFilesTouched = true;
            }
        }

        private bool abandoned;
        private bool projectFilesTouched;
        private IContext context;
    }
}
