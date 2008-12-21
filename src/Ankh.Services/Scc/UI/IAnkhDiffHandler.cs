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
using System.Diagnostics;

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

    /// <summary>
    /// A template in the dialog above.
    /// </summary>
    public class AnkhDiffArgumentDefinition
    {
        readonly string _key;
        readonly string[] _aliases;
        readonly string _description;

        public AnkhDiffArgumentDefinition(string key, string description, params string[] aliases)
        {
            _key = key;
            _description = description;
            _aliases = aliases ?? new string[0];
        }

        public AnkhDiffArgumentDefinition(string key, string description)
            : this(key, description, (string[])null)
        {
        }

        public string Key
        {
            get { return _key; }
        }

        public string Description
        {
            get { return _description; }
        }

        public string[] Aliases
        {
            get { return _aliases; }
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

    [DebuggerDisplay("{Name} ({Title})")]
    public abstract class AnkhDiffTool
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public abstract string Title
        {
            get;
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get { return string.Format("{0}{1}", Title, IsAvailable ? "" : " (Not Found)"); }
        }

        /// <summary>
        /// Gets the tool template.
        /// </summary>
        /// <value>The tool template.</value>
        public string ToolTemplate
        {
            get { return string.Format("$(AppTemplate({0}))", Name); }
        }

        /// <summary>
        /// Gets the tool name from template.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string GetToolNameFromTemplate(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if (value.StartsWith("$(AppTemplate(", StringComparison.OrdinalIgnoreCase) &&
                value.EndsWith("))"))
                return value.Substring(14, value.Length - 16);

            return null;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is available.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is available; otherwise, <c>false</c>.
        /// </value>
        public abstract bool IsAvailable
        {
            get;
        }

        /// <summary>
        /// Gets the program.
        /// </summary>
        /// <value>The program.</value>
        public abstract string Program
        {
            get;
        }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public abstract string Arguments
        {
            get;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return Title ?? base.ToString();
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

        /// <summary>
        /// Gets a list of diff tool templates.
        /// </summary>
        /// <returns></returns>
        IList<AnkhDiffTool> DiffToolTemplates { get; }
        /// <summary>
        /// Gets a list of merge tool templates.
        /// </summary>
        /// <returns></returns>
        IList<AnkhDiffTool> MergeToolTemplates { get; }
        /// <summary>
        /// Gets a list of patch tools.
        /// </summary>
        /// <returns></returns>
        IList<AnkhDiffTool> PatchToolTemplates { get; }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        IList<AnkhDiffArgumentDefinition> ArgumentDefinitions { get; }

        /// <summary>
        /// Gets the copy origin.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        SvnUriTarget GetCopyOrigin(SvnItem item);
    }
}
