using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;

namespace Ankh.UI.MergeWizard
{
    class MergeConflictHandler
    {
        /// Conflict resolution preference for binary files
        SvnAccept _binaryChoice = SvnAccept.Postpone;

        /// Conflict resolution preference for text files
        SvnAccept _textChoice = SvnAccept.Postpone;

        /// Conflict resolution preference for properties
        SvnAccept _propertyChoice = SvnAccept.Postpone;

        /// flag (not) to show conflict resolution option dialog for text files
        bool _txt_showDialog = false;

        /// flag (not) to show conflict resolution option dialog for binary files
        bool _binary_showDialog = false;

        public MergeConflictHandler(SvnAccept binaryChoice, SvnAccept textChoice, SvnAccept propChoice)
            : this(binaryChoice, textChoice)
        {
        }
        
        public MergeConflictHandler(SvnAccept binaryChoice, SvnAccept textChoice)
            : this()
        {
            this._binaryChoice = binaryChoice;
            this._textChoice = textChoice;
        }
        
        public MergeConflictHandler()
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
        /// Handles the conflict based on the preferences.
        /// </summary>
        public void OnConflict(SvnConflictEventArgs args)
        {
            if (args.ConflictReason == SvnConflictReason.Edited)
            {
                SvnAccept choice = SvnAccept.Postpone;
                if (args.IsBinary)
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
                        HandleConflictWithDialog(args);
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
        }

        private void HandleConflictWithDialog(SvnConflictEventArgs args)
        {
            using (MergeConflictHandlerDialog dlg = new MergeConflictHandlerDialog(args))
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    args.Choice = dlg.ConflictResolution;
                    // TODO handle merged file option
                }
                else
                {
                    args.Choice = SvnAccept.Postpone;
                }
            }
        }
    }
}
