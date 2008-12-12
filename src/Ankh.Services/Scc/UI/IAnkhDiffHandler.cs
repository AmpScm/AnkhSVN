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
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using SharpSvn;

namespace Ankh.Scc.UI
{
    [Flags]
    public enum DiffMode
    {
        Default=0,
        PreferExternal=1,
        PreferInternal=2
    }

    public class AnkhDiffArgs 
    {
        DiffMode _diffMode;
        string _baseFile;
        string _baseTitle;

        string _mineFile;
        string _mineTitle;

        public DiffMode Mode
        {
            get { return _diffMode; }
            set { _diffMode = value; }
        }

        public string BaseFile
        {
            get { return _baseFile; }
            set { _baseFile = value; }
        }

        public string MineFile
        {
            get { return _mineFile; }
            set { _mineFile = value; }
        }

        public string BaseTitle
        {
            get { return _baseTitle; }
            set { _baseTitle = value; }
        }

        public string MineTitle
        {
            get { return _mineTitle; }
            set { _mineTitle = value; }
        }

        public virtual bool Validate()
        {
            return !string.IsNullOrEmpty(BaseFile) && !string.IsNullOrEmpty(MineFile);
        }


    }

    public class AnkhMergeArgs : AnkhDiffArgs
    {
        string _theirsFile;
        string _theirsTitle;
        string _mergedFile;
        string _mergedTitle;

        public string TheirsFile
        {
            get { return _theirsFile; }
            set { _theirsFile = value; }
        }

        public string TheirsTitle
        {
            get { return _theirsTitle; }
            set { _theirsTitle = value; }
        }
        
        public string MergedFile
        {
            get { return _mergedFile; }
            set { _mergedFile = value; }
        }

        public string MergedTitle
        {
            get { return _mergedTitle; }
            set { _mergedTitle = value; }
        }

        public override bool Validate()
        {
            return base.Validate() && !string.IsNullOrEmpty(TheirsFile) && !string.IsNullOrEmpty(MergedFile);
        }
    }

    public interface IAnkhDiffHandler
    {
        bool RunDiff(AnkhDiffArgs args);
        bool RunMerge(AnkhMergeArgs args);

        /// <summary>
        /// Releases the diff.
        /// </summary>
        /// <param name="frameNumber">The frame number.</param>
        void ReleaseDiff(int frameNumber);

        /// <summary>
        /// Gets the temp file.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="revision">The revision.</param>
        /// <param name="withProgress">if set to <c>true</c> [with progress].</param>
        /// <returns></returns>
        string GetTempFile(SvnTarget target, SvnRevision revision, bool withProgress);
        /// <summary>
        /// Gets the temp file.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="revision">The revision.</param>
        /// <param name="withProgress">if set to <c>true</c> [with progress].</param>
        /// <returns></returns>
        string GetTempFile(SvnItem target, SvnRevision revision, bool withProgress);
        string[] GetTempFiles(SvnTarget target, SvnRevision first, SvnRevision last, bool withProgress);
        string GetTitle(SvnTarget target, SvnRevision revision);
        string GetTitle(SvnItem target, SvnRevision revision);

        SvnUriTarget GetCopyOrigin(SvnItem item);


    }
}
