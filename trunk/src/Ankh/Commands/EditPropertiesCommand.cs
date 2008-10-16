using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using SharpSvn;
using Ankh.Ids;
using Ankh.Scc;
using Ankh.UI;
using Ankh.UI.PropertyEditors;
using Ankh.Selection;
using Ankh.VS;


namespace Ankh.Commands
{
    /// <remarks>
    /// If project/solution (logical) node is selected, target for this command is the project/solution (physical) folder.
    /// </remarks>
    [Command(AnkhCommand.ItemEditProperties, HideWhenDisabled = true)]
    [Command(AnkhCommand.ProjectEditProperties, HideWhenDisabled = true)]
    [Command(AnkhCommand.SolutionEditProperties, HideWhenDisabled = true)]
    class EditPropertiesCommand : CommandBase
    {
        /// <summary>
        /// Raises the <see cref="E:Update"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Ankh.Commands.CommandUpdateEventArgs"/> instance containing the event data.</param>
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            IFileStatusCache cache;

            int count = 0;
            switch (e.Command)
            {
                case AnkhCommand.ItemEditProperties:
                    foreach (SvnItem i in e.Selection.GetSelectedSvnItems(false))
                    {
                        if (i.IsVersioned)
                        {
                            count++;

                            if (count > 1)
                            {
                                if (e.Selection.IsSingleNodeSelection)
                                    break;
                                else
                                {
                                    e.Enabled = false;
                                    return;
                                }
                            }
                        }
                    }
                    break;
                case AnkhCommand.ProjectEditProperties:
                    IProjectFileMapper pfm = e.GetService<IProjectFileMapper>();
                    cache = e.GetService<IFileStatusCache>();
                    foreach (SvnProject project in e.Selection.GetSelectedProjects(false))
                    {
                        ISvnProjectInfo info = pfm.GetProjectInfo(project);
                        if (info == null)
                        {
                            e.Enabled = false;
                            return;
                        }
                        SvnItem projectFolder = cache[info.ProjectDirectory];

                        if(projectFolder.IsVersioned)
                            count++;

                        if (count > 1)
                            break;
                    }
                    break;
                case AnkhCommand.SolutionEditProperties:
                    cache = e.GetService<IFileStatusCache>();
                    IAnkhSolutionSettings solutionSettings = e.GetService<IAnkhSolutionSettings>();
                    if (solutionSettings == null || string.IsNullOrEmpty(solutionSettings.ProjectRoot))
                    {
                        e.Enabled = false;
                        return;
                    }
                    SvnItem solutionItem = cache[solutionSettings.ProjectRoot];
                    if(solutionItem.IsVersioned)
                        count = 1;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            if (count == 0 || (count > 1 && !e.Selection.IsSingleNodeSelection))
                e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            SvnItem firstVersioned = null;
            IFileStatusCache cache = e.GetService<IFileStatusCache>();

            switch (e.Command)
            {
                case AnkhCommand.ItemEditProperties:
                    foreach (SvnItem i in e.Selection.GetSelectedSvnItems(false))
                    {
                        if (i.IsVersioned)
                        {
                            firstVersioned = i;
                            break;
                        }
                    }
                    break;
                case AnkhCommand.ProjectEditProperties: // use project folder
                    foreach (SvnProject p in e.Selection.GetSelectedProjects(false))
                    {
                        IProjectFileMapper pfm = e.GetService<IProjectFileMapper>();
                        if (pfm != null)
                        {
                            ISvnProjectInfo info = pfm.GetProjectInfo(p);
                            if (info != null)
                            {
                                firstVersioned = cache[info.ProjectDirectory];
                            }
                            if (firstVersioned != null)
                            {
                                break;
                            }
                        }
                    }
                    break;
                case AnkhCommand.SolutionEditProperties: // use solution folder
                    IAnkhSolutionSettings solutionSettings = e.GetService<IAnkhSolutionSettings>();
                    if (solutionSettings != null)
                    {
                        firstVersioned = cache[solutionSettings.ProjectRoot];
                    }
                    break;
            }
            if (firstVersioned == null)
                return; // exceptional case

            IUIService ui = e.GetService<IUIService>();
            using (SvnClient client = e.GetService<ISvnClientPool>().GetNoUIClient())
            using (PropertyEditorDialog dialog = new PropertyEditorDialog(firstVersioned.FullPath))
            {
                dialog.Context = e.Context;

                SvnPropertyListArgs args = new SvnPropertyListArgs();
                Collection<SvnPropertyListEventArgs> properties;
                if (client.GetPropertyList(firstVersioned.FullPath, args, out properties) && properties.Count == 1) // Handle single-file case for now
                {
                    List<PropertyItem> propItems = new List<PropertyItem>();
                    foreach (SvnPropertyValue prop in properties[0].Properties)
                    {
                        PropertyItem pi;
                        if (prop.StringValue == null)
                            pi = new BinaryPropertyItem(prop.RawValue);
                        else
                            pi = new TextPropertyItem(prop.StringValue);

                        pi.Name = prop.Key;
                        propItems.Add(pi);
                    }
                    dialog.PropertyItems = propItems.ToArray();
                }
                if (ui.ShowDialog(dialog) == DialogResult.OK)
                {
                    if (properties.Count <= 1)
                    {
                        PropertyItem[] finalItems = dialog.PropertyItems;

                        #region perform delete
                        if (properties.Count > 0)
                        {
                            SvnPropertyListEventArgs propArgs = properties[0];
                            SvnPropertyCollection propCollection = propArgs.Properties;
                            Collection<SvnPropertyValue> deletedProps = new Collection<SvnPropertyValue>();
                            foreach (SvnPropertyValue svnProp in propCollection)
                            {
                                bool deleted = true;
                                foreach (PropertyItem item in finalItems)
                                {
                                    if (svnProp.Key.Equals(item.Name))
                                    {
                                        deleted = false;
                                        break;
                                    }
                                }
                                if (deleted)
                                {
                                    deletedProps.Add(svnProp);
                                }
                            }
                            foreach (SvnPropertyValue deletedProp in deletedProps)
                            {
                                client.DeleteProperty(firstVersioned.FullPath, deletedProp.Key);
                            }
                        }
                        #endregion

                        foreach (PropertyItem item in finalItems)
                        {
                            string key = item.Name;
                            if (item is BinaryPropertyItem)
                            {
                                ICollection<byte> data = ((BinaryPropertyItem)item).Data;
                                client.SetProperty(firstVersioned.FullPath, key, data);
                            }
                            else if (item is TextPropertyItem)
                            {
                                string data = ((TextPropertyItem)item).Text;
                                client.SetProperty(firstVersioned.FullPath, key, data);
                            }
                        }
                    }
                } // if
            } // using

            // TODO: this can be removed when switching to Subversion 1.6
            firstVersioned.MarkDirty();
            e.GetService<IFileStatusMonitor>().ScheduleMonitor(firstVersioned.FullPath);
        } // OnExecute
    }
}
