using System;
using System.Collections.Generic;
using System.Text;
using WizardFramework;
using System.Drawing;
using System.Windows.Forms;

namespace Ankh.UI.MergeWizard
{
    public abstract class BasePage<TWizard, TControl> : WizardPage
        where TWizard : class, IWizard
        where TControl : UserControl, new()
    {
        protected BasePage(TWizard wizard, string name)
            : base(name)
        {
            Wizard = wizard;
            _control = new TControl();
        }

        protected BasePage(TWizard wizard, string name, Image image)
            : base(name, image)
        {
            Wizard = wizard;
            _control = new TControl();
        }

        TWizard _wizard;
        public new TWizard Wizard
        {
            get
            {
                if(_wizard == null)
                    _wizard = (TWizard)base.Wizard;
                
                return _wizard;
            }
            set
            {
                _wizard = value;
                base.Wizard = value;

                // HACK: Should not have intimage knowledge about mergewizard, refactor
                MergeWizard mergeWizard = value as MergeWizard;
                if(mergeWizard != null)
                {
                    mergeWizard.WizardDialog.PageChangeEvent += new EventHandler<WizardPageChangeEventArgs>(WizardDialog_PageChangeEvent);
                    mergeWizard.WizardDialog.PageChangingEvent += new EventHandler<WizardPageChangingEventArgs>(WizardDialog_PageChangingEvent);
                }
            }
        }

        void WizardDialog_PageChangingEvent(object sender, WizardPageChangingEventArgs e)
        {
            if (e.CurrentPage == this)
                OnPageChanging(e);
        }
        void WizardDialog_PageChangeEvent(object sender, WizardPageChangeEventArgs e)
        {
            if (e.CurrentPage.PreviousPage == this)
                OnPageChanged(e);
        }

        readonly TControl _control;
        public sealed override UserControl Control
        {
            get 
            {
                return _control;
            }
        }

        public TControl PageControl
        {
            get
            {
                return _control;
            }
        }

        protected virtual void OnPageChanging(WizardPageChangingEventArgs e)
        {
        }

        protected virtual void OnPageChanged(WizardPageChangeEventArgs e)
        {
        }
    }
}
