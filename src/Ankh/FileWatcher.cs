// $Id$
using System;
using System.Collections;
using System.IO;
using NSvn.Core;
using Utils;

namespace Ankh
{
    /// <summary>
    /// Watches project files.
    /// </summary>
    public class FileWatcher
    {
        public FileWatcher( Client client )
        {
            client.Notification += new NotificationDelegate(this.OnNotification);
            this.watchees = new ArrayList();
        }

        /// <summary>
        /// Whether any projects have been modified since the last time 
        /// the watcher was reset.
        /// </summary>
        public bool HasDirtyProjects
        {
            get{ return this.dirty; }
        }

        /// <summary>
        /// Add a project file to the watcher list.
        /// </summary>
        /// <param name="path"></param>
        public void AddFile( string path )
        {           
            this.watchees.Add( PathUtils.NormalizePath(path) );
        }

        /// <summary>
        /// Clears the watchee list and performs a reset.
        /// </summary>
        public void Clear()
        {
            this.watchees.Clear();
            this.dirty = true;
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
            string normPath = PathUtils.NormalizePath(path);
            return this.watchees.Contains( normPath );
        }

        private ArrayList watchees;
        private bool dirty;
    }
}
