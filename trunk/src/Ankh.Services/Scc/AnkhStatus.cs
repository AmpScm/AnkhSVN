using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;

namespace Ankh
{
    public sealed class AnkhStatus
    {
        readonly SvnNodeKind _nodeKind;        
        readonly string fullPath;
        readonly SvnStatus localContentStatus;
        readonly bool localCopied;
        readonly bool localLocked;
        readonly SvnStatus localPropertyStatus;
        readonly string path;
        readonly bool switched;
        readonly Uri uri;

        readonly string _repositoryId;
        readonly DateTime _lastChangeTime;
        readonly string _lastChangeAuthor;
        readonly long _lastChangeRevision;
        readonly long _revision;

        public AnkhStatus(SvnStatusEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            _nodeKind = args.NodeKind;
            fullPath = args.FullPath;
            localContentStatus = args.LocalContentStatus;
            localCopied = args.LocalCopied;
            localLocked = args.LocalLocked;
            localPropertyStatus = args.LocalPropertyStatus;
            path = args.Path;
            switched = args.Switched;
            uri = args.Uri;

            if (args.WorkingCopyInfo != null)
            {
                _lastChangeTime = args.WorkingCopyInfo.LastChangeTime;
                _lastChangeRevision = args.WorkingCopyInfo.LastChangeRevision;
                _lastChangeAuthor = args.WorkingCopyInfo.LastChangeAuthor;
                _revision = args.WorkingCopyInfo.Revision;
                _repositoryId = args.WorkingCopyInfo.RepositoryId.ToString();
            }
        }        

        /// <summary>
        /// Create non-locked, non-copied item with status specified
        /// </summary>
        /// <param name="allStatuses"></param>
        private AnkhStatus(SvnStatus allStatuses)
        {
            localContentStatus = allStatuses;
            localPropertyStatus = allStatuses;
            localLocked = false;
            localCopied = false;
        }

        #region Static instances
        readonly static AnkhStatus _unversioned = new AnkhStatus(SvnStatus.NotVersioned);
        readonly static AnkhStatus _none = new AnkhStatus(SvnStatus.None);
        public static AnkhStatus Unversioned
        {
            get { return _unversioned; }
        }

        public static AnkhStatus None
        {
            get { return _none; }
        }
        #endregion

        public SvnNodeKind NodeKind
        {
            get { return _nodeKind; }
        }

        public DateTime LastChangeTime
        {
            get { return _lastChangeTime; }
        }

        public string LastChangeAuthor
        {
            get { return _lastChangeAuthor; }
        }

        public long LastChangeRevision
        {
            get { return _lastChangeRevision; }
        }

        public long Revision
        {
            get { return _revision; }
        }

        public string RepositoryId
        {
            get { return _repositoryId; }
        }

        // Summary:
        //     The path the notification is about, translated via System.IO.Path.GetFullPath(System.String)
        //
        // Remarks:
        //     The SharpSvn.SvnStatusEventArgs.FullPath property contains the path in normalized
        //     format; while SharpSvn.SvnStatusEventArgs.Path returns the exact path from
        //     the subversion api
        public string FullPath
        {
            get { return fullPath; }
        }

        //
        // Summary:
        //     Content status in working copy
        public SvnStatus LocalContentStatus
        {
            get { return localContentStatus; }
        }
        //
        // Summary:
        //     Gets a boolean indicating whether the file is copied in the working copy
        //
        // Remarks:
        //     A file or directory can be 'copied' if it's scheduled for addition-with-history
        //     (or part of a subtree that is scheduled as such.).
        public bool LocalCopied
        {
            get { return localCopied; }
        }
        //
        // Summary:
        //     Gets a boolean indicating whether the workingcopy is locked
        public bool LocalLocked
        {
            get { return localLocked; }
        }
        //
        // Summary:
        //     Property status in working copy
        public SvnStatus LocalPropertyStatus
        {
            get { return localPropertyStatus; }
        }
        public string Path
        {
            get { return path; }
        }
        //
        // Summary:
        //     Gets a boolean indicating whether the file is switched in the working copy
        public bool Switched
        {
            get { return switched; }
        }
        public Uri Uri
        {
            get { return uri; }
        }           
    }

}
