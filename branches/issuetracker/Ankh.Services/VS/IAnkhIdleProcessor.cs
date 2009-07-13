// $Id$
//
// Copyright 2009 The AnkhSVN Project
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
using Microsoft.VisualStudio.OLE.Interop;

namespace Ankh.VS
{
    /// <summary>
    /// 
    /// </summary>
    public class AnkhIdleArgs : EventArgs
    {
        readonly bool _periodic;
        readonly bool _nonPeriodic;
        readonly bool _priority;
        readonly IAnkhServiceProvider _context;
        IOleComponentManager _mgr;
        bool _done;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnkhIdleArgs"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="idleFlags">The idle flags.</param>
        public AnkhIdleArgs(IAnkhServiceProvider context, int idleFlags)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            _context = context;
            _periodic = (0 != (idleFlags & (int)_OLEIDLEF.oleidlefPeriodic));
            _nonPeriodic = (0 != (idleFlags & (int)_OLEIDLEF.oleidlefNonPeriodic));
            _priority = (0 != (idleFlags & (int)_OLEIDLEF.oleidlefPriority));
        }

        /// <summary>
        /// Gets a value indicating whether periodic idle tasks should be processed
        /// </summary>
        /// <value><c>true</c> if periodic; otherwise, <c>false</c>.</value>
        /// <remarks>AnkhSVN receives periodic idle events at least every second; see AnkhSvnPackage.RegisterAsComponent()</remarks>
        public bool Periodic
        {
            get { return _periodic; }
        }

        /// <summary>
        /// Gets a value indicating whether non periodic idle tasks should be processed
        /// </summary>
        /// <value><c>true</c> if [non periodic]; otherwise, <c>false</c>.</value>
        public bool NonPeriodic
        {
            get { return _nonPeriodic; }
        }

        /// <summary>
        /// Gets a value indicating whether only priority tasks should be processed
        /// </summary>
        /// <value><c>true</c> if priority; otherwise, <c>false</c>.</value>
        public bool Priority
        {
            get { return _priority; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this idle processing task is done
        /// </summary>
        /// <value><c>true</c> if done; otherwise, <c>false</c>.</value>
        public bool Done
        {
            get { return _done; }
            set { _done = value; }
        }

        /// <summary>
        /// Gets a boolean indicating whether a component can continue idle processing. Returns false when idle processing should stop
        /// </summary>
        /// <returns></returns>
        public bool ContinueIdle()
        {
            if(_mgr == null)
                _mgr = _context.GetService<IOleComponentManager>(typeof(SOleComponentManager));

            return 0 != _mgr.FContinueIdle();
        }
    }

    public interface IAnkhIdleProcessor
    {
        void OnIdle(AnkhIdleArgs e);
    }
}
