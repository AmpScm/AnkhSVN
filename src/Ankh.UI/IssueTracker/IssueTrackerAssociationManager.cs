// Copyright 2026 The AnkhSVN Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.IssueTracker;
using Ankh.Scc;
using Ankh.UI.PendingChanges;
using Ankh.VS;

namespace Ankh.UI.IssueTracker
{
    internal static class IssueTrackerAssociationManager
    {
        // Removal deliberately removes only Ankh's association metadata.
        // Provider data/properties remain available for reuse and history.
        internal const bool DeleteLocalIssueDataOnRemove = false;
        internal const bool DeleteBugtraqPropertiesOnRemove = false;

        internal static bool Apply(
            IAnkhServiceProvider context,
            IssueRepository newRepository)
        {
            if (context == null)
                return false;

            ISvnStatusCache cache = context.GetService<ISvnStatusCache>();
            IAnkhSolutionSettings solutionSettings =
                context.GetService<IAnkhSolutionSettings>();

            if (cache == null
                || solutionSettings == null
                || string.IsNullOrEmpty(solutionSettings.ProjectRoot))
            {
                return false;
            }

            SvnItem projectRoot = cache[solutionSettings.ProjectRoot];
            if (projectRoot == null || !projectRoot.IsVersioned)
                return false;

            bool removing = newRepository == null
                || string.IsNullOrEmpty(newRepository.ConnectorName)
                || newRepository.RepositoryUri == null;

            bool succeeded;
            if (removing)
            {
                succeeded = DeleteAssociationProperties(
                    context,
                    projectRoot);
                newRepository = null;
            }
            else
            {
                IBuiltInIssueRepositoryPersistence builtIn =
                    newRepository as IBuiltInIssueRepositoryPersistence;
                if (builtIn != null)
                    builtIn.Persist(projectRoot);

                IIssueTrackerSettings currentSettings =
                    context.GetService<IIssueTrackerSettings>();

                succeeded = currentSettings == null
                    || currentSettings.ShouldPersist(newRepository)
                    ? SetAssociationProperties(
                        context,
                        projectRoot,
                        newRepository)
                    : true;
            }

            if (!succeeded)
                return false;

            cache.MarkDirty(projectRoot.FullPath);

            IAnkhIssueService service =
                context.GetService<IAnkhIssueService>();
            if (service != null)
                service.CurrentIssueRepository = newRepository;

            PendingCommitsPage commits =
                context.GetService<PendingCommitsPage>();
            if (commits != null)
                commits.RefreshIssueSettings();

            return true;
        }

        static bool DeleteAssociationProperties(
            IAnkhServiceProvider context,
            SvnItem item)
        {
            IProgressRunner runner = context.GetService<IProgressRunner>();
            if (runner == null)
                return false;

            return runner.RunModal(
                "Removing Issue Repository settings",
                delegate(object sender, ProgressWorkerArgs wa)
                {
                    wa.Client.DeleteProperty(
                        item.FullPath,
                        AnkhSccPropertyNames.IssueRepositoryConnector);
                    wa.Client.DeleteProperty(
                        item.FullPath,
                        AnkhSccPropertyNames.IssueRepositoryUri);
                    wa.Client.DeleteProperty(
                        item.FullPath,
                        AnkhSccPropertyNames.IssueRepositoryId);
                    wa.Client.DeleteProperty(
                        item.FullPath,
                        AnkhSccPropertyNames.IssueRepositoryPropertyNames);
                    wa.Client.DeleteProperty(
                        item.FullPath,
                        AnkhSccPropertyNames.IssueRepositoryPropertyValues);
                }).Succeeded;
        }

        static bool SetAssociationProperties(
            IAnkhServiceProvider context,
            SvnItem item,
            IssueRepositorySettings settings)
        {
            IProgressRunner runner = context.GetService<IProgressRunner>();
            if (runner == null)
                return false;

            return runner.RunModal(
                "Applying Issue Repository settings",
                delegate(object sender, ProgressWorkerArgs wa)
                {
                    wa.Client.SetProperty(
                        item.FullPath,
                        AnkhSccPropertyNames.IssueRepositoryConnector,
                        settings.ConnectorName);
                    wa.Client.SetProperty(
                        item.FullPath,
                        AnkhSccPropertyNames.IssueRepositoryUri,
                        settings.RepositoryUri.ToString());

                    string repositoryId = settings.RepositoryId;
                    if (string.IsNullOrEmpty(repositoryId))
                    {
                        wa.Client.DeleteProperty(
                            item.FullPath,
                            AnkhSccPropertyNames.IssueRepositoryId);
                    }
                    else
                    {
                        wa.Client.SetProperty(
                            item.FullPath,
                            AnkhSccPropertyNames.IssueRepositoryId,
                            repositoryId);
                    }

                    IDictionary<string, object> customProperties =
                        settings.CustomProperties;
                    if (customProperties == null
                        || customProperties.Count == 0)
                    {
                        wa.Client.DeleteProperty(
                            item.FullPath,
                            AnkhSccPropertyNames.IssueRepositoryPropertyNames);
                        wa.Client.DeleteProperty(
                            item.FullPath,
                            AnkhSccPropertyNames.IssueRepositoryPropertyValues);
                        return;
                    }

                    string[] propNameArray =
                        new string[customProperties.Keys.Count];
                    customProperties.Keys.CopyTo(propNameArray, 0);
                    string propNames =
                        string.Join(",", propNameArray);

                    List<string> propValueList = new List<string>();
                    foreach (string propName in propNameArray)
                    {
                        object propValue;
                        if (!customProperties.TryGetValue(
                            propName,
                            out propValue))
                        {
                            propValue = string.Empty;
                        }

                        propValueList.Add(
                            propValue == null
                                ? string.Empty
                                : propValue.ToString());
                    }

                    wa.Client.SetProperty(
                        item.FullPath,
                        AnkhSccPropertyNames.IssueRepositoryPropertyNames,
                        propNames);
                    wa.Client.SetProperty(
                        item.FullPath,
                        AnkhSccPropertyNames.IssueRepositoryPropertyValues,
                        string.Join(",", propValueList.ToArray()));
                }).Succeeded;
        }
    }
}
