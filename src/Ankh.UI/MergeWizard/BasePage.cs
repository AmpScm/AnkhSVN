using System;
using System.Collections.Generic;
using System.Text;
using WizardFramework;
using System.Drawing;
using System.Windows.Forms;

namespace Ankh.UI.MergeWizard
{
    
    public class BasePage : WizardPage
    {
        protected BasePage(AnkhWizard wizard, BasePageControl control, string name)
            : base(name)
        {
            if (wizard == null)
                throw new ArgumentNullException("wizard");
            if (control == null)
                throw new ArgumentNullException("control");

            Wizard = wizard;
            _control = control;
        }

        protected BasePage(AnkhWizard wizard, BasePageControl control, string name, Image image)
            : base(name, image)
        {
            if (wizard == null)
                throw new ArgumentNullException("wizard");
            if (control == null)
                throw new ArgumentNullException("control");

            Wizard = wizard;
            _control = control;
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public IAnkhServiceProvider Context
        {
            get { return _wizard.Context; }
        }

        

        AnkhWizard _wizard;
        /// <summary>
        /// Gets or sets the wizard.
        /// </summary>
        /// <value>The wizard.</value>
        public new AnkhWizard Wizard
        {
            get { return _wizard ?? (_wizard = (AnkhWizard)base.Wizard); }
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

        readonly BasePageControl _control;
        public sealed override UserControl Control
        {
            get 
            {
                return _control;
            }
        }

        //[Obsolete("Use Control")]
        public BasePageControl PageControl
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
