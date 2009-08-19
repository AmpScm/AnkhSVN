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

using Ankh.Commands;
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.VS;
using Ankh.Scc;
using Ankh.IssueTracker;
using System.Collections.Generic;

namespace Ankh.UI.IssueTracker
{
    [Command(AnkhCommand.SolutionIssueTrackerSetup)]
    class IssueTrackerSetupCommand : ICommandHandler
    {
        #region ICommandHandler Members

        public void OnUpdate(CommandUpdateEventArgs e)
        {
            IAnkhIssueService service = e.GetService<IAnkhIssueService>(typeof(IAnkhIssueService));
            e.Enabled = true
                && service != null
                && service.Connectors != null
                && service.Connectors.Count > 0;
        }

        public void OnExecute(CommandEventArgs e)
        {
            SvnItem firstVersioned = null;
            IFileStatusCache cache = e.GetService<IFileStatusCache>();
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
                    if (newRepository == null
                        || string.IsNullOrEmpty(newRepository.ConnectorName)
                        || newRepository.RepositoryUri == null)
                    {
                        DeleteIssueRepositoryProperties(e.Context, firstVersioned);
                    }
                    else if (currentSettings == null
                        || currentSettings.ShouldPersist(newRepository))
                    {
                        SetIssueRepositoryProperties(e.Context, firstVersioned, newRepository);
                    }
                }
            }
        }

        #endregion

        private bool DeleteIssueRepositoryProperties(AnkhContext context, SvnItem item)
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

        private bool SetIssueRepositoryProperties(AnkhContext context, SvnItem item, IssueRepositorySettings settings)
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
    }
}
