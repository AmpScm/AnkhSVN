using System;
using System.Collections.Generic;
using System.Text;
using WizardFramework;
using System.Windows.Forms;

namespace Ankh.UI.IssueTracker
{
    public class ConnectorWizardPage : WizardPage
    {
        Control _pageControl;

        public ConnectorWizardPage(string name, Control pageControl)
            : base(name)
        {
            _pageControl = pageControl;
        }

        public override System.Windows.Forms.UserControl Control
        {
            get { return _pageControl as UserControl; }
        }
    }
}
