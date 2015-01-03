using System;
using System.Collections.Generic;
using System.Diagnostics;
using Ankh.Commands;
using Ankh.Scc.Git;

namespace Ankh
{
    public interface IGitItemStateUpdate
    {
        IList<GitItem> GetUpdateQueueAndClearScheduled();

        void SetDocumentDirty(bool value);
        void SetSolutionContained(bool inSolution, bool sccExcluded);
    }

    partial class GitItem : IGitItemStateUpdate
    {
        GitItemState _currentState;
        GitItemState _validState;
        GitItemState _onceValid;

        const GitItemState MaskRefreshTo = GitItemState.Versioned | GitItemState.Modified | GitItemState.Added
            | GitItemState.Deleted | GitItemState.GitDirty | GitItemState.Ignored | GitItemState.Conflicted;

        public GitItemState GetState(GitItemState flagsToGet)
        {
            GitItemState unavailable = flagsToGet & ~_validState;

            if (unavailable == 0)
                return _currentState & flagsToGet; // We have everything we need

            if (0 != (unavailable & MaskRefreshTo))
            {
                Debug.Assert(_statusDirty != XBool.False);
                RefreshStatus();

                unavailable = flagsToGet & ~_validState;

                Debug.Assert((~_validState & MaskRefreshTo) == 0, "RefreshMe() set all attributes it should");
            }

            /*if (0 != (unavailable & MaskGetAttributes))
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

            if (0 != (unavailable & MaskWCRoot))
            {
                UpdateWCRoot();

                unavailable = flagsToGet & ~_validState;

                Debug.Assert((~_validState & MaskWCRoot) == 0, "UpdateWCRoot() set all attributes it should");
            }

            if (0 != (unavailable & MaskIsAdministrativeArea))
            {
                UpdateAdministrativeArea();

                unavailable = flagsToGet & ~_validState;

                Debug.Assert((~_validState & MaskIsAdministrativeArea) == 0, "UpdateIsAdministrativeArea() set all attributes it should");
            }

            if (unavailable != 0)
            {
                Trace.WriteLine(string.Format("Don't know how to retrieve {0:X} state; clearing dirty flag", (int)unavailable));

                _validState |= unavailable;
            }*/

            return _currentState & flagsToGet;
        }

        private void SetDirty(GitItemState dirty)
        {
            // NOTE: This method is /not/ thread safe, but its callers have race conditions anyway
            // Setting an integer could worst case completely destroy the integer; nothing a refresh can't fix

            _validState &= ~dirty;
        }

        // Mask of states not to broadcast for
        const GitItemState NoBroadcastFor = ~(GitItemState.DocumentDirty | GitItemState.InSolution);

        void SetState(GitItemState set, GitItemState unset)
        {
            // NOTE: This method is /not/ thread safe, but its callers have race conditions anyway
            // Setting an integer could worst case completely destroy the integer; nothing a refresh can't fix

            GitItemState st = (_currentState & ~unset) | set;

            if (st != _currentState)
            {
                // Calculate whether we have a change or just new information
                bool changed = (st & _onceValid & NoBroadcastFor) != (_currentState & _onceValid & NoBroadcastFor);

                if (changed && !_enqueued)
                {
                    _enqueued = true;

                    // Schedule a stat changed broadcast
                    lock (_stateChanged)
                    {
                        _stateChanged.Enqueue(this);

                        ScheduleUpdateNotify();
                    }
                }

                _currentState = st;

            }
            _validState |= (set | unset);
            _onceValid |= _validState;
        }

        void ScheduleUpdateNotify()
        {
            if (_scheduled)
                return;

            IAnkhCommandService cs = _context.GetService<IAnkhCommandService>();

            if (cs != null)
                cs.PostTickCommand(ref _scheduled, AnkhCommand.TickRefreshSvnItems);
        }

        void IGitItemUpdate.SetState(GitItemState set, GitItemState unset)
        {
            SetState(set, unset);
        }
        void IGitItemUpdate.SetDirty(GitItemState dirty)
        {
            SetDirty(dirty);
        }
        bool IGitItemUpdate.TryGetState(GitItemState get, out GitItemState value)
        {
            return TryGetState(get, out value);
        }

        IList<GitItem> IGitItemStateUpdate.GetUpdateQueueAndClearScheduled()
        {
            throw new NotImplementedException();
        }

        void IGitItemStateUpdate.SetDocumentDirty(bool value)
        {
            throw new NotImplementedException();
        }

        void IGitItemStateUpdate.SetSolutionContained(bool inSolution, bool sccExcluded)
        {
            throw new NotImplementedException();
        }
    }
}
