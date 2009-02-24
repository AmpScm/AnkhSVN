// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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
using WizardFramework;
using System.Windows.Forms;
using SharpSvn;
using Ankh.UI.SvnLog;
using System.Collections.ObjectModel;
using Ankh.Scc;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// This is the wizard implementation for AnkhSVN's merge capability.
    /// </summary>
    public class MergeWizard : AnkhWizard
    {
        private WizardPage mergeTypePage;
        private WizardPage bestPracticesPage;
        private WizardPage mergeSourceRangeOfRevisionsPage;
        private WizardPage mergeSourceReintegratePage;
        private WizardPage mergeSourceTwoDifferentTreesPage;
        private WizardPage mergeSourceManuallyRecordPage;
        private WizardPage mergeSourceManuallyRemovePage;
        private WizardPage mergeRevisionsSelectionPage;
        private WizardPage mergeOptionsPage;
        private WizardPage mergeSummaryPage;

        MergeUtils _mergeUtils = null;
        SvnItem _mergeTarget = null;
        long[] _mergeRevisions = null;
        bool _performDryRun = false;
        List<SvnNotifyEventArgs> _mergeActions;
        Dictionary<string, List<SvnConflictType>> _resolvedMergeConflicts;

        /// <summary>
        /// Enumeration of available merge types.
        /// </summary>
        public enum MergeType
        {
            RangeOfRevisions,
            Reintegrate,
            TwoDifferentTrees,
            ManuallyRecord,
            ManuallyRemove
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MergeWizard(IAnkhServiceProvider context, IWizardContainer container)
            : base(context)
        {
            Container = container;
            this.WindowTitle = Resources.MergeWizardTitle;

            mergeTypePage = new MergeTypePage(this);
            bestPracticesPage = new MergeBestPracticesPage(this);
            mergeSourceRangeOfRevisionsPage = new MergeSourceRangeOfRevisionsPage(this);
            mergeSourceReintegratePage = new MergeSourceReintegratePage(this);
            mergeSourceTwoDifferentTreesPage = new MergeSourceTwoDifferentTreesPage(this);
            mergeSourceManuallyRecordPage = new MergeSourceManuallyRecordPage(this);
            mergeSourceManuallyRemovePage = new MergeSourceManuallyRemovePage(this);
            mergeRevisionsSelectionPage = new MergeRevisionsSelectionPage(this);
            mergeOptionsPage = new MergeOptionsPage(this);
            mergeSummaryPage = new MergeSummaryPage(this);
        }

        public override void AddPages()
        {
            AddPage(mergeTypePage);
            AddPage(bestPracticesPage);
            AddPage(mergeSourceRangeOfRevisionsPage);
            AddPage(mergeSourceReintegratePage);
            AddPage(mergeSourceTwoDifferentTreesPage);
            AddPage(mergeSourceManuallyRecordPage);
            AddPage(mergeSourceManuallyRemovePage);
            AddPage(mergeRevisionsSelectionPage);
            AddPage(mergeOptionsPage);
            AddPage(mergeSummaryPage);
        }

        /// <see cref="WizardFramework.IWizard.GetNextPage" />
        public override IWizardPage GetNextPage(IWizardPage page)
        {
            // Handle the main page
            if (page is MergeTypePage)
            {
                // Handle displaying the best practices page
                if (((MergeTypePage)page).ShowBestPracticesPage 
                    && ((MergeBestPracticesPage)bestPracticesPage).DisplayBestPracticesPage)
                    return bestPracticesPage;
                else
                    switch (((MergeTypePage)page).SelectedMergeType)
                    {
                        case MergeType.RangeOfRevisions:
                            return mergeSourceRangeOfRevisionsPage;
                        case MergeType.Reintegrate:
                            return mergeSourceReintegratePage;
                        case MergeType.TwoDifferentTrees:
                            return mergeSourceTwoDifferentTreesPage;
                        case MergeType.ManuallyRecord:
                            return mergeSourceManuallyRecordPage;
                        case MergeType.ManuallyRemove:
                            return mergeSourceManuallyRemovePage;
                        default:
                            return null;
                    }
            }

            // Handle the best practices page
            if (page is MergeBestPracticesPage)
                return null; // For now, if you see the best practices page,
                             // you have to fix the issue and then reattempt to merge.

            // Handle the range of revisions page
            if (page is MergeSourceRangeOfRevisionsPage)
            {
                if (((MergeSourceRangeOfRevisionsPage)page).NextPageRequired)
                    return mergeRevisionsSelectionPage;
                else
                    return mergeOptionsPage;
            }

            // Handle the reintegrate page
            // Handle the revision selection page
            // Handle the two different trees page
            if (page is MergeSourceReintegratePage ||
                page is MergeRevisionsSelectionPage ||
                page is MergeSourceTwoDifferentTreesPage)
                return mergeOptionsPage;

            // Handle the manually record/remove pages
            if (page is MergeSourceManuallyRecordPage || page is MergeSourceManuallyRemovePage)
                return mergeRevisionsSelectionPage;

            // Handle the merge options page
            if (page is MergeOptionsPage)
                return mergeSummaryPage;

            return null;
        }

        /// <see cref="WizardFramework.IWizard.CanFinish" />
        public override bool CanFinish
        {
            get
            {
                if (Container.CurrentPage is MergeTypePage)
                    return false;

                if (Container.CurrentPage is MergeBestPracticesPage)
                    return false;

                if (Container.CurrentPage is MergeSourceRangeOfRevisionsPage)
                {
                    return Container.CurrentPage.IsPageComplete &&
                        !((MergeSourceRangeOfRevisionsPage)Container.CurrentPage).NextPageRequired;
                }

                if (Container.CurrentPage is MergeSourceManuallyRecordPage)
                    return false;
                
                return Container.CurrentPage.IsPageComplete;
            }
        }

        private MergeConflictHandler currentMergeConflictHandler;

        /// <see cref="WizardFramework.IWizard.PerformFinish" />
        public override bool PerformFinish()
        {
            bool status = false;

            try
            {
                ((WizardDialog)Form).EnablePageAndButtons(false);

                if (PerformDryRun)
                {
                    PerformMerge();

                    if (_mergeActions != null && _resolvedMergeConflicts != null)
                    {
                        MergeResultsDialog dialog = new MergeResultsDialog();

                        dialog.MergeActions = MergeActions;
                        dialog.ResolvedMergeConflicts = ResolvedMergeConflicts;

                        dialog.ShowDialog(Context);
                    }

                    status = false;
                }
                else
                {
                    PerformMerge();

                    this.Form.DialogResult = DialogResult.OK;

                    status = true;
                }
            }
            catch (Exception e)
            {
                ((WizardDialog)Form).CurrentPage.Message = new WizardMessage(e.InnerException.Message, WizardMessage.MessageType.Error);

                status = false;
            }
            finally
            {
                ((WizardDialog)Form).EnablePageAndButtons(true);
            }

            return status;
        }

        public void PerformMerge()
        {
            MergeType mergeType = ((MergeTypePage)GetPage(MergeTypePage.PAGE_NAME)).SelectedMergeType;

            this.currentMergeConflictHandler = CreateMergeConflictHandler();

            // Perform merge using IProgressRunner
            Context.GetService<IProgressRunner>().RunModal(Resources.MergingTitle,
                delegate(object sender, ProgressWorkerArgs ee)
                {
                    _mergeActions = new List<SvnNotifyEventArgs>();
                    _resolvedMergeConflicts = new Dictionary<string, List<SvnConflictType>>();

                    try
                    {
                        if (!PerformDryRun)
                        {
                            // Attach the conflict handler
                            ee.Client.Conflict += new EventHandler<SvnConflictEventArgs>(this.OnConflict);
                        }

                        // Attach the cancel handler
                        ee.Client.Cancel += new EventHandler<SvnCancelEventArgs>(this.OnCancel);

                        // Attach the notify handler
                        ee.Client.Notify += new EventHandler<SvnNotifyEventArgs>(this.OnNotify);

                        if (mergeType == MergeType.TwoDifferentTrees)
                        {
                            MergeSourceTwoDifferentTreesPage page = ((MergeSourceTwoDifferentTreesPage)mergeSourceTwoDifferentTreesPage);
                            SvnDiffMergeArgs dArgs = new SvnDiffMergeArgs();
                            Uri fromUri;
                            Uri toUri;

                            // Set the proper depth
                            dArgs.Depth = ((MergeOptionsPage)mergeOptionsPage).Depth;

                            // Set whether or not unversioned obstructions should be allowed
                            dArgs.Force = ((MergeOptionsPage)mergeOptionsPage).AllowUnversionedObstructions;

                            // Set whether or not to ignore ancestry
                            dArgs.IgnoreAncestry = ((MergeOptionsPage)mergeOptionsPage).IgnoreAncestry;

                            // Set whether or not this is a dry run
                            dArgs.DryRun = PerformDryRun;

                            // Create 'From' uri
                            Uri.TryCreate(page.MergeSourceOne, UriKind.Absolute, out fromUri);

                            // Create 'To' uri if necessary
                            if (page.HasSecondMergeSourceUrl)
                                Uri.TryCreate(page.MergeSourceTwo, UriKind.Absolute, out toUri);
                            else
                                toUri = fromUri;

                            ee.Client.DiffMerge(MergeTarget.FullPath,
                                new SvnUriTarget(fromUri, (page.MergeFromRevision > -1 ?
                                    new SvnRevision(page.MergeFromRevision - 1) :
                                    SvnRevision.Head)),
                                new SvnUriTarget(toUri, (page.MergeToRevision > -1 ?
                                    new SvnRevision(page.MergeToRevision) :
                                    SvnRevision.Head)),
                                dArgs);
                        }
                        else if (mergeType == MergeType.Reintegrate)
                        {
                            SvnReintegrationMergeArgs args = new SvnReintegrationMergeArgs();

                            // Set whether or not this is a dry run
                            args.DryRun = PerformDryRun;

                            ee.Client.ReintegrationMerge(MergeTarget.FullPath, MergeSource.Target, args);
                        }
                        else
                        {
                            SvnMergeArgs args = new SvnMergeArgs();
                            List<SvnRevisionRange> mergeRevisions = null;

                            // Set the proper depth
                            args.Depth = ((MergeOptionsPage)mergeOptionsPage).Depth;

                            // Set whether or not unversioned obstructions should be allowed
                            args.Force = ((MergeOptionsPage)mergeOptionsPage).AllowUnversionedObstructions;

                            // Set whether or not to ignore ancestry
                            args.IgnoreAncestry = ((MergeOptionsPage)mergeOptionsPage).IgnoreAncestry;

                            // Set whether or not this merge should just record the merge information
                            args.RecordOnly = (mergeType == MergeType.ManuallyRecord || mergeType == MergeType.ManuallyRemove);

                            // Set whether or not this is a dry run
                            args.DryRun = PerformDryRun;

                            // TODO: Enhance to be range-aware
                            if (MergeRevisions != null)
                            {
                                mergeRevisions = new List<SvnRevisionRange>();
                                foreach (long rev in MergeRevisions)
                                {
                                    mergeRevisions.Add(new SvnRevisionRange(rev - 1, rev));
                                }
                            }
                            else
                            {
                                // This should only occur when you choose 'All eligible revisions'
                                if (mergeType == MergeType.RangeOfRevisions)
                                {
                                    // Don't calculate eligible, just use 0:HEAD further on
                                    // TODO: clean this up
                                }
                            }

                            //no need to continue with the merge operation since there are no revisions to merge
                            if (mergeRevisions != null && mergeRevisions.Count == 0)
                            {
                                throw new Exception(Resources.NoLogItems);
                            }

                            if (mergeRevisions == null)
                                ee.Client.Merge(MergeTarget.FullPath, MergeSource.Target, new SvnRevisionRange(SvnRevision.Zero, SvnRevision.Head), args);
                            else
                                ee.Client.Merge(MergeTarget.FullPath, MergeSource.Target, mergeRevisions, args);
                        }
                    }
                    finally
                    {
                        if (!PerformDryRun)
                        {
                            // Detach the conflict handler
                            ee.Client.Conflict -= new EventHandler<SvnConflictEventArgs>(OnConflict);
                        }

                        // Detach the notify handler
                        ee.Client.Notify -= new EventHandler<SvnNotifyEventArgs>(OnNotify);

                        // Detach the cancel handler
                        ee.Client.Cancel -= new EventHandler<SvnCancelEventArgs>(this.OnCancel);
                    }
                });

            if (this.currentMergeConflictHandler != null)
            {
                _resolvedMergeConflicts = this.currentMergeConflictHandler.ResolvedMergedConflicts;
            }
        }

        void OnCancel(object sender, SvnCancelEventArgs e)
        {
            // BH: This method relies on the knowledge that the dialog hooks Cancel before us.
            if (e.Cancel)
            {
                _mergeActions = null;
                _resolvedMergeConflicts = null;
            }
        }

        void OnNotify(object sender, SvnNotifyEventArgs e)
        {
            // Do not catalog MergeBegin
            if (e.Action == SvnNotifyAction.MergeBegin)
                return;

            e.Detach();

            _mergeActions.Add(e);
        }

        /// <see cref="WizardFramework.IWizard.DefaultPageImage" />
        public override System.Drawing.Image DefaultPageImage
        {
            get
            {
                return Resources.MergeWizardHeaderImage;
            }
        }

        internal MergeUtils MergeUtils
        {
            get { return _mergeUtils; }
            set { _mergeUtils = value; }
        }


        /// <summary>
        /// Gets or sets the merge target.
        /// </summary>
        /// <value>The merge target.</value>
        public SvnItem MergeTarget
        {
            get { return _mergeTarget; }
            set { _mergeTarget = value; }
        }

        SvnOrigin _mergeSource;
        /// <summary>
        /// Gets or sets the merge source.
        /// </summary>
        /// <value>The merge source.</value>
        public SvnOrigin MergeSource
        {
            get { return _mergeSource; }
            set { _mergeSource = value; }
        }

        /// <summary>
        /// Integer array for the revision(s) being merged.
        /// </summary>
        internal long[] MergeRevisions
        {
            get { return _mergeRevisions; }
            set { _mergeRevisions = value; }
        }

        /// <summary>
        /// Returns the revisions from <code>MergeRevisions</code> as a string.
        /// </summary>
        internal string MergeRevisionsAsString
        {
            get
            {
                long[] revs = MergeRevisions;
                StringBuilder sb = new StringBuilder();


                for (int i = 0; i < revs.Length; i++)
                {
                    if (i != revs.Length - 1)
                        sb.Append(revs[i].ToString() + ", ");
                    else
                        sb.Append(revs[i].ToString());
                }

                // TODO: Make the string range-aware.  So instead of '1, 3, 4, 5, 8',
                // return '1, 3-5, 8'.

                return sb.ToString();
            }
        }

        LogMode _logMode = LogMode.Log;
        /// <summary>
        /// Gets or sets the log mode.
        /// </summary>
        /// <value>The log mode.</value>
        public LogMode LogMode
        {
            get { return _logMode; }
            set { _logMode = value; }
        }

        private void OnConflict(object sender, SvnConflictEventArgs args)
        {
            if (this.currentMergeConflictHandler == null)
            {
                this.currentMergeConflictHandler = CreateMergeConflictHandler();
            }
            this.currentMergeConflictHandler.OnConflict(args);
        }

        private MergeConflictHandler CreateMergeConflictHandler()
        {
            MergeConflictHandler mergeConflictHandler = new MergeConflictHandler(Context);
            if (mergeOptionsPage != null)
            {
                MergeOptionsPage optionsPage = (MergeOptionsPage)mergeOptionsPage;
                MergeOptionsPage.ConflictResolutionOption binaryOption = optionsPage.BinaryConflictResolution;
                if (binaryOption == MergeOptionsPage.ConflictResolutionOption.PROMPT)
                {
                    mergeConflictHandler.PromptOnBinaryConflict = true;
                }
                else
                {
                    mergeConflictHandler.BinaryConflictResolutionChoice = ToSvnAccept(binaryOption);
                }

                MergeOptionsPage.ConflictResolutionOption textOption = optionsPage.TextConflictResolution;
                if (textOption == MergeOptionsPage.ConflictResolutionOption.PROMPT)
                {
                    mergeConflictHandler.PromptOnTextConflict = true;
                }
                else
                {
                    mergeConflictHandler.TextConflictResolutionChoice = ToSvnAccept(textOption);
                }
            }
            return mergeConflictHandler;
        }

        private SvnAccept ToSvnAccept(MergeOptionsPage.ConflictResolutionOption option)
        {
            SvnAccept choice = SvnAccept.Postpone;
            switch (option)
            {
                case MergeOptionsPage.ConflictResolutionOption.BASE:
                    choice = SvnAccept.Base;
                    break;
                case MergeOptionsPage.ConflictResolutionOption.MINE:
                    choice = SvnAccept.MineFull;
                    break;
                case MergeOptionsPage.ConflictResolutionOption.THEIRS:
                    choice = SvnAccept.TheirsFull;
                    break;
                default:
                    choice = SvnAccept.Postpone;
                    break;
            }
            return choice;
        }

        internal MergeWizardDialog WizardDialog
        {
            get { return (MergeWizardDialog)Form; }
            //set { Form = value; }
        }

        /// <summary>
        /// Gets/Sets whether or not to perform a dry run.
        /// </summary>
        public bool PerformDryRun
        {
            get { return _performDryRun; }
            set { _performDryRun = value; }
        }

        /// <summary>
        /// Gets the actions performed by the merge.
        /// </summary>
        public List<SvnNotifyEventArgs> MergeActions
        {
            get { return _mergeActions; }
        }

        /// <summary>
        /// Gets the paths of the conflicts resolved during the merge.
        /// </summary>
        public Dictionary<string, List<SvnConflictType>> ResolvedMergeConflicts
        {
            get { return _resolvedMergeConflicts; }
        }        
    }
}
