using System;
using System.Collections.Generic;
using System.Text;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
	public class CommandEventArgs : EventArgs
	{
		readonly AnkhCommand _command;
		readonly IContext _context;
		readonly object _argument;
		object _result;
		bool _promptUser;
		bool _dontPromptUser;

		public CommandEventArgs(AnkhCommand command, IContext context)
		{
			_command = command;
			_context = context;
		}

		public CommandEventArgs(AnkhCommand command, IContext context, object argument, bool promptUser, bool dontPromptUser)
			: this(command, context)
		{
			_argument = argument;
			_promptUser = promptUser;
			_dontPromptUser = dontPromptUser;
		}

		public AnkhCommand Command
		{
			get { return _command; }
		}

		public IContext Context
		{
			get { return _context; }
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

	public class CommandUpdateEventArgs : EventArgs
	{
		readonly AnkhCommand _command;
		readonly IContext _context;
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
			_command = command;
			_queryType = textQuery;
			if (textQuery != TextQueryType.None)
				_originalText = oldText;
		}

		public CommandUpdateEventArgs(AnkhCommand command, IContext context)
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

	public class HelpEventArgs : EventArgs
	{
		readonly AnkhCommand _command;

		public HelpEventArgs(AnkhCommand command)
		{
			_command = command;
		}

		public AnkhCommand Command
		{
			get { return _command; }
		}
	}
}
