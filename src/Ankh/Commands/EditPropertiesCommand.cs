using System;
using System.Collections.Generic;
using System.Text;
using Ankh.UI;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using SharpSvn;
using System.Collections.ObjectModel;
using SharpSvn.Implementation;
using Ankh.Ids;

namespace Ankh.Commands
{
	[Command(AnkhCommand.ItemEditProperties,HideWhenDisabled=true)]
	class EditPropertiesCommand: CommandBase
	{
		public override void OnUpdate(CommandUpdateEventArgs e)
		{
			int count = 0;
			foreach (SvnItem i in e.Selection.GetSelectedSvnItems(false))
			{
				if (i.IsVersioned)
					count++;
			}
			e.Enabled = count == 1;
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
			using(SvnClient client = e.GetService<ISvnClientPool>().GetNoUIClient())
			using (PropertyEditorDialog dialog = new PropertyEditorDialog())
			{
				SvnPropertyListArgs args = new SvnPropertyListArgs();
				Collection<SvnPropertyListEventArgs> properties;
				if(client.GetPropertyList(firstVersioned.FullPath, args, out properties) && properties.Count == 1) // Handle single-file case for now
				{
					List<PropertyItem> propItems = new List<PropertyItem>();
					foreach(SvnPropertyValue prop in properties[0].Properties)
					{
						PropertyItem pi;
						if(prop.StringValue == null)
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
					// TODO: implement propset
				}
			}
		}
	}
}
