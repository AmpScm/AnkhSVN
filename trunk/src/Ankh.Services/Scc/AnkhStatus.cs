using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;

namespace Ankh
{
    public sealed class AnkhStatus
    {
        private AnkhStatus(SvnStatusEventArgs args)
        {
            fullPath = args.FullPath;
            isRemoteUpdated = args.IsRemoteUpdated;
            localContentStatus = args.LocalContentStatus;
            localCopied = args.LocalCopied;
            localLocked = args.LocalLocked;
            localPropertyStatus = args.LocalPropertyStatus;
            path = args.Path;
            remoteContentStatus = args.RemoteContentStatus;
            remoteLock = args.RemoteLock;
            remotePropertyStatus = args.RemotePropertyStatus;
            remoteUpdateCommitAuthor = args.RemoteUpdateCommitAuthor;
            remoteUpdateCommitDate = args.RemoteUpdateCommitTime;
            remoteUpdateNodeKind = args.RemoteUpdateNodeKind;
            remoteUpdateRevision = args.RemoteUpdateRevision;
            switched = args.Switched;
            uri = args.Uri;
            workingCopyInfo = args.WorkingCopyInfo;
        }

        public override bool Equals(object obj)
        {
            AnkhStatus other = obj as AnkhStatus;
            if (other == null)
                return false;

            return other.LocalContentStatus == LocalContentStatus &&
                other.LocalPropertyStatus == LocalPropertyStatus &&
                other.RemoteContentStatus == RemoteContentStatus &&
                other.RemotePropertyStatus == RemotePropertyStatus &&
                other.LocalLocked == LocalLocked &&
                other.LocalCopied == LocalCopied &&
                other.Switched == Switched &&
                Compare(other.WorkingCopyInfo, WorkingCopyInfo);
        }

        public override int GetHashCode()
        {
            int hashCode = WorkingCopyInfo == null ? 0 : WorkingCopyInfo.GetHashCode();

            if (LocalCopied)
                hashCode ^= 1 << 32;
            if (LocalLocked)
                hashCode ^= 1 << 31;
            if (Switched)
                hashCode ^= 1 << 30;

            return hashCode ^
                (int)LocalContentStatus << 24 ^ // Although SvnStatus is an int, treat it as a byte because there will probably never be more than 256 statuses
                (int)LocalPropertyStatus << 16 ^
                (int)RemoteContentStatus << 8 ^
                (int)RemotePropertyStatus;
        }

        static bool Compare(SvnWorkingCopyInfo a, SvnWorkingCopyInfo b)
        {
            if (a == null)
            {
                if (b == null)
                    return true;
                else
                    return false;
            }
            else
            {
                if (b == null)
                    return false;

                return Compare(a.Name, b.Name) &&
                    a.Revision == b.Revision &&
                    Compare(a.Checksum, b.Checksum) &&
                    Compare(a.Uri, b.Uri) &&
                    Compare(a.RepositoryId, b.RepositoryId) &&
                    Compare(a.RepositoryUri, b.RepositoryUri) &&
                    a.NodeKind == b.NodeKind &&
                    a.Schedule == b.Schedule &&
                    a.IsCopy == b.IsCopy &&
                    a.IsDeleted == b.IsDeleted &&
                    Compare(a.CopiedFrom, b.CopiedFrom) &&
                    a.CopiedFromRevision == b.CopiedFromRevision &&
                    Compare(a.ConflictNewFile, b.ConflictNewFile) &&
                    Compare(a.ConflictOldFile, b.ConflictOldFile) &&
                    Compare(a.ConflictWorkFile, b.ConflictWorkFile) &&
                    Compare(a.PropertyRejectFile, b.PropertyRejectFile) &&
                    Compare(a.ContentChangeTime, b.ContentChangeTime) &&
                    Compare(a.PropertyChangeTime, b.PropertyChangeTime) &&
                    a.LastChangeRevision == b.LastChangeRevision &&
                    Compare(a.LastChangeTime, b.LastChangeTime) &&
                    Compare(a.LastChangeAuthor, b.LastChangeAuthor) &&
                    Compare(a.LockToken, b.LockToken) &&
                    Compare(a.LockComment, b.LockComment) &&
                    Compare(a.LockOwner, b.LockOwner) &&
                    Compare(a.LockTime, b.LockTime) &&
                    Compare(a.ChangeList, b.ChangeList) &&
                    a.IsAbsent == b.IsAbsent &&
                    a.IsIncomplete == b.IsIncomplete &&
                    a.WorkingCopySize == b.WorkingCopySize;
            }
        }
        static bool Compare(object a, object b)
        {
            if (a == null)
            {
                if (b == null)
                    return true;
                else
                    return false;
            }
            else
            {
                if (b == null)
                    return false;
                else
                    return a.Equals(b);
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
            remoteContentStatus = allStatuses;
            remotePropertyStatus = allStatuses;
        }

        static AnkhStatus()
        {
            unversioned = new AnkhStatus(SvnStatus.NotVersioned);

            none = new AnkhStatus(SvnStatus.None);
        }

        public static implicit operator AnkhStatus(SvnStatusEventArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");

            args.Detach();
            return new AnkhStatus(args);
        }

        public static AnkhStatus Unversioned
        {
            get { return unversioned; }
        }
        public static AnkhStatus None
        {
            get { return none; }
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
        //     Gets the out of date status of the item; if true the RemoteUpdate* properties
        //     are set
        public bool IsRemoteUpdated
        {
            get { return isRemoteUpdated; }
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
        public SvnStatus RemoteContentStatus
        {
            get { return remoteContentStatus; }
        }
        public SvnLockInfo RemoteLock
        {
            get { return remoteLock; }
        }
        public SvnStatus RemotePropertyStatus
        {
            get { return remotePropertyStatus; }
        }

        //
        // Summary:
        //     Out of Date: Gets the author of the OutOfDate commit
        public string RemoteUpdateCommitAuthor
        {
            get { return remoteUpdateCommitAuthor; }
        }
        //
        // Summary:
        //     Out of Date: Last commit date of the item
        public DateTime RemoteUpdateCommitDate
        {
            get { return remoteUpdateCommitDate; }
        }
        //
        // Summary:
        //     Out of Date: Gets the node kind of the OutOfDate commit
        public SvnNodeKind RemoteUpdateNodeKind
        {
            get { return remoteUpdateNodeKind; }
        }

        //
        // Summary:
        //     Out of Date: Last commit version of the item
        public long RemoteUpdateRevision
        {
            get { return remoteUpdateRevision; }
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
        public SvnWorkingCopyInfo WorkingCopyInfo
        {
            get { return workingCopyInfo; }
        }


        readonly SvnWorkingCopyInfo workingCopyInfo;
        readonly static AnkhStatus unversioned;
        readonly static AnkhStatus none;
        readonly string fullPath;
        readonly bool isRemoteUpdated;
        readonly SvnStatus localContentStatus;
        readonly bool localCopied;
        readonly bool localLocked;
        readonly SvnStatus localPropertyStatus;
        readonly string path;
        readonly SvnStatus remoteContentStatus;
        readonly SvnLockInfo remoteLock;
        readonly SvnStatus remotePropertyStatus;
        readonly string remoteUpdateCommitAuthor;
        readonly DateTime remoteUpdateCommitDate;
        readonly SvnNodeKind remoteUpdateNodeKind;
        readonly long remoteUpdateRevision;
        readonly bool switched;
        readonly Uri uri;
    }

}
