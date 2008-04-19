﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;

namespace Ankh.Scc.ProjectMap
{
    /// <summary>
    /// A file contained one or more times in one or more Scc projects
    /// </summary>
    [DebuggerDisplay("{Filename}, Projects={ProjectCount}, References={TotalReferenceCount}")]
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

            ((ISvnItemStateUpdate)_context.GetService<IFileStatusCache>()[this.FullPath]).SetSolutionContained(true);
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
                SccProjectFileReference rf = _firstReference;
                int i = 0;

                while (rf != null)
                {
                    i++;

                    rf = rf._nextReference;                    
                }

                return i;
            }
        }

        /// <summary>
        /// Gets the total number of references to this file
        /// </summary>
        public int TotalReferenceCount
        {
            get
            {
                SccProjectFileReference rf = _firstReference;
                int i = 0;

                while (rf != null)
                {
                    i += rf.ReferenceCount;
                    rf = rf._nextReference;                    
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

            SccProjectFileReference rf = _firstReference;

            while (rf != null)
            {
                pd.Add(rf.Project);

                rf = rf._nextReference;
            }

            return pd;
        }

        public IList<SccProjectFileReference> GetAllReferences()
        {
            List<SccProjectFileReference> refs = new List<SccProjectFileReference>();

            SccProjectFileReference rf = _firstReference;

            while (rf != null)
            {
                refs.Add(rf);

                rf = rf._nextReference;
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

            if (reference._nextReference != null || reference.ProjectFile != this)
                throw new InvalidOperationException(); 

            reference._nextReference = _firstReference;
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

            if (reference == _firstReference)
            {
                _firstReference = reference._nextReference;
                reference._nextReference = null;

                if (_firstReference == null)
                {
                    ((ISvnItemStateUpdate)_context.GetService<IFileStatusCache>()[this.FullPath]).SetSolutionContained(false);
                    _context.GetService<AnkhSccProvider>().RemoveFile(this);
                }
                return;
            }

            SccProjectFileReference rf = _firstReference;

            while (rf != null)
            {
                if (rf._nextReference == reference)
                {
                    rf._nextReference = reference._nextReference;
                    reference._nextReference = null;
                    return;
                }

                rf = rf._nextReference;
            }

            throw new InvalidOperationException("Reference list invalid; scc project mapping corrupted; please reopen the solution");
        }
        #endregion
    }
}
