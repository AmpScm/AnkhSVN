// $Id$
using System;
using System.Collections;
using System.IO;
using NSvn.Core;
using Utils;
using EnvDTE;

namespace Ankh
{
    /// <summary>
    /// Arguments for the ProjectFileModifiedDelegate
    /// </summary>
    public class FileModifiedEventArgs
    {
        public FileModifiedEventArgs( string file )
        {
            this.filename = file;
        }
        public string Filename
        {
            get{ return this.filename; }
        }

        private string filename;
    }

    public delegate void FileModifiedDelegate( object sender, 
        FileModifiedEventArgs args );

    /// <summary>
    /// Watches project files.
    /// </summary>
    internal class FileWatcher
    {
        /// <summary>
        /// A project file is modified.
        /// </summary>
        public event FileModifiedDelegate FileModified;

        public FileWatcher( Client client )
        {
            client.Notification += new NotificationDelegate(this.OnNotification);
            this.projectWatchers = new ArrayList();
        }

        public void StartWatchingForChanges()
        {
            this.dirty = false;
        }

        /// <summary>
        /// Whether any projects have been modified since the last time 
        /// the watcher was reset.
        /// </summary>
        public bool HasDirtyFiles
        {
            get{ return this.dirty; }
        }

        public void AddFile( string path )
        {
            Watcher w = new Watcher( path, this );
            this.projectWatchers.Add( w );
            w.Run();
        }

        /// <summary>
        /// Clears the watchee list and performs a reset.
        /// </summary>
        public void Clear()
        {
            foreach( IDisposable d in this.projectWatchers )
                d.Dispose();
            this.projectWatchers.Clear();
            this.dirty = false;
        }

        /// <summary>
        /// Resets the object
        /// </summary>
        public void Reset()
        {
            this.dirty = false;
        }

        private void OnNotification( object sender, NotificationEventArgs args )
        {
            if ( IsChanged( args.ContentState ) || args.Action == NotifyAction.Revert )
            {
                if ( this.IsWatchee( args.Path ) )
                    this.dirty = true;
            }
        }

        private static bool IsChanged( NotifyState state )
        {
            return  state == NotifyState.Changed || 
                state == NotifyState.Conflicted ||
                state == NotifyState.Merged;
        }

        private bool IsWatchee( string path )
        {
            foreach( Watcher w in this.projectWatchers )
            {
                if ( PathUtils.NormalizePath(w.FilePath) == PathUtils.NormalizePath(path) )
                    return true;
            }
            return false;
        }

        #region class Watcher
        private class Watcher : IDisposable
        {
            public Watcher( string path, FileWatcher parent )
            {
                this.parent = parent;
                this.path = path;

                string dir = Path.GetDirectoryName( path );
                string file = Path.GetFileName( path );

                this.fileSystemWatcher = new FileSystemWatcher( dir, file ); 
                this.fileSystemWatcher.IncludeSubdirectories = false;
                this.fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite |
                    NotifyFilters.FileName;
                this.fileSystemWatcher.Changed += new 
                    FileSystemEventHandler(this.OnChanged);
                //                this.fileSystemWatcher.Renamed += 
                //                    new RenamedEventHandler(parent.OnRenamed);
            }

            ~Watcher()
            {
                this.Dispose();
            }

            public string FilePath
            {
                get{ return this.path; }
            }

            public void Stop()
            {
                this.fileSystemWatcher.EnableRaisingEvents = false;
            }

            public void Run()
            {
                this.fileSystemWatcher.EnableRaisingEvents = true;
            }
            
            #region IDisposable Members
            public void Dispose()
            {
                this.fileSystemWatcher.Dispose();
                GC.SuppressFinalize(this);
            }
            #endregion


            private void OnChanged( object sender, FileSystemEventArgs args )
            {
                // nope, we are watching a project file
                if ( this.parent.FileModified != null )
                {
                    FileModifiedEventArgs projArgs = new 
                        FileModifiedEventArgs(this.path);
                    this.parent.FileModified( this.parent, projArgs );
                }
            }

            private FileWatcher parent;
            private FileSystemWatcher fileSystemWatcher;
            private string path;
            
        }
        #endregion

        private ArrayList projectWatchers;
        private bool dirty;
    }
}
