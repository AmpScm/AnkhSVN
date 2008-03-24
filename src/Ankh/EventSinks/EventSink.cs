// $Id$
using System;
using System.Collections;
using EnvDTE;
using Microsoft.Win32;
using Microsoft;
using System.Threading;
//using Microsoft.VisualStudio.VCProjectEngine;

namespace Ankh.EventSinks
{
    /// <summary>
    /// Base class for event sink classes.
    /// </summary>
    public abstract class EventSink
    {
        protected EventSink(IAnkhServiceProvider context)
        {
            this.context = context;
        }

        /// <summary>
        /// Unhooks from the events.
        /// </summary>
        public abstract void Unhook();

        protected IAnkhServiceProvider Context
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this.context; }
        }

        private IAnkhServiceProvider context;
    }
}
