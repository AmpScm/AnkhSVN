using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Ankh.Scc;
using SharpSvn;

namespace Ankh
{
    public interface ISvnItemStateUpdate
    {
        List<SvnItem> GetUpdateQueueAndClearScheduled();

        void SetDocumentDirty(bool value);
        void SetSolutionContained(bool value);
    }

    partial class SvnItem : ISvnItemStateUpdate
    {
        SvnItemState _currentState;
        SvnItemState _validState;
        SvnItemState _onceValid;

        const SvnItemState MaskRefreshTo = SvnItemState.Versioned | SvnItemState.HasLockToken | SvnItemState.Obstructed | SvnItemState.Modified | SvnItemState.PropertyModified | SvnItemState.Added | SvnItemState.HasCopyOrigin
            | SvnItemState.Deleted | SvnItemState.Replaced | SvnItemState.HasProperties | SvnItemState.ContentConflicted | SvnItemState.PropertyModified | SvnItemState.SvnDirty | SvnItemState.Ignored;

        public SvnItemState GetState(SvnItemState flagsToGet)
        {
            SvnItemState unavailable = flagsToGet & ~_validState;

            if (unavailable == 0)
                return _currentState & flagsToGet; // We have everything we need

            if (0 != (unavailable & MaskRefreshTo))
            {
                Debug.Assert(_statusDirty != XBool.False);
                RefreshStatus();

                unavailable = flagsToGet & ~_validState;

                Debug.Assert((~_validState & MaskRefreshTo) == 0, "RefreshMe() set all attributes it should");
            }

            if (0 != (unavailable & MaskGetAttributes))
            {
                UpdateAttributeInfo();

                unavailable = flagsToGet & ~_validState;

                Debug.Assert((~_validState & MaskGetAttributes) == 0, "UpdateAttributeInfo() set all attributes it should");
            }

            if (0 != (unavailable & MaskUpdateSolution))
            {
                UpdateSolutionInfo();

                unavailable = flagsToGet & ~_validState;

                Debug.Assert((~_validState & MaskUpdateSolution) == 0, "UpdateSolution() set all attributes it should");
            }

            if (0 != (unavailable & MaskDocumentInfo))
            {
                UpdateDocumentInfo();

                unavailable = flagsToGet & ~_validState;

                Debug.Assert((~_validState & MaskDocumentInfo) == 0, "UpdateDocumentInfo() set all attributes it should");
            }

            if (0 != (unavailable & MaskVersionable))
            {
                UpdateVersionable();

                unavailable = flagsToGet & ~_validState;

                Debug.Assert((~_validState & MaskVersionable) == 0, "UpdateVersionable() set all attributes it should");
            }

            if (0 != (unavailable & MaskMustLock))
            {
                UpdateMustLock();

                unavailable = flagsToGet & ~_validState;

                Debug.Assert((~_validState & MaskMustLock) == 0, "UpdateMustLock() set all attributes it should");
            }

            if (0 != (unavailable & MaskTextFile))
            {
                UpdateTextFile();

                unavailable = flagsToGet & ~_validState;

                Debug.Assert((~_validState & MaskTextFile) == 0, "UpdateTextFile() set all attributes it should");
            }

            if (0 != (unavailable & MaskNested))
            {
                UpdateNested();

                unavailable = flagsToGet & ~_validState;

                Debug.Assert((~_validState & MaskNested) == 0, "UpdateNested() set all attributes it should");
            }

            if (unavailable != 0)
            {
                Trace.WriteLine(string.Format("Don't know how to retrieve {0:X} state; clearing dirty flag", (int)unavailable));

                _validState |= unavailable;
            }

            return _currentState & flagsToGet;
        }

        List<SvnItem> ISvnItemStateUpdate.GetUpdateQueueAndClearScheduled()
        {
            lock (_stateChanged)
            {
                _scheduled = false;

                if (_stateChanged.Count == 0)
                    return null;

                List<SvnItem> modified = new List<SvnItem>(_stateChanged.Count);
                modified.AddRange(_stateChanged);
                _stateChanged.Clear();

                foreach (SvnItem i in modified)
                    i._enqueued = false;


                return modified;
            }
        }

        private void SetDirty(SvnItemState dirty)
        {
            // NOTE: This method is /not/ thread safe, but its callers have race conditions anyway
            // Setting an integer could worst case completely destroy the integer; nothing a refresh can't fix

            _validState &= ~dirty;
        }

        void SetState(SvnItemState set, SvnItemState unset)
        {
            // NOTE: This method is /not/ thread safe, but its callers have race conditions anyway
            // Setting an integer could worst case completely destroy the integer; nothing a refresh can't fix

            SvnItemState st = (_currentState & ~unset) | set;

            if (st != _currentState)
            {
                // Calculate whether we have a change or just new information
                bool changed = (st & _onceValid) != (_currentState & _onceValid);

                if (changed)
                {
                    if (!_enqueued)
                    {
                        _enqueued = true;

                        // Schedule a stat changed broadcast
                        lock (_stateChanged)
                        {
                            _stateChanged.Enqueue(this);

                            ScheduleUpdateNotify();
                        }
                    }
                }
                _currentState = st;

            }
            _validState |= (set | unset);
            _onceValid |= _validState;
        }

        void ISvnItemUpdate.SetState(SvnItemState set, SvnItemState unset)
        {
            SetState(set, unset);
        }
        void ISvnItemUpdate.SetDirty(SvnItemState dirty)
        {
            SetDirty(dirty);
        }
        bool ISvnItemUpdate.TryGetState(SvnItemState get, out SvnItemState value)
        {
            return TryGetState(get, out value);
        }

        #region Versionable

        const SvnItemState MaskVersionable = SvnItemState.Versionable;

        void UpdateVersionable()
        {
            bool versionable;

            SvnItemState state;

            if (TryGetState(SvnItemState.Versioned, out state) && state != 0)
                versionable = true;
            else if (Exists && SvnTools.IsBelowManagedPath(FullPath)) // Will call GetState again!
                versionable = true;
            else
                versionable = false;

            if (versionable)
                SetState(SvnItemState.Versionable, SvnItemState.None);
            else
                SetState(SvnItemState.None, SvnItemState.Versionable);
        }

        #endregion

        #region DocumentInfo

        const SvnItemState MaskDocumentInfo = SvnItemState.DocumentDirty;

        void UpdateDocumentInfo()
        {
            IAnkhOpenDocumentTracker dt = _context.GetService<IAnkhOpenDocumentTracker>();

            if (dt == null)
            {
                // We /must/ make the state not dirty
                SetState(SvnItemState.None, SvnItemState.DocumentDirty);
                return;
            }

            if (dt.IsDocumentDirty(FullPath, true))
                SetState(SvnItemState.DocumentDirty, SvnItemState.None);
            else
                SetState(SvnItemState.None, SvnItemState.DocumentDirty);
        }

        #endregion

        #region Solution Info
        const SvnItemState MaskUpdateSolution = SvnItemState.InSolution;
        void UpdateSolutionInfo()
        {
            IProjectFileMapper pfm = _context.GetService<IProjectFileMapper>();
            bool inSolution = false;

            if (pfm != null)
                inSolution = pfm.ContainsPath(FullPath);

            if (inSolution)
                SetState(SvnItemState.InSolution, SvnItemState.None);
            else
                SetState(SvnItemState.None, SvnItemState.InSolution);
        }

        void ISvnItemStateUpdate.SetSolutionContained(bool inSolution)
        {
            if (inSolution)
                SetState(SvnItemState.InSolution, SvnItemState.None);
            else
                SetState(SvnItemState.None, SvnItemState.InSolution);
        }

        #endregion

        #region Must Lock
        const SvnItemState MaskMustLock = SvnItemState.MustLock;
        void UpdateMustLock()
        {
            SvnItemState value = SvnItemState.IsDiskFile | SvnItemState.ReadOnly | SvnItemState.Versioned;
            SvnItemState v;

            bool mustLock;

            if (TryGetState(SvnItemState.Versioned, out v) && (v == 0))
                mustLock = false;
            else if (TryGetState(SvnItemState.HasProperties, out v) && (v == 0))
                mustLock = false;
            else if (TryGetState(SvnItemState.ReadOnly, out v) && (v == 0))
                mustLock = false;
            else if (GetState(value) != value)
                mustLock = false;
            else
            {
                using (SvnClient client = _context.GetService<ISvnClientPool>().GetNoUIClient())
                {
                    string propVal;

                    if (client.TryGetProperty(new SvnPathTarget(_fullPath), SvnPropertyNames.SvnNeedsLock, out propVal))
                    {
                        mustLock = propVal != null; // Value should be equal to SvnPropertyNames.SvnBooleanValue
                    }
                    else
                        mustLock = false;
                }
            }

            if (mustLock)
                SetState(SvnItemState.MustLock, SvnItemState.None);
            else
                SetState(SvnItemState.None, SvnItemState.MustLock);
        }
        #endregion

        #region TextFile File
        const SvnItemState MaskTextFile = SvnItemState.IsTextFile;
        void UpdateTextFile()
        {
            SvnItemState value = SvnItemState.IsDiskFile | SvnItemState.Versioned;
            SvnItemState v;

            bool isTextFile;

            if (TryGetState(SvnItemState.Versioned, out v) && (v == 0))
                isTextFile = false;
            else if (GetState(value) != value)
                isTextFile = false;
            else
            {
                using (SvnWorkingCopyClient client = _context.GetService<ISvnClientPool>().GetWcClient())
                {
                    SvnWorkingCopyStateArgs a = new SvnWorkingCopyStateArgs();
                    a.ThrowOnError = false;
                    a.RetrieveFileData = true;
                    SvnWorkingCopyState state;
                    if (client.GetState(FullPath, out state))
                    {
                        isTextFile = state.IsTextFile;
                    }
                    else
                        isTextFile = false;
                }
            }

            if (isTextFile)
                SetState(SvnItemState.IsTextFile, SvnItemState.None);
            else
                SetState(SvnItemState.None, SvnItemState.IsTextFile);
        }
        #endregion

        #region Attribute Info
        const SvnItemState MaskGetAttributes = SvnItemState.Exists | SvnItemState.ReadOnly | SvnItemState.IsDiskFile | SvnItemState.IsDiskFolder;

        void UpdateAttributeInfo()
        {
            // One call of the kernel's GetFileAttributesW() gives us most info we need

            uint value = NativeMethods.GetFileAttributes(FullPath);

            if (value == NativeMethods.INVALID_FILE_ATTRIBUTES)
            {
                // File does not exist / no rights, etc.

                SetState(SvnItemState.None,
                    SvnItemState.Exists | SvnItemState.ReadOnly | SvnItemState.MustLock | SvnItemState.Versionable | SvnItemState.IsDiskFolder | SvnItemState.IsDiskFile);

                return;
            }

            SvnItemState set = SvnItemState.Exists;
            SvnItemState unset = SvnItemState.None;

            if ((value & NativeMethods.FILE_ATTRIBUTE_READONLY) != 0)
                set |= SvnItemState.ReadOnly;
            else
                unset = SvnItemState.ReadOnly;

            if ((value & NativeMethods.FILE_ATTRIBUTE_DIRECTORY) != 0)
            {
                unset |= SvnItemState.IsDiskFile | SvnItemState.ReadOnly;
                set = SvnItemState.IsDiskFolder | (set & ~SvnItemState.ReadOnly); // Don't set readonly
            }
            else
            {
                set |= SvnItemState.IsDiskFile;
                unset |= SvnItemState.IsDiskFolder;
            }

            SetState(set, unset);
        }
        #endregion

        #region Nested Info
        const SvnItemState MaskNested = SvnItemState.IsNested;
        static readonly Uri parentRefUri = new Uri("../", UriKind.Relative);

        void UpdateNested()
        {
            bool isNested;
            SvnItem parentItem;

            AnkhStatus sMe = Status;

            if (!IsDirectory || !IsVersioned)
                isNested = false; // Not nested
            else if ((null == (parentItem = Parent)) || !parentItem.IsVersioned)
                isNested = false; // No versioned parent
            else
            {
                StatusCache.RefreshNested(this);
                SvnItemState r;

                Debug.Assert(TryGetState(SvnItemState.IsNested, out r));

                if (TryGetState(SvnItemState.IsNested, out r))
                    return;

                isNested = false;
            }

            if (isNested)
                SetState(SvnItemState.IsNested, SvnItemState.None);
            else
                SetState(SvnItemState.None, SvnItemState.IsNested);
        }
        #endregion

        #region ISvnItemStateUpdate Members

        void ISvnItemStateUpdate.SetDocumentDirty(bool value)
        {
            if (value)
                SetState(SvnItemState.DocumentDirty, SvnItemState.None);
            else
                SetState(SvnItemState.None, SvnItemState.DocumentDirty);
        }

        #endregion
    }
}
