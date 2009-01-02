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
using SharpSvn;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using Ankh.Scc.UI;
using System.IO;

namespace Ankh.UI.MergeWizard
{
    public class MergeConflictHandler : AnkhService
    {  
        /// Conflict resolution preference for binary files
        SvnAccept _binaryChoice = SvnAccept.Postpone;

        /// Conflict resolution preference for text files
        SvnAccept _textChoice = SvnAccept.Postpone;

        /// Conflict resolution preference for properties
        SvnAccept _propertyChoice = SvnAccept.Postpone;

        /// flag (not) to show conflict resolution option dialog for text files
        bool _txt_showDialog/* = false*/;

        /// flag (not) to show conflict resolution option dialog for binary files
        bool _binary_showDialog/* = false*/;

        /// flag (not) to show conflict resolution option dialog for property files
        bool _property_showDialog = true; // prompt for properties initially

        List<string> currentResolutions = new List<string>();
        Dictionary<string, List<SvnConflictType>> _resolvedMergeConflicts = new Dictionary<string, List<SvnConflictType>>();

        public MergeConflictHandler(IAnkhServiceProvider context, SvnAccept binaryChoice, SvnAccept textChoice, SvnAccept propChoice)
            : this(context, binaryChoice, textChoice)
        {
        }
        
        public MergeConflictHandler(IAnkhServiceProvider context, SvnAccept binaryChoice, SvnAccept textChoice)
            : this(context)
        {
            this._binaryChoice = binaryChoice;
            this._textChoice = textChoice;
        }

        public MergeConflictHandler(IAnkhServiceProvider context)
            : base(context)
        {
        }

        /// <summary>
        /// Gets/sets the conflict resolution preference for text files
        /// </summary>
        public SvnAccept TextConflictResolutionChoice
        {
            get
            {
                return this._textChoice;
            }
            set
            {
                this._textChoice = value;
            }
        }

        /// <summary>
        /// Gets/sets the conflict resolution preference for binary files
        /// </summary>
        public SvnAccept BinaryConflictResolutionChoice
        {
            get
            {
                return this._binaryChoice;
            }
            set
            {
                this._binaryChoice = value;
            }
        }

        /// <summary>
        /// Gets/sets the conflict resolution preference for properties
        /// </summary>
        public SvnAccept PropertyConflictResolutionChoice
        {
            get
            {
                return this._propertyChoice;
            }
            set
            {
                this._propertyChoice = value;
            }
        }

        /// <summary>
        /// Gets/sets the flag to show conflict resolution dialog for text file conflicts.
        /// </summary>
        public bool PromptOnTextConflict
        {
            get
            {
                return this._txt_showDialog;
            }
            set
            {
                this._txt_showDialog = value;
            }
        }

        /// <summary>
        /// Gets/sets the flag to show conflict resolution dialog for binary file conflicts.
        /// </summary>
        public bool PromptOnBinaryConflict
        {
            get
            {
                return this._binary_showDialog;
            }
            set
            {
                this._binary_showDialog = value;
            }
        }

        /// <summary>
        /// Gets/sets the flag to show conflict resolution dialog for property conflicts.
        /// </summary>
        public bool PromptOnPropertyConflict
        {
            get
            {
                return this._property_showDialog;
            }
            set
            {
                this._property_showDialog = value;
            }
        }

        /// <summary>
        /// Gets the dictionary of resolved conflicts.
        /// key: file path
        /// value: list of conflict types
        /// </summary>
        public Dictionary<string, List<SvnConflictType>> ResolvedMergedConflicts
        {
            get
            {
                return this._resolvedMergeConflicts;
            }
        }

        /// <summary>
        /// Resets the handler's cache.
        /// </summary>
        public void Reset()
        {
            // reset current resolutions
            this._resolvedMergeConflicts = new Dictionary<string, List<SvnConflictType>>();
        }

        /// <summary>
        /// Handles the conflict based on the preferences.
        /// </summary>
        public void OnConflict(SvnConflictEventArgs args)
        {
            if (args.ConflictReason == SvnConflictReason.Edited)
            {
                SvnAccept choice = SvnAccept.Postpone;
                if (args.ConflictType == SvnConflictType.Property)
                {
                    if (PromptOnPropertyConflict)
                    {
                        HandleConflictWithDialog(args);
                        return;
                    }
                    else
                    {
                        choice = PropertyConflictResolutionChoice;
                    }
                }
                else if (args.IsBinary)
                {
                    if (PromptOnBinaryConflict)
                    {
                        HandleConflictWithDialog(args);
                        return;
                    }
                    else
                    {
                        choice = BinaryConflictResolutionChoice;
                    }
                }
                else
                {
                    if (PromptOnTextConflict)
                    {
                        if (UseExternalMergeTool())
                        {
                            HandleConflictWithExternalMergeTool(args);
                        }
                        else
                        {
                            HandleConflictWithDialog(args);
                        }
                        return;
                    }
                    else
                    {
                        choice = TextConflictResolutionChoice;
                    }
                }
                args.Choice = choice;
            }
            else
            {
                args.Choice = SvnAccept.Postpone;
            }
            AddToCurrentResolutions(args);
        }

        private void HandleConflictWithDialog(SvnConflictEventArgs e)
        {
            using (MergeConflictHandlerDialog dlg = new MergeConflictHandlerDialog(e))
            {
                if (dlg.ShowDialog(Context) == DialogResult.OK)
                {
                    e.Choice = dlg.ConflictResolution;
                    bool applyToAll = dlg.ApplyToAll;
                    // modify the preferences based on the conflicted file type
                    if (applyToAll)
                    {
                        PropertyConflictResolutionChoice = e.Choice;
                        PromptOnPropertyConflict = false;
                        BinaryConflictResolutionChoice = e.Choice;
                        PromptOnBinaryConflict = false;
                        TextConflictResolutionChoice = e.Choice;
                        PromptOnTextConflict = false;
                    }
                    else
                    {
                        bool applyToType = dlg.ApplyToType;
                        if (applyToType)
                        {
                            if (e.ConflictType == SvnConflictType.Property)
                            {
                                PropertyConflictResolutionChoice = e.Choice;
                                PromptOnPropertyConflict = false;
                            }
                            else if (e.IsBinary)
                            {
                                BinaryConflictResolutionChoice = e.Choice;
                                PromptOnBinaryConflict = false;
                            }
                            else
                            {
                                TextConflictResolutionChoice = e.Choice;
                                PromptOnTextConflict = false;
                            }
                        }
                    }
                    // TODO handle merged file option
                }
                else
                {
                    // Aborts the current operation.
                    e.Cancel = true;
                }
            }

            AddToCurrentResolutions(e);
        }

        private void HandleConflictWithExternalMergeTool(SvnConflictEventArgs e)
        {
            IAnkhDiffHandler handler = GetService<IAnkhDiffHandler>();
            if (handler == null)
            {
                HandleConflictWithDialog(e);
            }
            else
            {
                //Temporary file for the working copy file
                string workingTempFile = CreateTempFile();

                //copy working file contents to temporary working file
                CopyFile(e.MyFile, workingTempFile);

                //Temporary file for the merged file (in case user quits editing and original nerged file needs to be restored)
                string mergeTempFile = CreateTempFile();
                string mergeFilePath = e.MergedFile;

                //Copy original merged file to the temporary merged file
                CopyFile(mergeFilePath, mergeTempFile);
                string conflictOldFile = e.BaseFile;
                string conflictNewFile = e.TheirFile;

                AnkhMergeArgs ama = new AnkhMergeArgs();
                //Replace "/" with "\\" otherwise 
                //DiffToolMonitor constructor throws argument exception validatig the file path to be monitored.
                ama.BaseFile = conflictOldFile.Replace("/",@"\\");
                ama.TheirsFile = conflictNewFile.Replace("/", @"\\"); ;
                ama.MineFile = workingTempFile.Replace("/", @"\\"); ;
                ama.MergedFile = mergeFilePath.Replace("/", @"\\"); ;
                ama.Mode = DiffMode.PreferExternal;
                ama.BaseTitle = "Base";
                ama.TheirsTitle = "Theirs";
                ama.MineTitle = "Mine";
                ama.MergedTitle = new System.IO.FileInfo(e.Path).Name;
                bool merged = handler.RunMerge(ama);       
                if (merged)
                {
                    IUIService ui = Context.GetService<IUIService>();
                    string message = "Did you resolve all of the conflicts in the file (Mark this file resolved)?";
                    string caption = "Resolve Conflict";
                    DialogResult result = ui.ShowMessage(message, caption, MessageBoxButtons.YesNo);
                    merged = result == DialogResult.Yes;
                }
                if (!merged)
                {
                    //Restore original merged file.
                    CopyFile(mergeTempFile, mergeFilePath);
                    HandleConflictWithDialog(e);
                }
                else
                {
                    e.Choice = SvnAccept.Merged;
                }
            }
        }

        private void AddToCurrentResolutions(SvnConflictEventArgs args)
        {
            if (args != null && args.Choice != SvnAccept.Postpone)
            {
                List<SvnConflictType> conflictTypes = null;
                if (_resolvedMergeConflicts.ContainsKey(args.Path))
                {
                    conflictTypes = _resolvedMergeConflicts[args.Path];
                }
                else
                {
                    conflictTypes = new List<SvnConflictType>();
                    _resolvedMergeConflicts.Add(args.Path.Replace('/', '\\'), conflictTypes);
                }
                conflictTypes.Add(args.ConflictType);
            }
        }

        private bool UseExternalMergeTool()
        {
            IAnkhConfigurationService cs = GetService<IAnkhConfigurationService>();
            if (cs == null) { return false; }
            string mergePath = cs.Instance.MergeExePath;
            return !string.IsNullOrEmpty(mergePath);
        }

        private string CreateTempFile()
        {
            return Path.GetTempFileName();
        }

        private void CopyFile(string from, string to)
        {
            File.Copy(from, to, true);
        }

    }
}
