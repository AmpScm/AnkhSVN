﻿using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Ids;

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
        bool _dynamicMenuEnd;
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

        /// <summary>
        /// The command is enabled
        /// </summary>
        public bool Enabled
        {
            get { return !_disabled; }
            set { _disabled = !value; }
        }

        /// <summary>
        /// The command is visible (Requires dynamicVisibility)
        /// </summary>
        public bool Visible
        {
            get { return !_invisible; }
            set { _invisible = !value; }
        }

        /// <summary>
        /// The command is an on-off toggle and is currently on.
        /// </summary>
        public bool Latched
        {
            get { return _latched; }
            set { _latched = value; }
        }

        /// <summary>
        /// Visual Studio SDK: Reserved for future usage
        /// </summary>
        public bool Ninched
        {
            get { return _ninched; }
            set { _ninched = value; }
        }

        /// <summary>
        /// Allows updating the text (Use <see cref="TextQueryType" /> to determine what text to update
        /// </summary>
        public string Text
        {
            get { return _text ?? _originalText; }
            set { _text = value; }
        }

        /// <summary>
        /// Gets the type of the text query.
        /// </summary>
        /// <value>The type of the text query.</value>
        public TextQueryType TextQueryType
        {
            get { return _queryType; }
        }

        /// <summary>
        /// Marks the end of a dynamic menu range if set
        /// </summary>
        public bool DynamicMenuEnd
        {
            get { return _dynamicMenuEnd; }
            set { _dynamicMenuEnd = value; }
        }

        /// <summary>
        /// Gets the original text
        /// </summary>
        public string OriginalText
        {
            get { return _originalText; }
        }
    }
}
