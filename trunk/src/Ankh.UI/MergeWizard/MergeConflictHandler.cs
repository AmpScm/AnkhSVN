using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using System.Windows.Forms.Design;
using System.Windows.Forms;

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
            AddToCurrentResolutions(args);
        }

        private void HandleConflictWithDialog(SvnConflictEventArgs e)
        {
            using (MergeConflictHandlerDialog dlg = new MergeConflictHandlerDialog(e))
            {
                IUIService ui = Context.GetService<IUIService>();

                DialogResult dr;

                if (ui != null)
                    dr = ui.ShowDialog(dlg);
                else
                    dr = dlg.ShowDialog();


                if (dr == DialogResult.OK)
                {
                    e.Choice = dlg.ConflictResolution;
                    bool applyToAll = dlg.ApplyToAll;
                    // modify the preferences based on the conflicted file type
                    if (applyToAll)
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
                    // TODO handle merged file option
                }
                else
                {
                    e.Choice = SvnAccept.Postpone;
                }
            }

            AddToCurrentResolutions(e);
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

    }
}
