﻿// $Id: IssueTrackerConfigDialog.cs $
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Ankh.ExtensionPoints.IssueTracker;

namespace Ankh.UI.IssueTracker
{
    /// <summary>
    /// Issue Tracker Configuration dialog
    /// </summary>
    public partial class IssueTrackerConfigDialog : VSDialogForm
    {
        private IDictionary<string, IssueRepositoryConfigurationPage> _connectorPageMap;
        private IDictionary<string, Control> _connectorPageControlMap;
        private IssueRepositoryConfigurationPage _configPage;
        private IssueRepositoryConnector _connector;

        protected IssueTrackerConfigDialog()
        {
            InitializeComponent();
        }

        public IssueTrackerConfigDialog(IAnkhServiceProvider context)
        {
            InitializeComponent();
            base.Context = context;
            _connectorPageMap = new Dictionary<string, IssueRepositoryConfigurationPage>();
        }

        /// <summary>
        /// Gets the new issue repository to be associated with the current solution
        /// </summary>
        public IssueRepository NewIssueRepository
        {
            get
            {
                if (_connector != null && _configPage != null)
                {
                    IssueRepositorySettings newSettings = _configPage.Settings;
                    if (newSettings != null)
                    {
                        return _connector.Create(newSettings);
                    }
                }
                return null;
            }
        }

        private void IssueTrackerConfigDialog_Load(object sender, EventArgs e)
        {
            PopulateConnectors();
        }

        private void PopulateConnectors()
        {
            BindingList<IssueRepositoryConnector> dataSource = new BindingList<IssueRepositoryConnector>();
            dataSource.Add(new DummyConnector());
            if (Context != null)
            {
                IAnkhIssueService iService = Context.GetService<IAnkhIssueService>();
                if (iService != null)
                {
                    IssueRepositorySettings currentSettings = CurrentSolutionSettings;

                    ICollection<IssueRepositoryConnector> connectors = iService.Connectors;
                    if (connectors != null)
                    {
                        foreach (IssueRepositoryConnector connector in connectors)
                        {
                            dataSource.Add(connector);
                        }
                    }
                }
            }
            connectorComboBox.DataSource = dataSource;
            connectorComboBox.DisplayMember = "Name";
        }

        private bool firstActivated = true;
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            // apply current connector selection when dialog is first activated
            if (firstActivated)
            {
                firstActivated = false;
                IAnkhIssueService iService = Context.GetService<IAnkhIssueService>();
                if (iService != null)
                {
                    IssueRepositorySettings solutionSettings = CurrentSolutionSettings;
                    if (solutionSettings != null) {

                        IssueRepositoryConnector selectConnector;
                        if (iService.TryGetConnector(solutionSettings.ConnectorName, out selectConnector))
                        {
                            if (selectConnector != null)
                            {
                                connectorComboBox.SelectedItem = selectConnector;
                                return;
                            }
                        }
                    }
                }
                connectorComboBox.SelectedIndex = 0;
            }
        }

        delegate void DoSomething();

        private void connectorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            connectorComboBox.Enabled = false;

            try
            {
                if (_configPage != null)
                {
                    // remove the page event handler from the current page
                    _configPage.OnPageEvent -= new EventHandler<ConfigPageEventArgs>(_configPage_OnPageEvent);
                }

                _connector = connectorComboBox.SelectedItem as IssueRepositoryConnector;

                // if the dummy connector is selected and current settings are not null
                // enable ok button to enable removing existing settings
                // otherwise, disable ok button. it will be enabled when the selected connector page raises "page complete" event
                okButton.Enabled = CurrentSolutionSettings != null
                    && _connector is DummyConnector;

                UpdatePageFor(_connector);
            }
            finally
            {
                connectorComboBox.Enabled = true;
            }
        }

        /// <summary>
        /// Handles page events raised by the connector
        /// </summary>
        void _configPage_OnPageEvent(object sender, ConfigPageEventArgs e)
        {
            okButton.Enabled = e.IsComplete;
        }

        private void UpdatePageFor(IssueRepositoryConnector connector)
        {
            configPagePanel.Controls.Clear();

            bool needsCurrentSettings = false;
            _configPage = GetConfigurationPageFor(connector, ref needsCurrentSettings);

            if (_connectorPageControlMap == null)
            {
                _connectorPageControlMap = new Dictionary<string, Control>();
            }

            Control newControl = null;
            if (!_connectorPageControlMap.TryGetValue(connector.Name, out newControl))
            {
                newControl = GetControlFor(_configPage);
                newControl.Dock = DockStyle.Fill;
                _connectorPageControlMap.Add(connector.Name, newControl);
            }

            // setup config page
            if (_configPage != null)
            {
                _configPage.OnPageEvent += new EventHandler<ConfigPageEventArgs>(_configPage_OnPageEvent);
            }

            configPagePanel.Controls.Add(newControl);

            if (_configPage != null && needsCurrentSettings)
            {
                BeginInvoke((DoSomething)delegate()
                {
                    IssueRepositorySettings currentSettings = CurrentSolutionSettings;
                    if (currentSettings != null
                        && string.Equals(currentSettings.ConnectorName, connector.Name))
                    {
                        _configPage.Settings = currentSettings;
                    }
                });
            }
        }

        private IssueRepositoryConfigurationPage GetConfigurationPageFor(IssueRepositoryConnector connector, ref bool needsCurrentSettings)
        {
            if (connector == null)
            {
                return null;
            }
            if (_connectorPageMap == null)
            {
                _connectorPageMap = new Dictionary<string, IssueRepositoryConfigurationPage>();
            }
            IssueRepositoryConfigurationPage configPage = null;
            if (_connectorPageMap.ContainsKey(connector.Name))
            {
                configPage = _connectorPageMap[connector.Name];
            }
            else
            {
                Exception exception = null;
                try
                {
                    // triggers connector package initialization
                    configPage = connector.ConfigurationPage;
                }
                catch (Exception exc)
                {
                    exception = exc;
                }
                if (configPage == null)
                {
                    // use a dummy configuration page in case where connector does not provide a configuration page or
                    // if an exception is thrown while initializing the connector.
                    configPage = exception == null ?
                        new DummyIssueRepositoryConfigurationPage(string.Format("'{0}' does not provide a configuration page.", connector.Name))
                        : new DummyIssueRepositoryConfigurationPage(exception);
                }
                _connectorPageMap.Add(connector.Name, configPage);
                needsCurrentSettings = true;
            }
            return configPage;
        }

        /// <summary>
        /// Gets the settings associated with the current solution
        /// </summary>
        private IssueRepositorySettings CurrentSolutionSettings
        {
            get
            {
                IAnkhIssueService iService = Context == null ? null : Context.GetService<IAnkhIssueService>();
                IssueRepositorySettings settings = iService == null
                    ? null
                    : iService.CurrentIssueRepositorySettings;
                return settings == null
                    ? null
                    : string.IsNullOrEmpty(settings.ConnectorName)
                        ? null
                        : settings;
            }
        }

        private static Control GetControlFor(IssueRepositoryConfigurationPage configPage)
        {
            Control result = null;
            if (configPage != null)
            {
                IWin32Window window = configPage.Window;
                if (window != null)
                {
                    IntPtr handle = window.Handle;
                    if (handle != IntPtr.Zero)
                    {
                        result = Control.FromHandle(window.Handle);
                    }
                }
            }

            if (result == null)
            {
                result = new Label();
                result.Text = "Don't associate current solution with an issue repository.";
            }
            return result;
        }

        /// <summary>
        /// Placeholder for "None" selection
        /// </summary>
        private class DummyConnector : IssueRepositoryConnector
        {
            IssueRepositoryConfigurationPage _configPage;

            public override string Name
            {
                get { return "None"; }
            }

            public override IssueRepository Create(IssueRepositorySettings settings)
            {
                return null;
            }

            /// <summary>
            /// Returns an instamce of DummyIssueRepositoryConfigurationPage
            /// </summary>
            public override IssueRepositoryConfigurationPage ConfigurationPage
            {
                get
                {
                    if (_configPage == null)
                    {
                        System.Text.StringBuilder sb = new System.Text.StringBuilder("Don't associate this solution with an issue repository.");
                        _configPage = new DummyIssueRepositoryConfigurationPage(sb.ToString());
                    }
                    return _configPage;
                }
            }
        }

        /// <summary>
        /// Dummy configuration page to be used in case of connector package initialization exceptions
        /// </summary>
        private class DummyIssueRepositoryConfigurationPage : IssueRepositoryConfigurationPage
        {
            string _message = null;
            Exception _exception = null;
            Control _control = null;

            public DummyIssueRepositoryConfigurationPage(string message)
                : base()
            {
                _message = message;
            }

            public DummyIssueRepositoryConfigurationPage(Exception exception)
                : this(exception == null ? string.Empty : exception.Message)
            {
                _exception = exception;
            }

            public override IssueRepositorySettings Settings
            {
                get { return null; }
                set
                {
                    ConfigPageEventArgs args = new ConfigPageEventArgs();
                    args.IsComplete = false;
                    base.ConfigurationPageChanged(args);
                }
            }

            public override System.Windows.Forms.IWin32Window Window
            {
                get
                {
                    if (_control == null)
                    {
                        System.Text.StringBuilder sb = new System.Text.StringBuilder(_message);
                        if (_exception != null)
                        {
                            sb.AppendLine();
                            sb.AppendLine(_exception.StackTrace);
                        }
                        _control = new Label();
                        _control.Text = sb.ToString();
                    }
                    return _control;
                }
            }
        }
    }
}
