using System;
using System.Collections.Generic;
using System.Text;
using AnkhSvn.Ids;
using Ankh.Selection;
using Microsoft.VisualStudio.Shell;

namespace Ankh.Commands
{
    public class BaseCommandEventArgs : EventArgs
    {
        readonly AnkhCommand _command;
        readonly IContext _context;
        Nullable<bool> _inAutomation;

        public BaseCommandEventArgs(AnkhCommand command, IContext context)
		{
			_command = command;
			_context = context;
		}

        public AnkhCommand Command
        {
            get { return _command; }
        }

        public IContext Context
        {
            get { return _context; }
        }

        /// <summary>
        /// Gets the Visual Studio selection
        /// </summary>
        /// <value>The selection.</value>
        public ISelectionContext Selection
        {
            get { return _context.SelectionContext; }
        }

        public bool IsInAutomation
        {
            get
            {
                if (_inAutomation.HasValue)
                    return _inAutomation.Value;

                if (Context != null)
                    _inAutomation = VsShellUtilities.IsInAutomationFunction(Context.Package);
                else
                    _inAutomation = false;

                return _inAutomation.Value;
            }
        }
    }
	public class CommandEventArgs : BaseCommandEventArgs
	{		
		readonly object _argument;
		object _result;
		bool _promptUser;
		bool _dontPromptUser;

		public CommandEventArgs(AnkhCommand command, IContext context)
            : base(command, context)
		{
		}

		public CommandEventArgs(AnkhCommand command, IContext context, object argument, bool promptUser, bool dontPromptUser)
			: this(command, context)
		{
			_argument = argument;
			_promptUser = promptUser;
			_dontPromptUser = dontPromptUser;
		}				

		public object Argument
		{
			get { return _argument; }
		}

		public object Result
		{
			get { return _result; }
			set { _result = value; }
		}

		public bool DontPrompt
		{
			get { return _dontPromptUser; }
		}

		public bool PromptUser
		{
			get { return _promptUser; }
		}
	}

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

		public CommandUpdateEventArgs(AnkhCommand command, IContext context, TextQueryType textQuery, string oldText)
			: this(command, context)
		{
			_queryType = textQuery;
			if (textQuery != TextQueryType.None)
				_originalText = oldText;
		}

		public CommandUpdateEventArgs(AnkhCommand command, IContext context)
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
