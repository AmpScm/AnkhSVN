using System;
using System.Collections.Generic;
using System.Text;
using WizardFramework;
using System.Drawing;
using System.Windows.Forms;

namespace Ankh.UI.MergeWizard
{
    public abstract class BasePage : WizardPage
    {
        readonly AnkhWizard _wizard;
        protected BasePage(AnkhWizard wizard, string name)
            :base(name)
        {
            if (wizard == null)
                throw new ArgumentNullException("wizard");

            _wizard = wizard;
        }

        protected BasePage(AnkhWizard wizard, string name, Image image)
            : base(name, image)
        {
            if (wizard == null)
                throw new ArgumentNullException("wizard");

            _wizard = wizard;
        }
        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public IAnkhServiceProvider Context
        {
            get { return _wizard.Context; }
        }
    }
    public abstract class BasePage<TWizard, TControl> : BasePage
        where TWizard : AnkhWizard
        where TControl : UserControl, new()
    {
        protected BasePage(TWizard wizard, string name)
            : base(wizard, name)
        {
            Wizard = wizard;
            _control = new TControl();
        }

        protected BasePage(TWizard wizard, string name, Image image)
            : base(wizard, name, image)
        {
            Wizard = wizard;
            _control = new TControl();
        }

        

        TWizard _wizard;
        /// <summary>
        /// Gets or sets the wizard.
        /// </summary>
        /// <value>The wizard.</value>
        public new TWizard Wizard
        {
            get { return _wizard ?? (_wizard = (TWizard)base.Wizard); }
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
