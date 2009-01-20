// $Id$
//
// Copyright 2008 The AnkhSVN Project
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
    [Command(AnkhCommand.ItemEditProperties)]
    [Command(AnkhCommand.ProjectEditProperties)]
    [Command(AnkhCommand.SolutionEditProperties)]
    [Command(AnkhCommand.ItemShowPropertyChanges)]
    class ItemEditPropertiesCommand : CommandBase
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
                case AnkhCommand.ItemShowPropertyChanges:
                    foreach (SvnItem i in e.Selection.GetSelectedSvnItems(false))
                    {
                        if (i.IsVersioned)
                        {
                            count++;

                            if (e.Command == AnkhCommand.ItemShowPropertyChanges
                                && !i.IsPropertyModified)
                            {
                                e.Enabled = false;
                                return;
                            }

                            if (e.Selection.IsSingleNodeSelection)
                                break;
                            else if (count > 1)
                            {
                                e.Enabled = false;
                                return;
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

                        if (projectFolder.IsVersioned)
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
                    if (solutionItem.IsVersioned)
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
                case AnkhCommand.ItemShowPropertyChanges:
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
                            if (info != null && info.ProjectDirectory != null)
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

            using (SvnClient client = e.GetService<ISvnClientPool>().GetNoUIClient())
            using (PropertyEditorDialog dialog = new PropertyEditorDialog(firstVersioned))
            {
                dialog.Context = e.Context;

                SortedList<string, PropertyEditItem> editItems = new SortedList<string, PropertyEditItem>();
                SvnPropertyListArgs args = new SvnPropertyListArgs();
                if (!e.GetService<IProgressRunner>().RunModal("Retrieving Properties",
                    delegate(object Sender, ProgressWorkerArgs wa)
                    {
                        // Retrieve base properties
                        client.PropertyList(new SvnPathTarget(firstVersioned.FullPath, SvnRevision.Base),
                            delegate(object s, SvnPropertyListEventArgs la)
                            {
                                foreach (SvnPropertyValue pv in la.Properties)
                                {
                                    PropertyEditItem ei;
                                    if (!editItems.TryGetValue(pv.Key, out ei))
                                        editItems.Add(pv.Key, ei = new PropertyEditItem(dialog.ListView, pv.Key));

                                    ei.BaseValue = pv;
                                }
                            });
                        //

                        client.PropertyList(firstVersioned.FullPath,
                            delegate(object s, SvnPropertyListEventArgs la)
                            {
                                foreach (SvnPropertyValue pv in la.Properties)
                                {
                                    PropertyEditItem ei;
                                    if (!editItems.TryGetValue(pv.Key, out ei))
                                        editItems.Add(pv.Key, ei = new PropertyEditItem(dialog.ListView, pv.Key));

                                    ei.OriginalValue = ei.Value = pv;
                                }
                            });


                    }).Succeeded)
                {
                    return; // Canceled
                }

                PropertyEditItem[] items = new PropertyEditItem[editItems.Count];
                editItems.Values.CopyTo(items, 0);
                dialog.PropertyValues = items;

                if (dialog.ShowDialog(e.Context) == DialogResult.OK)
                {
                    // Hack: Currently we save all properties, not only the in memory changed ones

                    items = dialog.PropertyValues;

                    bool hasChanges = false;
                    foreach (PropertyEditItem i in items)
                    {
                        if (i.ShouldPersist)
                        {
                            hasChanges = true;
                            break;
                        }
                    }

                    if (!hasChanges)
                        return;

                    try
                    {
                        e.GetService<IProgressRunner>().RunModal("Applying property changes",
                            delegate(object sender, ProgressWorkerArgs wa)
                            {
                                foreach (PropertyEditItem ei in items)
                                {
                                    if (!ei.ShouldPersist)
                                        continue;

                                    if (ei.Value == null)
                                    {
                                        if (ei.OriginalValue != null)
                                            wa.Client.DeleteProperty(firstVersioned.FullPath, ei.PropertyName);
                                    }
                                    else if (!ei.Value.ValueEquals(ei.OriginalValue))
                                    {
                                        if (ei.Value.StringValue != null)
                                            wa.Client.SetProperty(firstVersioned.FullPath, ei.PropertyName, ei.Value.StringValue);
                                        else
                                            wa.Client.SetProperty(firstVersioned.FullPath, ei.PropertyName, ei.Value.RawValue);
                                    }
                                }
                            });

                    } // if
                    finally
                    {
                        // TODO: this can be removed when switching to Subversion 1.6
                        e.GetService<IFileStatusMonitor>().ScheduleMonitor(firstVersioned.FullPath);
                        e.GetService<IFileStatusMonitor>().ScheduleSvnStatus(firstVersioned.FullPath);
                    }
                }
            }
        }
    }
}
