using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc
{
    public class PendingChangeStatus : IEquatable<PendingChangeStatus>
    {
        string _text;
        public PendingChangeStatus(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            _text = text;
        }

        public override string ToString()
        {
            return _text;
        }

        /// <summary>
        /// Gets the text as shown in the property browser
        /// </summary>
        public string Text
        {
            get { return _text; }
        }

        /// <summary>
        /// Gets the text as shown in the pending commits window
        /// </summary>
        public string PendingCommitText
        {
            get { return _text; }
        }

        public override bool Equals(object obj)
        {
            if(!(obj is PendingChangeStatus))
                return false;

            return (PendingChangeStatus)obj == this;
        }

        public override int GetHashCode()
        {
            return _text.GetHashCode();
        }

        #region IEquatable<PendingChangeStatus> Members

        public bool Equals(PendingChangeStatus other)
        {
            if ((object)other == null)
                return false;

            return _text == other._text;
        }

        #endregion
    }
}
