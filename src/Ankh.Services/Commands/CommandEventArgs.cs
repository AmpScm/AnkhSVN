using System;
using System.Collections.Generic;
using System.Text;
using AnkhSvn.Ids;
using Ankh.Selection;

namespace Ankh.Commands
{    
	public class CommandEventArgs : BaseCommandEventArgs
	{		
		readonly object _argument;
		object _result;
		bool _promptUser;
		bool _dontPromptUser;

		public CommandEventArgs(AnkhCommand command, AnkhContext context)
            : base(command, context)
		{
		}

		public CommandEventArgs(AnkhCommand command, AnkhContext context, object argument, bool promptUser, bool dontPromptUser)
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

	}
