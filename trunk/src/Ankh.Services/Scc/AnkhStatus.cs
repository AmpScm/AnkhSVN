using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using System.Diagnostics;

namespace Ankh
{
    [DebuggerDisplay("Content={LocalContentStatus}, Property={LocalPropertyStatus}, Uri={Uri}")]
    public sealed class AnkhStatus
    {
        readonly SvnNodeKind _nodeKind;        
        readonly string _changeList;
        readonly SvnStatus _localContentStatus;
        readonly bool _localCopied;
        readonly bool _localLocked;
        readonly SvnStatus _localPropertyStatus;
        readonly bool _switched;
        readonly Uri _uri;

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
            _localContentStatus = args.LocalContentStatus;
            _localCopied = args.LocalCopied;
            _localPropertyStatus = args.LocalPropertyStatus;
            _switched = args.Switched;
            _uri = args.Uri;

            if (args.WorkingCopyInfo != null)
            {
                _lastChangeTime = args.WorkingCopyInfo.LastChangeTime;
                _lastChangeRevision = args.WorkingCopyInfo.LastChangeRevision;
                _lastChangeAuthor = args.WorkingCopyInfo.LastChangeAuthor;
                _revision = args.WorkingCopyInfo.Revision;
                _repositoryId = args.WorkingCopyInfo.RepositoryIdValue;
                _changeList = args.WorkingCopyInfo.ChangeList;
                _localLocked = args.WorkingCopyInfo.LockToken != null;
            }
        }        

        /// <summary>
        /// Create non-locked, non-copied item with status specified
        /// </summary>
        /// <param name="allStatuses"></param>
        private AnkhStatus(SvnStatus allStatuses)
        {
            _localContentStatus = allStatuses;
            _localPropertyStatus = SvnStatus.None;
            //_localLocked = false;
            //_localCopied = false;
        }

        #region Static instances
        readonly static AnkhStatus _unversioned = new AnkhStatus(SvnStatus.NotVersioned);
        readonly static AnkhStatus _none = new AnkhStatus(SvnStatus.None);
        /// <summary>
        /// Default status for nodes which do exist but are not managed
        /// </summary>
        public static AnkhStatus NotVersioned
        {
            get { return _unversioned; }
        }

        /// <summary>
        /// Default status for nodes which don't exist and are not managed
        /// </summary>
        public static AnkhStatus NotExisting
        {
            get { return _none; }
        }
        #endregion

        /// <summary>
        /// Content status in working copy
        /// </summary>
        public SvnStatus LocalContentStatus
        {
            get { return _localContentStatus; }
        }

        /// <summary>
        /// Property status in working copy
        /// </summary>
        public SvnStatus LocalPropertyStatus
        {
            get { return _localPropertyStatus; }
        }

        public SvnStatus CombinedStatus
        {
            get
            {
                switch(_localContentStatus)
                {
                    // High priority statuses on the content
                    case SvnStatus.Obstructed:
                    case SvnStatus.Missing:
                    case SvnStatus.Incomplete: 
                        return _localContentStatus;
                }

                switch(_localPropertyStatus)
                {
                    // High priority on the properties
                    case SvnStatus.Conflicted:
                        return _localPropertyStatus;
                }

                if (_localContentStatus != SvnStatus.Normal)
                    return _localContentStatus;
                else if (_localPropertyStatus != SvnStatus.None)
                    return _localPropertyStatus;
                else
                    return _localContentStatus;
            }
        }

        /// <summary>
        /// Gets the change list in which the file is placed
        /// </summary>
        /// <value>The change list.</value>
        /// <remarks>The changelist value is only valid if the file is modified</remarks>
        public string ChangeList
        {
            get { return _changeList; }
        }

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

        public Guid RepositoryGuid
        {
            get { return (_repositoryId != null) ? new Guid(_repositoryId) : Guid.Empty; }
        }        
       
        /// <summary>
        /// Gets a boolean indicating whether the file is copied in the working copy
        /// </summary>
        public bool IsCopied
        {
            get { return _localCopied; }
        }
        
        /// <summary>
        /// Gets a boolean indicating whether the workingcopy is locked in the local working copy
        /// </summary>
        public bool IsLockedLocal
        {
            get { return _localLocked; }
        }

        /// <summary>
        /// Gets a boolean indicating whether the file is switched in the working copy
        /// </summary>
        public bool IsSwitched
        {
            get { return _switched; }
        }

        public Uri Uri
        {
            get { return _uri; }
        }           
    }

}
