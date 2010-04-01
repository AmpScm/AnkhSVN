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
using Ankh.UI.WizardFramework;
using System.Windows.Forms;
using SharpSvn;
using Ankh.UI.SvnLog;
using System.Collections.ObjectModel;
using Ankh.Scc;
using System.ComponentModel;

namespace Ankh.UI.MergeWizard
{
    /// <summary>
    /// This is the wizard implementation for AnkhSVN's merge capability.
    /// </summary>
    public partial class MergeWizard : Wizard
    {
        private WizardPage mergeTypePage;
        private WizardPage bestPracticesPage;
        private WizardPage mergeSourceRangeOfRevisionsPage;
        private WizardPage mergeSourceReintegratePage;
        private WizardPage mergeSourceTwoDifferentTreesPage;
        private WizardPage mergeSourceManuallyRecordPage;
        private WizardPage mergeSourceManuallyRemovePage;
        private WizardPage mergeRevisionsSelectionPage;
        private MergeOptionsPage mergeOptionsPage;
        private WizardPage mergeSummaryPage;

        MergeUtils _mergeUtils = null;
        SvnItem _mergeTarget = null;
        IEnumerable<SvnRevisionRange> _mergeRevisions = null;
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
        /// <param name="utils"></param>
        public MergeWizard(IAnkhServiceProvider context, SvnItem mergeTarget)
        {
            InitializeComponent();

            DefaultPageImage = MergeStrings.MergeWizardHeaderImage;

            MergeUtils = new MergeUtils(context);
            MergeTarget = mergeTarget;

            Text = MergeStrings.MergeWizardTitle;

            mergeTypePage = new MergeTypePage();
            bestPracticesPage = new MergeBestPracticesPage();
            mergeSourceRangeOfRevisionsPage = new MergeSourceRangeOfRevisionsPage();
            mergeSourceReintegratePage = new MergeSourceReintegratePage();
            mergeSourceTwoDifferentTreesPage = new MergeSourceTwoDifferentTreesPage();
            mergeSourceManuallyRecordPage = new MergeSourceManuallyRecordPage();
            mergeSourceManuallyRemovePage = new MergeSourceManuallyRemovePage();
            mergeRevisionsSelectionPage = new MergeRevisionsSelectionPage();
            mergeOptionsPage = new MergeOptionsPage();
            mergeSummaryPage = new MergeSummaryPage();
        }

        public override void AddPages()
        {
            Pages.Add(mergeTypePage);
            Pages.Add(bestPracticesPage);
            Pages.Add(mergeSourceRangeOfRevisionsPage);
            Pages.Add(mergeSourceReintegratePage);
            Pages.Add(mergeSourceTwoDifferentTreesPage);
            Pages.Add(mergeSourceManuallyRecordPage);
            Pages.Add(mergeSourceManuallyRemovePage);
            Pages.Add(mergeRevisionsSelectionPage);
            Pages.Add(mergeOptionsPage);
            Pages.Add(mergeSummaryPage);
        }

        /// <see cref="WizardFramework.IWizard.GetNextPage" />
        public override WizardPage GetNextPage(WizardPage page)
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
        public override bool NextIsFinish
        {
            get
            {
                if (!(CurrentPage is MergeSummaryPage))
                    return false;

                return CurrentPage.IsPageComplete;
            }
        }

        /// <see cref="WizardFramework.IWizard.PerformFinish" />
        public override void OnFinish(CancelEventArgs e)
        {
            try
            {
                EnablePageAndButtons(false);

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

                    e.Cancel = true;
                }
                else
                {
                    PerformMerge();

                    this.DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                CurrentPage.Message = new WizardMessage(ex.InnerException.Message, WizardMessage.MessageType.Error);

                e.Cancel = false;
            }
            finally
            {
                EnablePageAndButtons(true);
            }
        }

        public void PerformMerge()
        {
            MergeType mergeType = GetPage<MergeTypePage>().SelectedMergeType;

            ProgressRunnerArgs runnerArgs = new ProgressRunnerArgs();
            runnerArgs.CreateLog = !PerformDryRun;
            // Perform merge using IProgressRunner
            Context.GetService<IProgressRunner>().RunModal(MergeStrings.MergingTitle, runnerArgs,
                delegate(object sender, ProgressWorkerArgs ee)
                {
                    _mergeActions = new List<SvnNotifyEventArgs>();
                    _resolvedMergeConflicts = new Dictionary<string, List<SvnConflictType>>();
                    MergeConflictHandler mergeConflictHandler = CreateMergeConflictHandler();
                    Handler conflictHandler = new Handler(Context, ee.Synchronizer, mergeConflictHandler);

                    try
                    {
                        if (!PerformDryRun)
                        {
                            // Attach the conflict handler
                            ee.Client.Conflict += new EventHandler<SvnConflictEventArgs>(conflictHandler.OnConflict);
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

                            // Set the proper depth
                            args.Depth = mergeOptionsPage.Depth;

                            // Set whether or not unversioned obstructions should be allowed
                            args.Force = mergeOptionsPage.AllowUnversionedObstructions;

                            // Set whether or not to ignore ancestry
                            args.IgnoreAncestry = mergeOptionsPage.IgnoreAncestry;

                            // Set whether or not this merge should just record the merge information
                            args.RecordOnly = (mergeType == MergeType.ManuallyRecord || mergeType == MergeType.ManuallyRemove);

                            // Set whether or not this is a dry run
                            args.DryRun = PerformDryRun;



                            //no need to continue with the merge operation since there are no revisions to merge
                            if (MergeRevisions != null && EnumTools.GetFirst(MergeRevisions) == null)
                            {
                                throw new Exception(MergeStrings.NoLogItems);
                            }

                            if (MergeRevisions == null)
                            {
                                // Merge all eligible
                                ee.Client.Merge(
                                    MergeTarget.FullPath,
                                    MergeSource.Target,
                                    new SvnRevisionRange(SvnRevision.Zero, SvnRevision.Head),
                                    args);
                            }
                            else
                            {
                                // Cherrypicking
                                ee.Client.Merge(
                                    MergeTarget.FullPath,
                                    MergeSource.Target,
                                    new List<SvnRevisionRange>(MergeRevisions),
                                    args);
                            }
                        }
                    }
                    finally
                    {
                        if (!PerformDryRun)
                        {
                            // Detach the conflict handler
                            ee.Client.Conflict -= new EventHandler<SvnConflictEventArgs>(conflictHandler.OnConflict);
                        }

                        // Detach the notify handler
                        ee.Client.Notify -= new EventHandler<SvnNotifyEventArgs>(OnNotify);

                        // Detach the cancel handler
                        ee.Client.Cancel -= new EventHandler<SvnCancelEventArgs>(this.OnCancel);

                        if (mergeConflictHandler != null)
                        {
                            _resolvedMergeConflicts = mergeConflictHandler.ResolvedMergedConflicts;
                            mergeConflictHandler = null;
                        }
                    }
                });

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
                return MergeStrings.MergeWizardHeaderImage;
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
        internal IEnumerable<SvnRevisionRange> MergeRevisions
        {
            get { return _mergeRevisions; }
            set { _mergeRevisions = value; }
        }

        /// <summary>
        /// Returns the revisions from <code>MergeRevisions</code> as a string.
        /// </summary>
        static internal string MergeRevisionsAsString(IEnumerable<SvnRevisionRange> mergeRevisions)
        {
            IEnumerable<SvnRevisionRange> revs = mergeRevisions;
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (SvnRevisionRange r in revs)
            {
                if (!first)
                    sb.Append(", ");

                if (r.StartRevision.Revision + 1 == r.EndRevision.Revision)
                {
                    sb.Append(r.EndRevision);
                }
                else
                {
                    sb.AppendFormat("{0}-{1}", r.StartRevision.Revision + 1, r.EndRevision);
                }

                first = false;
            }

            return sb.ToString();
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

        private MergeConflictHandler CreateMergeConflictHandler()
        {
            MergeConflictHandler mergeConflictHandler = new MergeConflictHandler(Context);
            if (mergeOptionsPage != null)
            {
                MergeOptionsPage.ConflictResolutionOption binaryOption = mergeOptionsPage.BinaryConflictResolution;
                if (binaryOption == MergeOptionsPage.ConflictResolutionOption.PROMPT)
                {
                    mergeConflictHandler.PromptOnBinaryConflict = true;
                }
                else
                {
                    mergeConflictHandler.BinaryConflictResolutionChoice = ToSvnAccept(binaryOption);
                }

                MergeOptionsPage.ConflictResolutionOption textOption = mergeOptionsPage.TextConflictResolution;
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

    // Makes sure there is no cross-thread call during interactive merge handling
    class Handler : AnkhService
    {
        ISynchronizeInvoke _synchronizer;
        MergeConflictHandler _currentMergeConflictHandler;

        public Handler(IAnkhServiceProvider context, ISynchronizeInvoke synchronizer, MergeConflictHandler conflictHandler)
            : base(context)
        {
            _synchronizer = synchronizer;
            _currentMergeConflictHandler = conflictHandler;
        }

        public void OnConflict(object sender, SvnConflictEventArgs e)
        {
            if (_synchronizer != null && _synchronizer.InvokeRequired)
            {
                // If needed marshall the call to the UI thread

                e.Detach(); // Make this instance thread safe!

                _synchronizer.Invoke(new EventHandler<SvnConflictEventArgs>(OnConflict), new object[] { sender, e });
                return;
            }

            _currentMergeConflictHandler.OnConflict(e);
        }
    }
}
