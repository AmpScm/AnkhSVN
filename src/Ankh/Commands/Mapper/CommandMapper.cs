using System;
using System.Collections.Generic;
using System.Text;
using AnkhSvn.Ids;
using System.Diagnostics;

namespace Ankh.Commands.Mapper
{
	public class CommandMapItem
	{
		readonly AnkhCommand _command;		
		ICommand _iCommand;

		public event EventHandler<CommandEventArgs> Execute;
		public event EventHandler<CommandUpdateEventArgs> Update;

		public CommandMapItem(AnkhCommand command)
		{
			_command = command;
		}

		public AnkhCommand Command
		{
			get { return _command; }
		}

		public ICommand ICommand
		{
			get { return _iCommand; }
			set { _iCommand = value; }
		}

		protected internal void OnExecute(CommandEventArgs e)
		{
			if (ICommand != null)
				ICommand.OnExecute(e);

			if (Execute != null)
				Execute(this, e);
		}

		protected internal void OnUpdate(CommandUpdateEventArgs e)
		{
            if (ICommand != null)
                ICommand.OnUpdate(e);			

			if (Update != null)
				Update(this, e);
		}

		public bool IsHandled
		{
			get { return (Execute != null) || (ICommand != null); }
		}
	}

	public sealed class CommandMapper
	{
		Dictionary<AnkhCommand, CommandMapItem> _map;

		public CommandMapper()
		{
			_map = new Dictionary<AnkhCommand, CommandMapItem>();
		}

		public bool PerformUpdate(AnkhCommand command, CommandUpdateEventArgs e)
		{
			EnsureLoaded();
			CommandMapItem item;

			if (_map.TryGetValue(command, out item))
			{
				try
				{
					item.OnUpdate(e);
				}
				catch (Exception ex)
				{
					e.Context.ErrorHandler.Handle(ex);
					return false;
				}

				return item.IsHandled;

			}

			return false;
		}

		public bool Execute(AnkhCommand command, CommandEventArgs e)
		{
			EnsureLoaded();
			CommandMapItem item;

			if (_map.TryGetValue(command, out item))
			{
				try
				{
					item.OnExecute(e);
				}
				catch (Exception ex)
				{
                    if (e.Context.ErrorHandler == null)
                        throw;

					e.Context.ErrorHandler.Handle(ex);
					return false;

				}

				return item.IsHandled;
			}

			return false;
		}

		/// <summary>
		/// Gets the <see cref="CommandMapItem"/> for the specified command
		/// </summary>
		/// <param name="command"></param>
		/// <returns>The <see cref="CommandMapItem"/> or null if the command is not valid</returns>
		public CommandMapItem this[AnkhCommand command]
		{
			get
			{
				CommandMapItem item;

				if (_map.TryGetValue(command, out item))
					return item;
				else if (command <= AnkhCommand.CommandFirst)
					return null;
				else if (Enum.IsDefined(typeof(AnkhCommand), command))
				{
					item = new CommandMapItem(command);

					_map.Add(command, item);

					return item;
				}
				else
					return null;
			}
		}

		bool _loaded;
		private void EnsureLoaded()
		{
			if (_loaded)
				return;

			_loaded = true;

			foreach (Type type in typeof(CommandMapper).Assembly.GetTypes())
			{
				if (!type.IsClass || type.IsAbstract)
					continue;

				if (!typeof(ICommand).IsAssignableFrom(type))
					continue;

				ICommand instance = null;

				foreach(VSNetCommandAttribute cmdAttr in type.GetCustomAttributes(typeof(VSNetCommandAttribute), false))
				{
					CommandMapItem item = this[cmdAttr.Command];

					if (item != null)
					{
						if (instance == null)
							instance = (ICommand)Activator.CreateInstance(type);

						Debug.Assert(item.ICommand == null || item.ICommand == instance, string.Format("No previous ICommand registered on the CommandMapItem for {0}", cmdAttr.Command));

						item.ICommand = instance; // hooks all events in compatibility mode
					}
				}

				foreach (VSNetControlAttribute ctrlAttr in type.GetCustomAttributes(typeof(VSNetControlAttribute), false))
				{
					CommandMapItem item = this[ctrlAttr.Command];

					if (item != null)
					{
						if (instance == null)
							instance = (ICommand)Activator.CreateInstance(type);

						Debug.Assert(item.ICommand == null || item.ICommand == instance, string.Format("No previous ICommand registered on the CommandMapItem for {0}", ctrlAttr.Command));

						item.ICommand = instance; // hooks all events in compatibility mode
					}
				}

			}

		}
	}
}
