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
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using Ankh.Diff.DiffUtils;
using SharpSvn;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Ankh.UI.MergeWizard
{
    public partial class MergeConflictHandlerDialog : VSDialogForm
    {
        SvnConflictEventArgs input;
        SvnAccept resolution = SvnAccept.Postpone;
        bool applyToAll = false;
        bool applyToType = false;
        bool isBinary;

        public MergeConflictHandlerDialog()
        {
            InitializeComponent();
        }

        public MergeConflictHandlerDialog(SvnConflictEventArgs args)
        {
            InitializeComponent();
            this.input = args;
            this.postponeRadioButton.Checked = true;
            this.Text = args.Path.Replace('/', '\\');
            if (this.input != null)
            {
                isBinary = this.input.IsBinary;
                if (this.input.ConflictType == SvnConflictType.Property)
                {
                    applyToTypedCheckBox.Text = "All &Property conflicts";
                }
                else if (isBinary)
                {
                    applyToTypedCheckBox.Text = "All &Binary conflicts";
                }
                else
                {
                    applyToTypedCheckBox.Text = "All &Text conflicts";
                }
                ShowDifferences(this.input.MyFile, this.input.TheirFile);
            }
        }

        /// <summary>
        /// Gets the conflict resolution preference
        /// </summary>
        public SvnAccept ConflictResolution
        {
            get
            {
                return resolution;
            }
            internal set
            {
                this.resolution = value;
            }
        }

        /// <summary>
        /// Gets applyToAll option.
        /// If this option is selected, the user choice will be used to resolve all conflicts.
        /// </summary>
        public bool ApplyToAll
        {
            get
            {
                return applyToAll;
            }
        }

        /// <summary>
        /// Gets applyToType option. 
        /// If this option is selected, the user choice will be used to resolve all conflicts of the same type.
        /// </summary>
        public bool ApplyToType
        {
            get
            {
                return applyToType;
            }
        }

        private void postponeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            this.ConflictResolution = SvnAccept.Postpone;
        }

        private void mineRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            this.ConflictResolution = SvnAccept.MineFull;
        }

        private void theirsRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            this.ConflictResolution = SvnAccept.TheirsFull;
        }

        private void baseRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            this.ConflictResolution = SvnAccept.Base;
        }

        /// Sets the diff data
        private void ShowDifferences(string mine, string theirs)
        {
            Collection<string> A, B;
            GetFileLines(mine, theirs, out A, out B);
            TextDiff Diff = new TextDiff(HashType.HashCode, false, false);
            EditScript Script = Diff.Execute(A, B);

            string strCaptionA = "Mine";
            string strCaptionB = "Theirs";
            //Ankh.Diff.FileName fnA = new Ankh.Diff.FileName(mine);
            //Ankh.Diff.FileName fnB = new Ankh.Diff.FileName(theirs);
            diffControl.SetData(A, B, Script, strCaptionA, strCaptionB);
        }

        private void GetFileLines(string strA, string strB, out Collection<string> A, out Collection<string> B)
        {
            if (this.isBinary)
            {
                using (FileStream AF = File.OpenRead(strA))
                using (FileStream BF = File.OpenRead(strB))
                {
                    BinaryDiff BDiff = new BinaryDiff();
                    BDiff.FootprintLength = 8;
                    AddCopyList List = BDiff.Execute(AF, BF);

                    BinaryDiffLines Lines = new BinaryDiffLines(AF, List, 8);
                    A = Lines.BaseLines;
                    B = Lines.VerLines;
                }
            }
            else
            {
                A = Functions.GetFileTextLines(strA);
                B = Functions.GetFileTextLines(strB);
            }
        }

        private void applyToCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.applyToAll = this.applyToAllCheckBox.Checked;

            // applyToType is implied if applyToAll is checked.
            this.applyToTypedCheckBox.Checked = this.applyToAll ? true : this.applyToType;
            this.applyToTypedCheckBox.Enabled = !this.applyToAll;
        }

        private void applyToTypedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.applyToType = this.applyToTypedCheckBox.Checked;
        }
    }
}
