// $Id$
using System;
using System.Collections;
using EnvDTE;
using Microsoft.Win32;
using Microsoft;
using Interop.esproj;
using System.Threading;
//using Microsoft.VisualStudio.VCProjectEngine;

namespace Ankh.EventSinks
{
    /// <summary>
    /// Base class for event sink classes.
    /// </summary>
    public abstract class EventSink
    {
        protected EventSink( IContext context )
        {
            this.context = context;
        }

        /// <summary>
        /// Unhooks from the events.
        /// </summary>
        public abstract void Unhook();



        protected IContext Context
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.context; }
        }  

        /// <summary>
        /// Whether a VC++ project is currently being added. This property is
        /// used by the VCProjectEventSink to keep track of when a VC++ project is being 
        /// added and suppress file added events during that time.
        /// </summary>
        protected static bool AddingProject
        {
            get{ return addingProject; }
            set{ addingProject = value; }
        }        
      
       
        
        protected const int REFRESHDELAY = 800;
        private static bool addingProject = false;
        private IContext context;

    }
}
