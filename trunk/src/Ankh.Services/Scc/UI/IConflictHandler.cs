using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using System.ComponentModel;

namespace Ankh.Scc
{
    public interface IConflictHandler
    {
        /// <summary>
        /// Registers the default interactive conflict handler on the specified args object
        /// </summary>
        /// <param name="args">The args object to register on</param>
        /// <param name="synch">The synchronization object to use or null if synchronization to the UI thread is not necessary</param>
        void RegisterConflictHandler(SvnClientArgsWithConflict args, ISynchronizeInvoke synch);        
    }
}
