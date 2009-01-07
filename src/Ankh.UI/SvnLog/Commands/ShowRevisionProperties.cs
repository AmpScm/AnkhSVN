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
using Ankh.Commands;
using Ankh.Ids;
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
        #region ICommandHandler Members

        public void OnUpdate(CommandUpdateEventArgs e)
        {
            int count = 0;
            foreach (ISvnLogItem i in e.Selection.GetSelection<ISvnLogItem>())
            {
                count++;

                if (count > 1)
                    break;
            }
            e.Enabled = count == 1;
        }

        public void OnExecute(CommandEventArgs e)
        {
            List<ISvnLogItem> logItems = new List<ISvnLogItem>(e.Selection.GetSelection<ISvnLogItem>());
            if (logItems.Count != 1)
                return;

            ILogControl logWindow = (ILogControl)e.Selection.ActiveDialogOrFrameControl;

            ISvnLogItem selectedLog = logItems[0];
            SvnUriTarget uriTarget = new SvnUriTarget(selectedLog.RepositoryRoot, selectedLog.Revision);
            using (PropertyEditorDialog dialog = new PropertyEditorDialog(uriTarget))
            {
                dialog.Context = e.Context;

                SvnRevisionPropertyListArgs args = new SvnRevisionPropertyListArgs();
                args.ThrowOnError = false;
                SvnPropertyCollection properties = null;

                if (e.GetService<IProgressRunner>().RunModal("Retrieving Revision Properties",
                    delegate(object sender, ProgressWorkerArgs wa)
                    {
                        wa.Client.GetRevisionPropertyList(uriTarget, args, out properties);
                    }).Succeeded && properties != null)
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
                if (dialog.ShowDialog(e.Context) == DialogResult.OK)
                {
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

                    ProgressRunnerResult result = progressRunner.RunModal("Updating Revision Properties",
                        delegate(object sender, ProgressWorkerArgs ee)
                        {
                            foreach (PropertyEditItem ei in finalItems)
                            {
                                if (!ei.ShouldPersist)
                                    continue;

                                if (ei.IsDeleted)
                                    ee.Client.DeleteRevisionProperty(uriTarget, ei.PropertyName);
                                else if (ei.Value.StringValue != null)
                                    ee.Client.SetRevisionProperty(uriTarget, ei.PropertyName, ei.Value.StringValue);
                                else
                                    ee.Client.SetRevisionProperty(uriTarget, ei.PropertyName, ei.Value.RawValue);
                            }
                        });

                    if (result.Succeeded)
                    {
                        logWindow.Restart();
                    }

                } // if
            } // using
        }

        #endregion
    }
}
