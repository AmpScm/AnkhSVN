using System;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using Ankh.Diff.DiffUtils;
using SharpSvn;

namespace Ankh.UI.MergeWizard
{
    public partial class MergeConflictHandlerDialog : Form
    {
        SvnConflictEventArgs input;
        SvnAccept resolution = SvnAccept.Postpone;
        bool applyToAll = false;
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
                    applyToCheckBox.Text = "&Apply to all property conflicts";
                }
                else if (isBinary)
                {
                    applyToCheckBox.Text = "&Apply to all binary conflicts";
                }
                else
                {
                    applyToCheckBox.Text = "&Apply to all text conflicts";
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
        /// </summary>
        public bool ApplyToAll
        {
            get
            {
                return applyToAll;
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
            StringCollection A, B;
            GetFileLines(mine, theirs, out A, out B);
            TextDiff Diff = new TextDiff(HashType.HashCode, false, false);
            EditScript Script = Diff.Execute(A, B);

            string strCaptionA = "Mine";
            string strCaptionB = "Theirs";
            Ankh.Diff.FileName fnA = new Ankh.Diff.FileName(mine);
            Ankh.Diff.FileName fnB = new Ankh.Diff.FileName(theirs);
            diffControl.SetData(A, B, Script, strCaptionA, strCaptionB);
        }

        private void GetFileLines(string strA, string strB, out StringCollection A, out StringCollection B)
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
            this.applyToAll = this.applyToCheckBox.Checked;
        }
    }
}
