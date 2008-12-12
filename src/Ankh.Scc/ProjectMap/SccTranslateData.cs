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
using System.Diagnostics;
using SharpSvn;

namespace Ankh.Scc.ProjectMap
{
    class SccTranslateData
    {
        readonly AnkhSccProvider _provider;
        readonly Guid _projectId;
        SccProjectData _project;
        bool _disposed;
        string _storedPath;
        string _sccPath;

        public SccTranslateData(AnkhSccProvider provider, Guid projectId)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            _provider = provider;
            _projectId = projectId;
        }

        internal virtual void Dispose()
        {
            _disposed = true;
        }

        [DebuggerStepThrough]
        T GetService<T>()
            where T : class
        {
            return ((IAnkhServiceProvider)_provider).GetService<T>();
        }

        [DebuggerStepThrough]
        T GetService<T>(Type serviceType)
            where T : class
        {
            return ((IAnkhServiceProvider)_provider).GetService<T>(serviceType);
        }

        public SccProjectData Project
        {
            get
            {
                if (_project != null && !_project.IsDisposed)
                    return _project;

                return _project = _provider.GetSccProject(_projectId);
            }
        }

        /// <summary>
        /// Gets or sets the path used for storing the project in the solution
        /// </summary>
        /// <value>The name of the stored path.</value>
        public string StoredPathName
        {
            get { return _storedPath; }
            set
            {
                if (_storedPath == value)
                    return;

                Debug.Assert(!_disposed, "Not disposed");

                string path = SvnTools.GetNormalizedFullPath(value);

                if (_provider.Translate_SetStoredPath(this, path))
                    _storedPath = path;
            }
        }

        /// <summary>
        /// Gets or sets the name of the SCC path; the path as used to access the project on disk
        /// </summary>
        /// <value>The name of the SCC path.</value>
        public string SccPathName
        {
            get { return _sccPath; }
            set
            {
                if (_sccPath == value)
                    return;

                Debug.Assert(!_disposed, "Not disposed");

                string path = SvnTools.GetNormalizedFullPath(value);

                if (_provider.Translate_SetSccPath(this, path))
                    _sccPath = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the enlist path.
        /// </summary>
        /// <value>The name of the enlist path.</value>
        public virtual string EnlistPathName
        {
            get { return null; }
            set { throw new InvalidOperationException(); }
        }

        /// <summary>
        /// Gets a boolean indicating whether translation data should be stored in the solution
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldSerializeInSolution()
        {
            return true;
        }

        /// <summary>
        /// Gets a boolean indicating whether translation data should be stored in the user settings
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldSerializeInUserSettings()
        {
            return true;
        }        
    }
}
