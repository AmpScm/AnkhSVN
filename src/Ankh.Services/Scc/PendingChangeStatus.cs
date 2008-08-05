using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc
{
    
    public class PendingChangeStatus : IEquatable<PendingChangeStatus>
    {
        readonly PendingChangeKind _state;
        string _text;
                
        public PendingChangeStatus(PendingChangeKind state)
        {
            _state = state;
        }

        public override string ToString()
        {
            return Text;
        }

        /// <summary>
        /// Gets the text as shown in the property browser
        /// </summary>
        public string Text
        {
            get { return _text ?? (_text = GetText()); }
        }

        private string GetText()
        {
            switch (State)
            {
                case PendingChangeKind.New:
                    return PendingChangeText.StateNew;
                case PendingChangeKind.Added:
                    return PendingChangeText.StateAdded;
                case PendingChangeKind.Copied:
                    return PendingChangeText.StateCopied;
                case PendingChangeKind.Deleted:
                    return PendingChangeText.StateDeleted;
                case PendingChangeKind.Replaced:
                    return PendingChangeText.StateReplaced;
                case PendingChangeKind.Missing:
                    return PendingChangeText.StateMissing;
                case PendingChangeKind.Modified:
                    return PendingChangeText.StateModified;
                case PendingChangeKind.EditorDirty:
                    return PendingChangeText.StateEdited;
                case PendingChangeKind.PropertyModified:
                    return PendingChangeText.PropertyModified;
                case PendingChangeKind.LockedOnly:
                    return PendingChangeText.Locked;
                case PendingChangeKind.Incomplete:
                    return PendingChangeText.Incomplete;
                default:
                    return State.ToString();
            }            
        }

        public PendingChangeKind State
        {
            get { return _state; }
        }

        /// <summary>
        /// Gets the text as shown in the pending commits window
        /// </summary>
        public string PendingCommitText
        {
            get { return Text; }
        }

        public override bool Equals(object obj)
        {
            if(!(obj is PendingChangeStatus))
                return false;

            return (PendingChangeStatus)obj == this;
        }

        public override int GetHashCode()
        {
            return _state.GetHashCode();
        }

        #region IEquatable<PendingChangeStatus> Members

        public bool Equals(PendingChangeStatus other)
        {
            if ((object)other == null)
                return false;

            return State == other.State && Text == other.Text; // Todo: Remove text check
        }

        #endregion
    }
}
