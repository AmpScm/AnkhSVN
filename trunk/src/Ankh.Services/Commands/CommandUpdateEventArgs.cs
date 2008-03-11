using System;
using System.Collections.Generic;
using System.Text;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    public enum TextQueryType
    {
        None,
        Name,
        Status
    }

    public class CommandUpdateEventArgs : BaseCommandEventArgs
    {
        readonly TextQueryType _queryType;
        readonly string _originalText;
        bool _disabled;
        bool _invisible;
        bool _latched;
        bool _ninched;
        string _text;

        public CommandUpdateEventArgs(AnkhCommand command, AnkhContext context, TextQueryType textQuery, string oldText)
            : this(command, context)
        {
            _queryType = textQuery;
            if (textQuery != TextQueryType.None)
                _originalText = oldText;
        }

        public CommandUpdateEventArgs(AnkhCommand command, AnkhContext context)
            : base(command, context)
        {
        }

        public bool Enabled
        {
            get { return !_disabled; }
            set { _disabled = !value; }
        }

        public bool Visible
        {
            get { return !_invisible; }
            set { _invisible = value; }
        }

        public bool Latched
        {
            get { return _latched; }
            set { _latched = value; }
        }

        public bool Ninched
        {
            get { return _ninched; }
            set { _ninched = value; }
        }

        public string Text
        {
            get { return _text ?? _originalText; }
            set { _text = value; }
        }

        public string OriginalText
        {
            get { return _originalText; }
        }
    }
}
