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

using Ankh.Commands;
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.VS;
using Ankh.Scc;
using Ankh.IssueTracker;
using Ankh.UI.PendingChanges;

namespace Ankh.UI.IssueTracker
{
    [SvnCommand(AnkhCommand.SolutionIssueTrackerSetup)]
    class IssueTrackerSetupCommand : ICommandHandler
    {
        #region ICommandHandler Members

        public void OnUpdate(CommandUpdateEventArgs e)
        {
            SvnItem item = GetRoot(e);
            IAnkhIssueService service = e.GetService<IAnkhIssueService>();
            int connectorCount =
                service == null || service.Connectors == null
                    ? 0
                    : service.Connectors.Count;

            bool available = IssueTrackerAvailabilityLogic.CanConfigure(
                item != null && item.IsVersioned,
                connectorCount);

            // The command is registered as defaultInvisible. Explicitly drive
            // visibility as well as enablement so it appears when a connector
            // is genuinely available and stays out of the menu otherwise.
            e.Enabled = available;
            e.Visible = available;
        }

        public void OnExecute(CommandEventArgs e)
        {
            SvnItem firstVersioned = null;
            ISvnStatusCache cache = e.GetService<ISvnStatusCache>();
            IAnkhSolutionSettings solutionSettings = e.GetService<IAnkhSolutionSettings>();
            if (solutionSettings != null)
            {
                firstVersioned = cache[solutionSettings.ProjectRoot];
            }

            if (firstVersioned == null)
                return; // exceptional case

            using (IssueTrackerConfigDialog dialog = new IssueTrackerConfigDialog(e.Context))
            {
                if (dialog.ShowDialog(e.Context) == System.Windows.Forms.DialogResult.OK)
                {
                    IIssueTrackerSettings currentSettings = e.GetService<IIssueTrackerSettings>();

                    IssueRepository newRepository = dialog.NewIssueRepository;
                    bool succeeded;

                    if (newRepository == null
                        || string.IsNullOrEmpty(newRepository.ConnectorName)
                        || newRepository.RepositoryUri == null)
                    {
                        succeeded = DeleteIssueRepositoryProperties(
                            e.Context,
                            firstVersioned);
                    }
                    else
                    {
                        IBuiltInIssueRepositoryPersistence builtIn =
                            newRepository as IBuiltInIssueRepositoryPersistence;
                        if (builtIn != null)
                            builtIn.Persist(firstVersioned);

                        succeeded = currentSettings == null
                            || currentSettings.ShouldPersist(newRepository)
                            ? SetIssueRepositoryProperties(
                                e.Context,
                                firstVersioned,
                                newRepository)
                            : true;
                    }

                    if (succeeded)
                    {
                        cache.MarkDirty(firstVersioned.FullPath);

                        IAnkhIssueService service =
                            e.GetService<IAnkhIssueService>();
                        if (service != null)
                            service.CurrentIssueRepository = newRepository;

                        PendingCommitsPage commits =
                            e.GetService<PendingCommitsPage>();
                        if (commits != null)
                            commits.RefreshIssueSettings();
                    }
                }
            }
        }

        #endregion

        private bool DeleteIssueRepositoryProperties(IAnkhServiceProvider context, SvnItem item)
        {
            return context.GetService<IProgressRunner>().RunModal("Removing Issue Repository settings",
                delegate(object sender, ProgressWorkerArgs wa)
                {
                    wa.Client.DeleteProperty(item.FullPath, AnkhSccPropertyNames.IssueRepositoryConnector);
                    wa.Client.DeleteProperty(item.FullPath, AnkhSccPropertyNames.IssueRepositoryUri);
                    wa.Client.DeleteProperty(item.FullPath, AnkhSccPropertyNames.IssueRepositoryId);
                    wa.Client.DeleteProperty(item.FullPath, AnkhSccPropertyNames.IssueRepositoryPropertyNames);
                    wa.Client.DeleteProperty(item.FullPath, AnkhSccPropertyNames.IssueRepositoryPropertyValues);
                }).Succeeded;
        }

        private bool SetIssueRepositoryProperties(IAnkhServiceProvider context, SvnItem item, IssueRepositorySettings settings)
        {
            return context.GetService<IProgressRunner>().RunModal("Applying Issue Repository settings",
                delegate(object sender, ProgressWorkerArgs wa)
                {
                    wa.Client.SetProperty(item.FullPath, AnkhSccPropertyNames.IssueRepositoryConnector, settings.ConnectorName);
                    wa.Client.SetProperty(item.FullPath, AnkhSccPropertyNames.IssueRepositoryUri, settings.RepositoryUri.ToString());
                    string repositoryId = settings.RepositoryId;
                    if (string.IsNullOrEmpty(repositoryId))
                    {
                        wa.Client.DeleteProperty(item.FullPath, AnkhSccPropertyNames.IssueRepositoryId);
                    }
                    else
                    {
                        wa.Client.SetProperty(item.FullPath, AnkhSccPropertyNames.IssueRepositoryId, settings.RepositoryId);
                    }
                    IDictionary<string, object> customProperties = settings.CustomProperties;
                    if (customProperties == null
                        || customProperties.Count == 0
                        )
                    {
                        wa.Client.DeleteProperty(item.FullPath, AnkhSccPropertyNames.IssueRepositoryPropertyNames);
                        wa.Client.DeleteProperty(item.FullPath, AnkhSccPropertyNames.IssueRepositoryPropertyValues);
                    }
                    else
                    {
                        string[] propNameArray = new string[customProperties.Keys.Count];
                        customProperties.Keys.CopyTo(propNameArray, 0);
                        string propNames = string.Join(",", propNameArray);

                        List<string> propValueList = new List<string>();
                        foreach (string propName in propNameArray)
                        {
                            object propValue;
                            if (!customProperties.TryGetValue(propName, out propValue))
                            {
                                propValue = string.Empty;
                            }
                            propValueList.Add(propValue == null ? string.Empty : propValue.ToString());
                        }
                        string propValues = string.Join(",", propValueList.ToArray());
                        wa.Client.SetProperty(item.FullPath, AnkhSccPropertyNames.IssueRepositoryPropertyNames, propNames);
                        wa.Client.SetProperty(item.FullPath, AnkhSccPropertyNames.IssueRepositoryPropertyValues, propValues);
                    }

                }).Succeeded;
        }

        /// <summary>
        /// Gets the "project root"
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static SvnItem GetRoot(BaseCommandEventArgs e)
        {
            SvnItem item = null;
            switch (e.Command)
            {
                case AnkhCommand.SolutionIssueTrackerSetup:
                    IAnkhSolutionSettings ss = e.GetService<IAnkhSolutionSettings>();
                    if (ss == null)
                        return null;

                    string root = ss.ProjectRoot;

                    if (string.IsNullOrEmpty(root))
                        return null;

                    item = e.GetService<ISvnStatusCache>()[root];
                    break;
            }

            return item;
        }
    }
}
