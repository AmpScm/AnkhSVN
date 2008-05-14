using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Ankh.UI;

namespace Ankh.VSPackage
{
    class AnkhSourceControlSettingsPage : DialogPage
    {
        AnkhSettingsControl _control;
        protected override System.Windows.Forms.IWin32Window Window
        {
            get
            {
				return Control;
            }
        }

		AnkhSettingsControl Control
		{
			get
			{
				return _control ?? (_control = CreateControl());
			}
		}

        AnkhSettingsControl CreateControl()
        {
            AnkhSettingsControl control = new AnkhSettingsControl();
            IAnkhServiceProvider sp = (IAnkhServiceProvider)GetService(typeof(IAnkhServiceProvider));
            
            if (sp != null)
                control.Context = sp;

            return control;
        }

		public override void LoadSettingsFromStorage()
		{
			base.LoadSettingsFromStorage();

			IAnkhServiceProvider sp = (IAnkhServiceProvider)GetService(typeof(IAnkhServiceProvider));
			if (sp != null)
			{
				IAnkhConfigurationService cfgSvc = sp.GetService<IAnkhConfigurationService>();
				cfgSvc.LoadConfig();
			}
			Control.LoadSettings();
		}

		public override void SaveSettingsToStorage()
		{
			base.SaveSettingsToStorage();

			Control.SaveSettings();
			IAnkhServiceProvider sp = (IAnkhServiceProvider)GetService(typeof(IAnkhServiceProvider));
			if (sp != null)
			{
				IAnkhConfigurationService cfgSvc = sp.GetService<IAnkhConfigurationService>();
				cfgSvc.SaveConfig(cfgSvc.Instance);
			}
		}

		public override void ResetSettings()
		{
			base.ResetSettings();

			IAnkhServiceProvider sp = (IAnkhServiceProvider)GetService(typeof(IAnkhServiceProvider));
			if (sp != null)
			{
				IAnkhConfigurationService cfgSvc = sp.GetService<IAnkhConfigurationService>();
				cfgSvc.LoadDefaultConfig();
			}
		}
    }
}
