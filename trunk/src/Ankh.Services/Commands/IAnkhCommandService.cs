using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Ids;
using System.ComponentModel.Design;

namespace Ankh.Commands
{
    public enum CommandPrompt
    {
        DoDefault,
        Always,
        Never
    }

    public delegate bool DelayDelegateCheck();

    public class CommandResult
    {
        readonly bool _success;
        readonly object _result;

        public CommandResult(bool success)
            : this(success, null)
        {
        }

        public CommandResult(bool success, object result)
        {
            _success = success;
            _result = result;            
        }

        public bool Success
        {
            get { return _success; }
        }

        public object Result
        {
            get { return _result; }
        }

        public static implicit operator bool(CommandResult r)
        {
            if (r == null)
                return false;

            return r.Success;
        }
    }

    public interface IAnkhCommandService
    {
        /// <summary>
        /// Shows a context menu at the specified location
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        void ShowContextMenu(AnkhCommandMenu menu, int x, int y);


        /// <summary>
        /// Shows a context menu at the specified location
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <param name="location">The location.</param>
        void ShowContextMenu(AnkhCommandMenu menu, System.Drawing.Point location);

        // ExecCommand has no args object because it would require a lot 
        // of custom interop code to make it work and there are far more 
        // efficient ways to call code than

        // The following methods should be called from the UI thread

        /// <summary>
        /// Executes the specified command synchronously
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        CommandResult ExecCommand(AnkhCommand command);

        /// <summary>
        /// Executes the specified command synchronously
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        CommandResult ExecCommand(AnkhCommand command, bool verifyEnabled);
        /// <summary>
        /// Executes the specified command synchronously
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        CommandResult ExecCommand(CommandID command);


        /// <summary>
        /// Executes the specified command synchronously
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="verifyEnabled">if set to <c>true</c> [verify enabled].</param>
        /// <returns></returns>
        CommandResult ExecCommand(CommandID command, bool verifyEnabled);

        /// <summary>
        /// Directly calls the ankh command handler.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        CommandResult DirectlyExecCommand(AnkhCommand command);

        /// <summary>
        /// Directly calls the ankh command handler.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        CommandResult DirectlyExecCommand(AnkhCommand command, object args);

        /// <summary>
        /// Directly calls the ankh command handler.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        CommandResult DirectlyExecCommand(AnkhCommand command, object args, CommandPrompt prompt);
        

        // These methods can be called from the UI or a background thread
        /// <summary>
        /// Posts the command to the command queue
        /// </summary>
        /// <param name="command">The command.</param>
        bool PostExecCommand(AnkhCommand command);
        /// <summary>
        /// Posts the command to the command queue
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="args">The args.</param>
        bool PostExecCommand(AnkhCommand command, object args);


        /// <summary>
        /// Posts the command to the command queue
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="args">The args.</param>
        /// <param name="prompt">The prompt.</param>
        /// <returns></returns>
        bool PostExecCommand(AnkhCommand command, object args, CommandPrompt prompt);

        /// <summary>
        /// Posts the command to the command queue
        /// </summary>
        /// <param name="command">The command.</param>
        bool PostExecCommand(CommandID command);
        /// <summary>
        /// Posts the command to the command queue
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="args">The args.</param>
        bool PostExecCommand(CommandID command, object args);

        /// <summary>
        /// Posts the command to the command queue
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="args">The args.</param>
        /// <param name="prompt">The prompt.</param>
        /// <returns></returns>
        bool PostExecCommand(CommandID command, object args, CommandPrompt prompt);

        // And those from the UI thread
        /// <summary>
        /// Updates the command UI.
        /// </summary>
        /// <param name="performImmediately">if set to <c>true</c> [perform immediately].</param>
        void UpdateCommandUI(bool performImmediately);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="check"></param>
        void DelayPostCommands(DelayDelegateCheck check);
    }
}
