using System;
using System.Collections.Generic;
using System.Text;
using AnkhSvn.Ids;
using Ankh.Selection;

namespace Ankh.Commands
{
    public class BaseCommandEventArgs : EventArgs
    {
        readonly AnkhCommand _command;
        readonly AnkhContext _context;

        public BaseCommandEventArgs(AnkhCommand command, AnkhContext context)
        {
            _command = command;
            _context = context;
        }

        public AnkhCommand Command
        {
            get { return _command; }
        }

        public AnkhContext Context
        {
            get { return _context; }
        }

        /// <summary>
        /// Gets the Visual Studio selection
        /// </summary>
        /// <value>The selection.</value>
        public ISelectionContext Selection
        {
            get { return _context.GetService<ISelectionContext>(); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is in automation.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is in automation; otherwise, <c>false</c>.
        /// </value>
        public bool IsInAutomation
        {
            get 
            {
                IAnkhRuntimeInfo runtimeInfo = Context.GetService<IAnkhRuntimeInfo>();

                if (runtimeInfo != null)
                    return runtimeInfo.IsInAutomation;
                else
                    return false;
            }
        }
    }
}
