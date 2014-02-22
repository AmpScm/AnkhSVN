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
using System.Diagnostics;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.VS.Selection
{
    sealed class SelectionItem : IEquatable<SelectionItem>
    {
        readonly IVsHierarchy _hierarchy;
        IVsSccProject2 _sccProject;
        readonly uint _id;
        bool _checkedSccProject;

        public SelectionItem(IVsHierarchy hierarchy, uint id)
        {
            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");

            _hierarchy = hierarchy;
            _id = id;
        }

        public SelectionItem(IVsHierarchy hierarchy, uint id, IVsSccProject2 project)
            : this(hierarchy, id)
        {
            _sccProject = project;
        }

        public IVsHierarchy Hierarchy
        {
            [DebuggerStepThrough]
            get { return _hierarchy; }
        }

        public IVsSccProject2 SccProject
        {
            get
            {
                if (_sccProject == null && !_checkedSccProject)
                {
                    _checkedSccProject = true;
                    _sccProject = _hierarchy as IVsSccProject2;
                }

                return _sccProject;
            }
        }

        public uint Id
        {
            get { return _id; }
        }

        public bool IsSolution
        {
            get { return (SccProject != null) && SelectionUtils.IsSolutionSccProject(SccProject); }
        }

        #region IEquatable<SelectionItem> Members

        public bool Equals(SelectionItem other)
        {
            if (other == null)
                return false;

            return (other.Id == Id) && (other.Hierarchy == Hierarchy);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SelectionItem);
        }

        public override int GetHashCode()
        {
            return _hierarchy.GetHashCode() ^ _id.GetHashCode();
        }

        #endregion
    }
}
