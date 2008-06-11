﻿using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using WizardFramework;
using Ankh.VS;

namespace Ankh.UI.MergeWizard
{
    public class MergeUtils
    {
        public static readonly WizardMessage INVALID_FROM_REVISION = new WizardMessage(Resources.InvalidFromRevision,
            WizardMessage.ERROR);
        public static readonly WizardMessage INVALID_TO_REVISION = new WizardMessage(Resources.InvalidToRevision,
            WizardMessage.ERROR);
        public static readonly WizardMessage INVALID_FROM_URL = new WizardMessage(Resources.InvalidFromUrl,
            WizardMessage.ERROR);
        public static readonly WizardMessage INVALID_TO_URL = new WizardMessage(Resources.InvalidToUrl,
            WizardMessage.ERROR);

        IAnkhServiceProvider _context;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context">The context.</param>
        public MergeUtils(IAnkhServiceProvider context)
        {
            Context = context;
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

                sources.Sort();
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
