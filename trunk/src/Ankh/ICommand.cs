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
        vsCommandStatus QueryStatus( _DTE dte );

        /// <summary>
        /// Execute the command
        /// </summary>
        void Execute( _DTE dte );
    }
}



