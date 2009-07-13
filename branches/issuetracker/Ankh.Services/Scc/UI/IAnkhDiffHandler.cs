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
        Default = 0,
        PreferExternal = 1,
        PreferInternal = 2
    }

    public abstract class AnkhDiffToolArgs
    {
        DiffMode _diffMode;

        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        /// <value>The mode.</value>
        public DiffMode Mode
        {
            get { return _diffMode; }
            set { _diffMode = value; }
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        public abstract bool Validate();
    }

    public class AnkhDiffArgs : AnkhDiffToolArgs
    {
        string _baseFile;
        string _baseTitle;

        string _mineFile;
        string _mineTitle;
        bool _readOnly;

        /// <summary>
        /// Gets or sets the base file.
        /// </summary>
        /// <value>The base file.</value>
        public string BaseFile
        {
            get { return _baseFile; }
            set { _baseFile = value; }
        }

        /// <summary>
        /// Gets or sets the mine file.
        /// </summary>
        /// <value>The mine file.</value>
        public string MineFile
        {
            get { return _mineFile; }
            set { _mineFile = value; }
        }

        /// <summary>
        /// Gets or sets the base title.
        /// </summary>
        /// <value>The base title.</value>
        public string BaseTitle
        {
            get { return _baseTitle; }
            set { _baseTitle = value; }
        }

        /// <summary>
        /// Gets or sets the mine title.
        /// </summary>
        /// <value>The mine title.</value>
        public string MineTitle
        {
            get { return _mineTitle; }
            set { _mineTitle = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the diff/merge should be presented as read only.
        /// </summary>
        /// <value><c>true</c> if [read only]; otherwise, <c>false</c>.</value>
        public bool ReadOnly
        {
            get { return _readOnly; }
            set { _readOnly = value; }
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Validate()
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

        /// <summary>
        /// Gets or sets the theirs file.
        /// </summary>
        /// <value>The theirs file.</value>
        public string TheirsFile
        {
            get { return _theirsFile; }
            set { _theirsFile = value; }
        }

        /// <summary>
        /// Gets or sets the theirs title.
        /// </summary>
        /// <value>The theirs title.</value>
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

    /// <summary>
    /// 
    /// </summary>
    public class AnkhPatchArgs : AnkhDiffToolArgs
    {
        string _patchFile;
        string _applyTo;

        /// <summary>
        /// Gets or sets the patch file.
        /// </summary>
        /// <value>The patch file.</value>
        public string PatchFile
        {
            get { return _patchFile; }
            set { _patchFile = value; }
        }

        /// <summary>
        /// Gets or sets the apply to.
        /// </summary>
        /// <value>The apply to.</value>
        public string ApplyTo
        {
            get { return _applyTo; }
            set { _applyTo = value; }
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        public override bool Validate()
        {
            return !string.IsNullOrEmpty(PatchFile) && !string.IsNullOrEmpty(ApplyTo);
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

        /// <summary>
        /// Initializes a new instance of the <see cref="AnkhDiffArgumentDefinition"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="description">The description.</param>
        /// <param name="aliases">The aliases.</param>
        public AnkhDiffArgumentDefinition(string key, string description, params string[] aliases)
        {
            _key = key;
            _description = description;
            _aliases = aliases ?? new string[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnkhDiffArgumentDefinition"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="description">The description.</param>
        public AnkhDiffArgumentDefinition(string key, string description)
            : this(key, description, (string[])null)
        {
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key
        {
            get { return _key; }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// Gets the aliases.
        /// </summary>
        /// <value>The aliases.</value>
        public string[] Aliases
        {
            get { return _aliases; }
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
        /// <summary>
        /// Runs the diff as specified by the args
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        bool RunDiff(AnkhDiffArgs args);
        /// <summary>
        /// Runs the merge as specified by the args
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        bool RunMerge(AnkhMergeArgs args);
        /// <summary>
        /// Runs the patch as specified by the args
        /// </summary>
        /// <param name="args">The args.</param>
        bool RunPatch(AnkhPatchArgs args);

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
