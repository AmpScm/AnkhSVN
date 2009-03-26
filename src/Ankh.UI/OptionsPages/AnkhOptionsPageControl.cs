// $Id$
//
// Copyright 2009 The AnkhSVN Project
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
using System.Windows.Forms;
using Ankh.Configuration;

namespace Ankh.UI.OptionsPages
{
    /// <summary>
    /// 
    /// </summary>
    public class AnkhOptionsPageControl : UserControl
    {
        IAnkhServiceProvider _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnkhOptionsPageControl"/> class.
        /// </summary>
        protected AnkhOptionsPageControl()
        {
            Size = new System.Drawing.Size(400, 271);
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set { _context = value; OnContextChanged(EventArgs.Empty); }
        }


        private IAnkhConfigurationService _configSvc;
        IAnkhConfigurationService ConfigSvc
        {
            get { return _configSvc ?? (_configSvc = Context.GetService<IAnkhConfigurationService>()); }
        }

        protected AnkhConfig Config
        {
            get { return ConfigSvc.Instance; }
        }

        /// <summary>
        /// Raises the <see cref="E:ContextChanged"/> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnContextChanged(EventArgs eventArgs)
        {

        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        public void LoadSettings()
        {
            ConfigSvc.LoadConfig();
            LoadSettingsCore();
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        public void SaveSettings()
        {
            // Load in case something changed in the mean time, other page/other VS instance
            ConfigSvc.LoadConfig();

            SaveSettingsCore();

            try
            {
                ConfigSvc.SaveConfig(Config);
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler eh = Context.GetService<IAnkhErrorHandler>();

                if (eh != null && eh.IsEnabled(ex))
                {
                    eh.OnError(ex);
                    return;
                }

                throw;
            }
        }

        protected virtual void LoadSettingsCore()
        {
        }

        protected virtual void SaveSettingsCore()
        {
        }

    }
}
