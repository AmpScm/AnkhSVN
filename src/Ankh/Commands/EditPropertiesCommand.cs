using System;
using System.Collections.Generic;
using System.Text;
using Ankh.UI;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using SharpSvn;
using System.Collections.ObjectModel;
using SharpSvn;
using Ankh.Ids;
using Ankh.Scc;

namespace Ankh.Commands
{
    [Command(AnkhCommand.ItemEditProperties, HideWhenDisabled = true)]
    class EditPropertiesCommand : CommandBase
    {
        // TODO: We should probably add commands for projects and solutions specific too
        // To handle the common directory != project file case

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            int count = 0;
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

            if (count == 0 || (count > 1 && !e.Selection.IsSingleNodeSelection))
                e.Enabled = false;
        }
        public override void OnExecute(CommandEventArgs e)
        {
            SvnItem firstVersioned = null;

            foreach (SvnItem i in e.Selection.GetSelectedSvnItems(false))
            {
                if (i.IsVersioned)
                {
                    firstVersioned = i;
                    break;
                }
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
            e.GetService<IFileStatusMonitor>().ScheduleSvnStatus(firstVersioned.FullPath);
        } // OnExecute
    }
}
