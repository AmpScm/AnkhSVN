using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc
{
    public enum PendingChangeState
    {
        None,
        New,
        Modified,
        Replaced,
        Copied,
        Added,
        Deleted,
        Missing,
    }
    public class PendingChangeStatus : IEquatable<PendingChangeStatus>
    {
        readonly PendingChangeState _state;
        string _text;
                
        public PendingChangeStatus(PendingChangeState state)
        {
            _state = state;
        }

        public PendingChangeStatus(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            _text = text;
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
                case PendingChangeState.New:
                    return PendingChangeText.StateNew;
                case PendingChangeState.Added:
                    return PendingChangeText.StateAdded;
                case PendingChangeState.Copied:
                    return PendingChangeText.StateCopied;
                case PendingChangeState.Deleted:
                    return PendingChangeText.StateDeleted;
                case PendingChangeState.Replaced:
                    return PendingChangeText.StateReplaced;
                case PendingChangeState.Missing:
                    return PendingChangeText.StateMissing;
                case PendingChangeState.Modified:
                    return PendingChangeText.StateModified;
                default:
                    return State.ToString();
            }            
        }

        public PendingChangeState State
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
