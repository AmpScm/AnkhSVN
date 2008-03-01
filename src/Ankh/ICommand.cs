using EnvDTE;
using Ankh.Commands;

namespace Ankh
{
	/// <summary>
	/// Represents an Ankh command.
	/// </summary>
	public interface ICommand
	{
		/// <summary>
		/// Get the status of the command
		/// </summary>
        void OnUpdate(CommandUpdateEventArgs e);

		/// <summary>
		/// Execute the command
		/// </summary>
        void OnExecute(CommandEventArgs e);                     

		/// <summary>
		/// The EnvDTE.Command instance corresponding to this command.
		/// </summary>
		EnvDTE.Command Command
		{
			get;
			set;
		}
	}
}
