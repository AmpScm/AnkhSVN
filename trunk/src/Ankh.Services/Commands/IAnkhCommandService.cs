﻿using System;
using System.Collections.Generic;
using System.Text;
using AnkhSvn.Ids;
using System.ComponentModel.Design;

namespace Ankh.Commands
{
    public interface IAnkhCommandService
    {
        // ExecCommand has no args object because it would require a lot 
        // of custom interop code to make it work and there are far more 
        // efficient ways to call code than

        // The following methods should be called from the UI thread
        /// <summary>
        /// Executes the specified command synchronously
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        bool ExecCommand(AnkhCommand command);
        /// <summary>
        /// Executes the specified command synchronously
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        bool ExecCommand(CommandID command);


        /// <summary>
        /// Executes the specified command synchronously
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="verifyEnabled">if set to <c>true</c> [verify enabled].</param>
        /// <returns></returns>
        bool ExecCommand(CommandID command, bool verifyEnabled);

        /// <summary>
        /// Directly calls the ankh command handler.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        bool DirectlyExecCommand(AnkhCommand command, object args);

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
        bool PostExecCommand(CommandID command);
        /// <summary>
        /// Posts the command to the command queue
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="args">The args.</param>
        bool PostExecCommand(CommandID command, object args);
    }
}
