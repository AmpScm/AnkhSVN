// $Id$

using EnvDTE;

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
        vsCommandStatus QueryStatus( AnkhContext context );

        /// <summary>
        /// Execute the command
        /// </summary>
        void Execute( AnkhContext context, string parameters );

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



