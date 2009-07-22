// $Id$
//
// Copyright 2008 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

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

        /// <summary>
        /// Posts the tick command.
        /// </summary>
        /// <param name="tick">if set to <c>true</c> [tick].</param>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        bool PostTickCommand(ref bool tick, AnkhCommand command);

        /// <summary>
        /// Safely posts a tick command
        /// </summary>
        /// <param name="tick">if set to <c>true</c> [tick].</param>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        void SafePostTickCommand(ref bool tick, AnkhCommand command);

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

        void TockCommand(AnkhCommand ankhCommand);
    }
}
