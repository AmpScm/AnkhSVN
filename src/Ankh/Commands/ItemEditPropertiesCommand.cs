// Copyright 2008-2009 The AnkhSVN Project
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
using System.Windows.Forms;
using SharpSvn;
using Ankh.Scc;
using Ankh.UI.PropertyEditors;
using Ankh.Selection;
using Ankh.VS;


namespace Ankh.Commands
{
    /// <remarks>
    /// If project/solution (logical) node is selected, target for this command is the project/solution (physical) folder.
    /// </remarks>
    [SvnCommand(AnkhCommand.ItemEditProperties)]
    [SvnCommand(AnkhCommand.ProjectEditProperties)]
    [SvnCommand(AnkhCommand.SolutionEditProperties)]
    [SvnCommand(AnkhCommand.ItemShowPropertyChanges)]
    class ItemEditPropertiesCommand : CommandBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e">The <see cref="Ankh.Commands.CommandUpdateEventArgs"/> instance containing the event data.</param>
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            ISvnStatusCache cache;

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
                            if (count > 1)
                            {
                                e.Enabled = false;
                                return;
                            }
                        }
                    }
                    break;
                case AnkhCommand.ProjectEditProperties:
                    IProjectFileMapper pfm = e.GetService<IProjectFileMapper>();
                    cache = e.GetService<ISvnStatusCache>();
                    foreach (SccProject project in e.Selection.GetSelectedProjects(false))
                    {
                        ISccProjectInfo info = pfm.GetProjectInfo(project);
                        if (info == null || string.IsNullOrEmpty(info.ProjectDirectory))
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
                    cache = e.GetService<ISvnStatusCache>();
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
            SvnItem firstVersioned = GetFirstVersionedItem(e);
            if (firstVersioned == null)
                return;

            using (PropertyEditorDialog dialog = new PropertyEditorDialog(firstVersioned))
            {
                dialog.Context = e.Context;

                PropertyEditItem[] items;
                if (!TryLoadProperties(e, firstVersioned, dialog, out items))
                    return;

                dialog.PropertyValues = items;

                if (dialog.ShowDialog(e.Context) != DialogResult.OK)
                    return;

                items = dialog.PropertyValues;

                List<bool> shouldPersist = new List<bool>(items.Length);
                foreach (PropertyEditItem item in items)
                    shouldPersist.Add(item.ShouldPersist);

                if (!ItemEditPropertiesLogic.HasPersistableChanges(shouldPersist))
                    return;

                StoreProperties(e, firstVersioned, items);
            }
        }

        private SvnItem GetFirstVersionedItem(CommandEventArgs e)
        {
            ISvnStatusCache cache = e.GetService<ISvnStatusCache>();

            switch (e.Command)
            {
                case AnkhCommand.ItemEditProperties:
                case AnkhCommand.ItemShowPropertyChanges:
                    foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
                    {
                        if (item.IsVersioned)
                            return item;
                    }
                    return null;

                case AnkhCommand.ProjectEditProperties:
                    IProjectFileMapper mapper = e.GetService<IProjectFileMapper>();
                    if (mapper == null)
                        return null;

                    foreach (SccProject project in e.Selection.GetSelectedProjects(false))
                    {
                        ISccProjectInfo info = mapper.GetProjectInfo(project);
                        if (info == null || info.ProjectDirectory == null)
                            continue;

                        SvnItem item = cache[info.ProjectDirectory];
                        if (item != null)
                            return item;
                    }
                    return null;

                case AnkhCommand.SolutionEditProperties:
                    IAnkhSolutionSettings solutionSettings =
                        e.GetService<IAnkhSolutionSettings>();

                    return solutionSettings != null
                        ? cache[solutionSettings.ProjectRoot]
                        : null;

                default:
                    return null;
            }
        }

        private bool TryLoadProperties(
            CommandEventArgs e,
            SvnItem firstVersioned,
            PropertyEditorDialog dialog,
            out PropertyEditItem[] items)
        {
            SortedList<string, PropertyEditItem> editItems =
                new SortedList<string, PropertyEditItem>();

            bool succeeded = e.GetService<IProgressRunner>()
                .RunModal(
                    CommandStrings.ReadingProperties,
                    delegate(object sender, ProgressWorkerArgs wa)
                    {
                        wa.Client.PropertyList(
                            new SvnPathTarget(
                                firstVersioned.FullPath,
                                SvnRevision.Base),
                            delegate(object s, SvnPropertyListEventArgs la)
                            {
                                foreach (SvnPropertyValue property in la.Properties)
                                {
                                    PropertyEditItem item;
                                    if (!editItems.TryGetValue(property.Key, out item))
                                    {
                                        item = new PropertyEditItem(
                                            dialog.ListView,
                                            property.Key);
                                        editItems.Add(property.Key, item);
                                    }

                                    item.BaseValue = property;
                                }
                            });

                        wa.Client.PropertyList(
                            firstVersioned.FullPath,
                            delegate(object s, SvnPropertyListEventArgs la)
                            {
                                foreach (SvnPropertyValue property in la.Properties)
                                {
                                    PropertyEditItem item;
                                    if (!editItems.TryGetValue(property.Key, out item))
                                    {
                                        item = new PropertyEditItem(
                                            dialog.ListView,
                                            property.Key);
                                        editItems.Add(property.Key, item);
                                    }

                                    item.OriginalValue = item.Value = property;
                                }
                            });
                    })
                .Succeeded;

            if (!succeeded)
            {
                items = null;
                return false;
            }

            items = new PropertyEditItem[editItems.Count];
            editItems.Values.CopyTo(items, 0);
            return true;
        }

        private void StoreProperties(
            CommandEventArgs e,
            SvnItem firstVersioned,
            PropertyEditItem[] items)
        {
            e.GetService<IProgressRunner>().RunModal(
                CommandStrings.StoringPropertyValues,
                delegate(object sender, ProgressWorkerArgs wa)
                {
                    foreach (PropertyEditItem item in items)
                    {
                        switch (ItemEditPropertiesLogic.GetPersistenceAction(
                            item.ShouldPersist,
                            item.OriginalValue,
                            item.Value))
                        {
                            case PropertyPersistenceAction.Delete:
                                wa.Client.DeleteProperty(
                                    firstVersioned.FullPath,
                                    item.PropertyName);
                                break;

                            case PropertyPersistenceAction.SetString:
                                wa.Client.SetProperty(
                                    firstVersioned.FullPath,
                                    item.PropertyName,
                                    item.Value.StringValue);
                                break;

                            case PropertyPersistenceAction.SetRaw:
                                wa.Client.SetProperty(
                                    firstVersioned.FullPath,
                                    item.PropertyName,
                                    item.Value.RawValue);
                                break;
                        }
                    }
                });
        }
    }
}
