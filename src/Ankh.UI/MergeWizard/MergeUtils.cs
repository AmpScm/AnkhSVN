using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using WizardFramework;
using Ankh.VS;

namespace Ankh.UI.MergeWizard
{
    internal class MergeUtils
    {
        public static readonly WizardMessage INVALID_FROM_REVISION = new WizardMessage(Resources.InvalidFromRevision,
            WizardMessage.MessageType.Error);
        public static readonly WizardMessage INVALID_TO_REVISION = new WizardMessage(Resources.InvalidToRevision,
            WizardMessage.MessageType.Error);
        public static readonly WizardMessage INVALID_FROM_URL = new WizardMessage(Resources.InvalidFromUrl,
            WizardMessage.MessageType.Error);
        public static readonly WizardMessage INVALID_TO_URL = new WizardMessage(Resources.InvalidToUrl,
            WizardMessage.MessageType.Error);

        private IAnkhServiceProvider _context;
        private Dictionary<SvnDepth, string> _mergeDepths;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">The context.</param>
        public MergeUtils(IAnkhServiceProvider context)
        {
            Context = context;
        }

        /// <summary>
        /// Returns a key/value pairing of <code>SharpSvn.SvnDepth</code> as the key
        /// and a string description of the depth key.
        /// </summary>
        public Dictionary<SvnDepth, string> MergeDepths
        {
            get
            {
                if (_mergeDepths == null)
                {
                    _mergeDepths = new Dictionary<SvnDepth, string>();

                    _mergeDepths.Add(SvnDepth.Unknown, Resources.SvnDepthUnknown);
                    _mergeDepths.Add(SvnDepth.Infinity, Resources.SvnDepthInfinity);
                    _mergeDepths.Add(SvnDepth.Children, Resources.SvnDepthChildren);
                    _mergeDepths.Add(SvnDepth.Files, Resources.SvnDepthFiles);
                    _mergeDepths.Add(SvnDepth.Empty, Resources.SvnDepthEmpty);
                }

                return _mergeDepths;
            }
        }

        /// <summary>
        /// Returns a list of strings for the suggested merge sources.
        /// </summary>
        public List<string> GetSuggestedMergeSources(SvnItem target, MergeWizard.MergeType mergeType)
        {
            List<string> sources = new List<string>();

            if (mergeType != MergeWizard.MergeType.Reintegrate)
            {
                using (SvnClient client = GetClient())
                {
                    SvnItem parent = target.Parent;

                    if (mergeType == MergeWizard.MergeType.ManuallyRemove)
                    {
                        SvnGetAppliedMergeInfoArgs args = new SvnGetAppliedMergeInfoArgs();
                        SvnAppliedMergeInfo mergeInfo;

                        args.ThrowOnError = false;

                        if (client.GetAppliedMergeInfo(SvnTarget.FromUri(target.Status.Uri), args, out mergeInfo))
                        {
                            foreach (SvnMergeItem item in mergeInfo.AppliedMerges)
                            {
                                string uri = item.Uri.ToString();

                                if (!sources.Contains(uri))
                                    sources.Add(uri);
                            }
                        }
                    }
                    else
                    {
                        SvnMergeSourcesCollection mergeSources;
                        SvnGetSuggestedMergeSourcesArgs args = new SvnGetSuggestedMergeSourcesArgs();

                        args.ThrowOnError = false;

                        if (client.GetSuggestedMergeSources(SvnTarget.FromUri(target.Status.Uri), args, out mergeSources))
                        {
                            foreach (SvnMergeSource source in mergeSources)
                            {
                                string uri = source.Uri.ToString();

                                if (!sources.Contains(uri))
                                    sources.Add(uri);
                            }
                        }
                    }
                }
            }

            return sources;
        }

        /// <summary>
        /// Returns an instance of <code>SharpSvn.SvnClient</code> from the pool.
        /// </summary>
        public SvnClient GetClient()
        {
            ISvnClientPool pool = (Context != null) ? Context.GetService<ISvnClientPool>() : null;

            if (pool != null)
                return pool.GetClient();
            else
                return new SvnClient();
        }

        public SvnWorkingCopyClient GetWcClient()
        {
            ISvnClientPool pool = (Context != null) ? Context.GetService<ISvnClientPool>() : null;

            if (pool != null)
                return pool.GetWcClient();
            else
                return new SvnWorkingCopyClient();
        }

        /// <summary>
        /// Returns the working copy root for the opened solution.
        /// </summary>
        public string WorkingCopyRootPath
        {
            get
            {
                IAnkhSolutionSettings settings = _context.GetService<IAnkhSolutionSettings>();

                if (settings == null)
                    return null;

                return settings.ProjectRoot;
            }
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set { _context = value; }
        }
    }
}
