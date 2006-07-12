// $Id$
using System;
using System.Collections;
using System.IO;
using NSvn.Core;
using Utils;
using EnvDTE;
using System.Threading;

using Timer = System.Threading.Timer;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using System.Diagnostics;

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
    /// Watches files.
    /// </summary>
    public class FileWatcher : IFileWatcher
    {
        /// <summary>
        /// A project file is modified.
        /// </summary>
        public event FileModifiedDelegate FileModified;

        public FileWatcher( IContext context )
        {
            context.Client.Notification += new NotificationDelegate(this.OnNotification);
            this.cookieList = new ArrayList();

            this.AdviseFileChangeEvents( context.ServiceProvider );
        }

        private void AdviseFileChangeEvents( IOleServiceProvider provider )
        {
            // Get the VS file change tracking service.
            Guid serviceGuid = typeof(SVsFileChangeEx).GUID;
            Guid interfaceGuid = typeof(IVsFileChangeEx).GUID;

            IntPtr ptr;
            provider.QueryService( ref serviceGuid, ref interfaceGuid, out ptr );

            this.fileChangeService = (IVsFileChangeEx) Marshal.GetObjectForIUnknown( ptr );

            // this is our event sink
            this.fileChangeEvents = new FileChangeEvents( this );
        }

        /// <summary>
        /// Add a file to be watched.
        /// </summary>
        /// <param name="path"></param>
        public void AddFile( string path )
        {
            uint cookie;
            this.fileChangeService.AdviseFileChange( path,
                (uint)( _VSFILECHANGEFLAGS.VSFILECHG_Size | _VSFILECHANGEFLAGS.VSFILECHG_Time | _VSFILECHANGEFLAGS.VSFILECHG_Del ),
                this.fileChangeEvents, out cookie );

            // need to keep track of this so we can unsubscribe later
            this.cookieList.Add( cookie );
        }

        /// <summary>
        /// Clears the watchee list and performs a reset.
        /// </summary>
        public void Clear()
        {
            Debug.WriteLine( "Clearing watched files, # of cookies is " + this.cookieList.Count, "Ankh" );

            foreach ( uint cookie in this.cookieList )
            {
                this.fileChangeService.UnadviseFileChange( cookie );
            }
        }

        private void RaiseChangedEvent( string file )
        {
            if ( this.FileModified != null )
            {
                this.FileModified( this, new FileModifiedEventArgs(file) );
            }
        }

        /// <summary>
        /// Callback for SVN notifications.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnNotification( object sender, NotificationEventArgs args )
        {
            if ( IsChanged( args.ContentState ) || args.Action == NotifyAction.Revert )
            {
                RaiseChangedEvent( args.Path );
            }
        }



        /// <summary>
        /// Whether a NotifyState indicates a change to an item.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private static bool IsChanged( NotifyState state )
        {
            return  state == NotifyState.Changed || 
                state == NotifyState.Conflicted ||
                state == NotifyState.Merged;
        }

        private class FileChangeEvents : IVsFileChangeEvents
        {
            public FileChangeEvents( FileWatcher parent )
            {
                this.parent = parent;
            }

            public int DirectoryChanged( string pszDirectory )
            {
                return VSConstants.S_OK;
            }

            public int FilesChanged( uint cChanges, string[] rgpszFile, uint[] rggrfChange )
            {
                foreach ( string file in rgpszFile )
                {
                    this.parent.RaiseChangedEvent( file );
                }

                return VSConstants.S_OK;
            }

            private FileWatcher parent;
        }
       
        private ArrayList cookieList;
        private IVsFileChangeEx fileChangeService;
        private IVsFileChangeEvents fileChangeEvents;

       
    }
}
