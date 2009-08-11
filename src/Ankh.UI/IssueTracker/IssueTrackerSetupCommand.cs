using Ankh.Commands;
using Ankh.ExtensionPoints.IssueTracker;
using WizardFramework;
using Ankh.VS;
using Ankh.Scc;
using SharpSvn;
using Ankh.IssueTracker;
using System;
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

            IIssueTrackerSettings currentSettings = e.GetService<IIssueTrackerSettings>();
            bool updateIssueService = false;
            IssueRepository newRepository = null;

            // TODO pass the current settings (repository) to the dialog so that this command serves for "edit" purpose.
            using (IssueTrackerConfigWizardDialog dialog = new IssueTrackerConfigWizardDialog(e.Context))
            {
                if (dialog.ShowDialog(e.Context) == System.Windows.Forms.DialogResult.OK)
                {
                    newRepository = dialog.NewIssueRepository;
                    if (newRepository == null)
                    {
                        updateIssueService = DeleteIssueRepositoryProperties(e.Context, firstVersioned);
                    }
                    else if (currentSettings == null
                        || currentSettings.ShouldPersist(newRepository))
                    {
                        updateIssueService = SetIssueRepositoryProperties(e.Context, firstVersioned, newRepository);
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
                    wa.Client.SetProperty(item.FullPath, AnkhSccPropertyNames.IssueRepositoryId, settings.RepositoryId);
                    IDictionary<string, object> customProperties = settings.CustomProperties;
                    if (customProperties != null)
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
