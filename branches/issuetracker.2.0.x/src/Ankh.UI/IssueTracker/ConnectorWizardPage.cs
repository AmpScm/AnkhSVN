using System;
using System.Collections.Generic;
using System.Text;
using WizardFramework;
using System.Windows.Forms;
using Ankh.ExtensionPoints.IssueTracker;

namespace Ankh.UI.IssueTracker
{
    public class ConnectorWizardPage : WizardPage
    {
        UserControl _pageControl;
        IIssueRepositoryConfigurationPage _configPage;
        IntPtr _pageControlHandle;

        public ConnectorWizardPage(string name, IIssueRepositoryConfigurationPage configPage)
            : base(name)
        {
            _configPage = configPage;
            _configPage.OnPageEvent += new EventHandler<ConfigPageEventArgs>(_configPage_OnPageEvent);
        }

        void _configPage_OnPageEvent(object sender, ConfigPageEventArgs e)
        {
            if (e.IsComplete)
            {
                IsPageComplete = true;
                Message = null;
            }
            else
            {
                IsPageComplete = false;
                if (e.Exception != null)
                {
                    Message = new WizardMessage(e.Exception.Message, WizardMessage.MessageType.Error);
                }
            }
        }

        public override UserControl Control
        {
            get
            {
                if (_pageControl == null
                    && _configPage != null)
                {
                    try
                    {
                        _pageControl = UserControl.FromHandle(_configPage.Handle) as UserControl;
                    }
                    catch { }
                }
                return _pageControl;
            }
        }
    }
}
