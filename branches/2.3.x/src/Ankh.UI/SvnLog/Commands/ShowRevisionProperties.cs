// $Id$
//
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
using Ankh.Commands;
using Ankh.Scc;
using Ankh.Scc.UI;
using System.Windows.Forms.Design;
using Ankh.UI.PropertyEditors;
using System.Collections.Generic;
using SharpSvn;
using System.Collections.ObjectModel;
using System.Windows.Forms;

namespace Ankh.UI.SvnLog.Commands
{
    [Command(AnkhCommand.LogShowRevisionProperties, AlwaysAvailable = true)]
    class ShowRevisionProperties : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            if (null == EnumTools.GetSingle(e.Selection.GetSelection<ISvnLogItem>()))
                e.Enabled = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            ISvnLogItem selectedLog = EnumTools.GetSingle(e.Selection.GetSelection<ISvnLogItem>());

            if (selectedLog == null)
                return;

            using (PropertyEditorDialog dialog = new PropertyEditorDialog(selectedLog.RepositoryRoot, selectedLog.Revision, true))
            {
                SvnRevisionPropertyListArgs args = new SvnRevisionPropertyListArgs();
                args.ThrowOnError = false;
                SvnPropertyCollection properties = null;

                if (!e.GetService<IProgressRunner>().RunModal(LogStrings.RetrievingRevisionProperties,
                    delegate(object sender, ProgressWorkerArgs wa)
                    {
                        if (!wa.Client.GetRevisionPropertyList(selectedLog.RepositoryRoot, selectedLog.Revision, args, out properties))
                            properties = null;
                    }).Succeeded)
                {
                    return;
                }
                else if (properties != null)
                {
                    List<PropertyEditItem> propItems = new List<PropertyEditItem>();
                    foreach (SvnPropertyValue prop in properties)
                    {
                        PropertyEditItem pi = new PropertyEditItem(dialog.ListView, prop.Key);
                        pi.OriginalValue = pi.Value = pi.BaseValue = prop;

                        propItems.Add(pi);
                    }
                    dialog.PropertyValues = propItems.ToArray();
                }

                if (dialog.ShowDialog(e.Context) != DialogResult.OK)
                    return;

                PropertyEditItem[] finalItems = dialog.PropertyValues;

                bool hasChanges = false;

                foreach (PropertyEditItem ei in finalItems)
                {
                    if (ei.ShouldPersist)
                    {
                        hasChanges = true;
                        break;
                    }
                }
                if (!hasChanges)
                    return;

                IProgressRunner progressRunner = e.GetService<IProgressRunner>();

                ProgressRunnerResult result = progressRunner.RunModal(LogStrings.UpdatingRevisionProperties,
                    delegate(object sender, ProgressWorkerArgs ee)
                    {
                        foreach (PropertyEditItem ei in finalItems)
                        {
                            if (!ei.ShouldPersist)
                                continue;

                            if (ei.IsDeleted)
                                ee.Client.DeleteRevisionProperty(selectedLog.RepositoryRoot, selectedLog.Revision, ei.PropertyName);
                            else if (ei.Value.StringValue != null)
                                ee.Client.SetRevisionProperty(selectedLog.RepositoryRoot, selectedLog.Revision, ei.PropertyName, ei.Value.StringValue);
                            else
                                ee.Client.SetRevisionProperty(selectedLog.RepositoryRoot, selectedLog.Revision, ei.PropertyName, ei.Value.RawValue);
                        }
                    });

                if (result.Succeeded)
                {
                    ILogControl logWindow = e.Selection.GetActiveControl<ILogControl>();

                    if (logWindow != null)
                        logWindow.Restart();
                }

            } // using
        }
    }
}
