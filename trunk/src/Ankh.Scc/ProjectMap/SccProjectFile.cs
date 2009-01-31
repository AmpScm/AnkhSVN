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
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;

namespace Ankh.Scc.ProjectMap
{
    /// <summary>
    /// A file contained one or more times in one or more Scc projects
    /// </summary>
    [DebuggerDisplay("{FullPath}, Projects={ProjectCount}, References={TotalReferenceCount}")]
    sealed class SccProjectFile
    {
        readonly IAnkhServiceProvider _context;
        readonly string _filename;
        SccProjectFileReference _firstReference;        

        /// <summary>
        /// Initializes a new instance of the <see cref="SccProjectFile"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="filename">The filename.</param>
        public SccProjectFile(IAnkhServiceProvider context, string filename)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            _context = context;
            _filename = filename;

            _context.GetService<IFileStatusCache>().SetSolutionContained(FullPath, true);
        }

        /// <summary>
        /// Gets the filename.
        /// </summary>
        /// <value>The filename.</value>
        public string FullPath
        {
            get { return _filename; }
        }

        /// <summary>
        /// Gets the number of projects containing this file
        /// </summary>
        public int ProjectCount
        {
            get
            {
                SccProjectFileReference rf = FirstReference;
                int i = 0;

                while (rf != null)
                {
                    i++;

                    rf = rf.NextReference;                    
                }

                return i;
            }
        }

        public bool IsProjectFile
        {
            get
            {
                SccProjectFileReference rf = FirstReference;

                while (rf != null)
                {
                    if (rf.IsProjectFile)
                        return true;

                    rf = rf.NextReference;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the total number of references to this file
        /// </summary>
        public int TotalReferenceCount
        {
            get
            {
                SccProjectFileReference rf = FirstReference;
                int i = 0;

                while (rf != null)
                {
                    i += rf.ReferenceCount;
                    rf = rf.NextReference;                    
                }

                return i;
            }
        }

        /// <summary>
        /// Gets the owner projects.
        /// </summary>
        /// <returns></returns>
        public IList<SccProjectData> GetOwnerProjects()
        {
            List<SccProjectData> pd = new List<SccProjectData>();

            SccProjectFileReference rf = FirstReference;

            while (rf != null)
            {
                pd.Add(rf.Project);

                rf = rf.NextReference;
            }

            return pd;
        }

        public IList<SccProjectFileReference> GetAllReferences()
        {
            List<SccProjectFileReference> refs = new List<SccProjectFileReference>();

            SccProjectFileReference rf = FirstReference;

            while (rf != null)
            {
                refs.Add(rf);

                rf = rf.NextReference;
            }

            return refs;
        }

        /// <summary>
        /// Gets the first project reference to this file
        /// </summary>
        /// <value>The first reference.</value>
        public SccProjectFileReference FirstReference
        {
            get { return _firstReference; }
        }

        #region Linked list management
        internal void AddReference(SccProjectFileReference reference)
        {
            if (reference == null)
                throw new ArgumentNullException("reference");

            if (reference.NextReference != null || reference.ProjectFile != this)
                throw new InvalidOperationException(); 

            reference._nextReference = FirstReference;
            _firstReference = reference;
        }

        /// <summary>
        /// Removes a reference from the references linked list
        /// </summary>
        /// <param name="reference"></param>
        internal void RemoveReference(SccProjectFileReference reference)
        {
            if (reference == null)
                throw new ArgumentNullException("reference");

            if (reference == FirstReference)
            {
                _firstReference = reference.NextReference;
                reference._nextReference = null;

                if (_firstReference == null)
                {
                    _context.GetService<IFileStatusCache>().SetSolutionContained(this.FullPath, false);
                    _context.GetService<AnkhSccProvider>().RemoveFile(this);
                }
                return;
            }

            SccProjectFileReference rf = FirstReference;

            while (rf != null)
            {
                if (rf.NextReference == reference)
                {
                    rf._nextReference = reference.NextReference;
                    reference._nextReference = null;
                    return;
                }

                rf = rf.NextReference;
            }

            throw new InvalidOperationException("Reference list invalid; scc project mapping corrupted; please reopen the solution");
        }
        #endregion
    }
}
