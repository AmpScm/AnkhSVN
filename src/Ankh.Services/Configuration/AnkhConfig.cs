// $Id$
//
// Copyright 2003-2009 The AnkhSVN Project
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
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using Microsoft.Win32;
using System.Security.AccessControl;
using System.Collections.Generic;
namespace Ankh.Configuration
{
    /// <summary>
    /// Ankh configuration container. Read and written via <see cref="IAnkhConfigurationService"/>
    /// </summary>
    public class AnkhConfig
    {
        string _mergeExePath;
        string _diffExePath;
        string _patchExePath;
        bool _interactiveMergeOnConflict;
        bool _flashWindow;
        bool _autoAddEnabled;
        bool _autoLockEnabled;
        bool _noDashComment;
        bool _pcDoubleClickShowsChanges;
        int _recentChangesRefreshInterval;
        bool _disableUpdateCheck;
        bool _enableTsvnHooks;
        bool _dontHookSlnRefresh;
        bool _floatDiffEditors;
        bool _useExternalWebbrowser;
        bool _preferPuttySsh;
        public List<ExtToolDefinition> _mergeExePaths = new List<ExtToolDefinition>();
        public List<ExtToolDefinition> _diffExePaths = new List<ExtToolDefinition>();

        /// <summary>
        /// Gets or sets the merge exe path.
        /// </summary>
        /// <value>The merge exe path.</value>
        [DefaultValue(null)]
        public List<ExtToolDefinition> MergeExePaths
        {
            get { return _mergeExePaths; }
            set { _mergeExePaths = value; }
        }

        /// <summary>
        /// Gets or sets the diff exe path.
        /// </summary>
        /// <value>The diff exe path.</value>
        [DefaultValue(null)]
        public List<ExtToolDefinition> DiffExePaths
        {
            get { return _diffExePaths; }
            set { _diffExePaths = value; }
        }
                
        /// <summary>
        /// Gets or sets the merge exe path.
        /// </summary>
        /// <value>The merge exe path.</value>
        [DefaultValue(null)]
        public string MergeExePath
        {
            get { return _mergeExePath; }
            set { _mergeExePath = value; }
        }

        /// <summary>
        /// Gets or sets the diff exe path.
        /// </summary>
        /// <value>The diff exe path.</value>
        [DefaultValue(null)]
        public string DiffExePath
        {
            get { return _diffExePath; }
            set { _diffExePath = value; }
        }

        /// <summary>
        /// Gets or sets the patch exe path.
        /// </summary>
        /// <value>The patch exe path.</value>
        [DefaultValue(null)]
        public string PatchExePath
        {
            get { return _patchExePath; }
            set { _patchExePath = value; }
        }

        [DefaultValue(false)]
        public bool InteractiveMergeOnConflict
        {
            get { return _interactiveMergeOnConflict; }
            set { _interactiveMergeOnConflict = value; }
        }

        [DefaultValue(false)]
        public bool FlashWindowWhenOperationCompletes
        {
            get { return _flashWindow; }
            set { _flashWindow = value; }
        }

        [DefaultValue(false)]
        public bool AutoAddEnabled
        {
            get { return _autoAddEnabled; }
            set { _autoAddEnabled = value; }
        }

        [DefaultValue(false)]
        public bool SuppressLockingUI
        {
            get { return _autoLockEnabled; }
            set { _autoLockEnabled = value; }
        }

        [DefaultValue(false)]
        public bool DisableDashInLogComment
        {
            get { return _noDashComment; }
            set { _noDashComment = value; }
        }

        [DefaultValue(false)]
        public bool PCDoubleClickShowsChanges
        {
            get { return _pcDoubleClickShowsChanges; }
            set { _pcDoubleClickShowsChanges = value; }
        }

        /// <summary>
        /// Gets or sets the Recent Changes auto-refresh interval in seconds
        /// </summary>
        [DefaultValue(0)]
        public int RecentChangesRefreshInterval
        {
            get { return _recentChangesRefreshInterval; }
            set { _recentChangesRefreshInterval = value; }
        }

        [DefaultValue(false)]
        public bool DisableUpdateCheck
        {
            get { return _disableUpdateCheck; }
            set { _disableUpdateCheck = value; }
        }

        [DefaultValue(false)]
        public bool EnableTortoiseSvnHooks
        {
            get { return _enableTsvnHooks; }
            set { _enableTsvnHooks = value; }
        }

        [DefaultValue(false)]
        public bool DontHookSolutionExplorerRefresh
        {
            get { return _dontHookSlnRefresh; }
            set { _dontHookSlnRefresh = value; }
        }

        [DefaultValue(false)]
        public bool FloatDiffEditors
        { 
            get { return _floatDiffEditors; }
            set { _floatDiffEditors = value; }
        }

        [DefaultValue(false)]
        public bool ForceExternalBrowser
        {
            get { return _useExternalWebbrowser; }
            set { _useExternalWebbrowser = value; }
        }

        [DefaultValue(false)]
        public bool PreferPuttyAsSSH
        {
            get { return _preferPuttySsh; }
            set { _preferPuttySsh = value; }
        }
        public string GetMergeExePath(string filename)
        {
            foreach (ExtToolDefinition ext in _mergeExePaths)
            {
                if (filename.ToLower().Contains(ext.extension.ToLower()))
                {
                    return ext.exePath;
                }
            }
            return _mergeExePath;
        }

        public string GetDiffExePath(string filename)
        {
            foreach (ExtToolDefinition ext in _diffExePaths)
            {
                if (filename.ToLower().Contains(ext.extension.ToLower()))
                {
                    return ext.exePath;
                }
            }
            return _diffExePath;
        }

    }

    public class ExtToolDefinition
    {
        public string extension;
        public string exePath;
    }
}
