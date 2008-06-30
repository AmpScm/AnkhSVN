using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;

namespace Ankh.UI.MergeWizard
{
    class MergeConflictHandler
    {
        SharpSvn.SvnAccept _binaryChoice = SharpSvn.SvnAccept.Postpone;
        SharpSvn.SvnAccept _textChoice = SharpSvn.SvnAccept.Postpone;
        SharpSvn.SvnAccept _propertyChoice = SharpSvn.SvnAccept.Postpone;

        bool _txt_showDialog = false;
        bool _binary_showDialog = false;

        public MergeConflictHandler(SharpSvn.SvnAccept binaryChoice, SharpSvn.SvnAccept textChoice, SharpSvn.SvnAccept propChoice)
            : this(binaryChoice, textChoice)
        {
        }
        
        public MergeConflictHandler(SharpSvn.SvnAccept binaryChoice, SharpSvn.SvnAccept textChoice)
            : this()
        {
            this._binaryChoice = binaryChoice;
            this._textChoice = textChoice;
        }
        
        public MergeConflictHandler()
        {
        }

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

        public void OnConflict(SharpSvn.SvnConflictEventArgs args)
        {
            SvnAccept choice = SvnAccept.Postpone;
            if (args.IsBinary)
            {
                if (PromptOnBinaryConflict)
                {
                    // TODO
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
                    // TODO
                }
                else
                {
                    choice = TextConflictResolutionChoice;
                }
            }
            args.Choice = choice;
        }
    }
}
