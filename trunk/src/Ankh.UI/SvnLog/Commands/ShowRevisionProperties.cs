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
            using (SvnClient client = e.GetService<ISvnClientPool>().GetClient())
            using (PropertyEditorDialog dialog = new PropertyEditorDialog(uriTarget))
            {
                dialog.Context = e.Context;

                SvnRevisionPropertyListArgs args = new SvnRevisionPropertyListArgs();
                args.ThrowOnError = false;
                SvnPropertyCollection properties;
                if (client.GetRevisionPropertyList(uriTarget, args, out properties))
                {
                    List<PropertyItem> propItems = new List<PropertyItem>();
                    foreach (SvnPropertyValue prop in properties)
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
                if (dialog.ShowDialog(e.Context) == DialogResult.OK)
                {
                    PropertyItem[] finalItems = dialog.PropertyItems;
                    Collection<SvnPropertyValue> deletedProps = new Collection<SvnPropertyValue>();

                    #region deleted props
                    if (properties.Count > 0)
                    {
                        foreach (SvnPropertyValue svnProp in properties)
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
                    }
                    #endregion

                    #region modified or new props
                    Collection<TextPropertyItem> modifiedOrNewProps = new Collection<TextPropertyItem>();
                    foreach (PropertyItem item in finalItems)
                    {
                        string key = item.Name;
                        if (properties.Contains(key))
                        {
                            string oldValue = properties[key].StringValue;
                            string newValue = ((TextPropertyItem)item).Text;
                            if (oldValue != newValue)
                            {
                                modifiedOrNewProps.Add((TextPropertyItem)item);
                            }
                        }
                        else // new property
                        {
                            modifiedOrNewProps.Add((TextPropertyItem)item);
                        }
                    }
                    #endregion

                    if (deletedProps.Count > 0
                        || modifiedOrNewProps.Count > 0)
                    {
                        IProgressRunner progressRunner = e.GetService<IProgressRunner>();

                        ProgressRunnerResult result = progressRunner.RunModal("Updating Revision Properties",
                            delegate(object sender, ProgressWorkerArgs ee)
                            {

                                foreach (SvnPropertyValue deletedProp in deletedProps)
                                {
                                    client.DeleteRevisionProperty(uriTarget, deletedProp.Key);
                                }

                                foreach (TextPropertyItem propItem in modifiedOrNewProps)
                                {
                                    string data = propItem.Text;
                                    client.SetRevisionProperty(uriTarget, propItem.Name, data);
                                }
                            }
                            );
                        if (result.Succeeded)
                        {
                            logWindow.Restart();
                        }
                    }
                } // if
            } // using
        }

        #endregion
    }
}
