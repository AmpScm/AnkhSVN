﻿using System;
using System.Collections.Generic;
using System.Text;
using WizardFramework;
using Ankh.ExtensionPoints.IssueTracker;

namespace Ankh.UI.IssueTracker
{
    public abstract class IssueTrackerWizard : Wizard
    {
        public IssueTrackerWizard(IAnkhServiceProvider context)
            : base(context)
        {
        }

        public override bool PerformFinish()
        {
            IssueRepositoryBase newRepository;
            if (TryCreateIssueRepository(out newRepository))
            {
                Container.NewIssueRepository = newRepository;
            }
            Form.DialogResult = System.Windows.Forms.DialogResult.OK;
            return true;
        }

        protected virtual bool TryCreateIssueRepository(out IssueRepositoryBase repository)
        {
            repository = null;
            return true;
        }

        public new IssueTrackerConfigWizardDialog Container
        {
            get
            {
                return base.Container as IssueTrackerConfigWizardDialog;
            }
            set
            {
                base.Container = value;
            }
        }


    }
}
