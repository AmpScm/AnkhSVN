using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;

namespace Ankh.UI.Controls
{
	class StatusPanelDesigner : ScrollableControlDesigner
	{
		StatusPanel _panel;

		public override void Initialize(System.ComponentModel.IComponent component)
		{
			base.Initialize(component);

			_panel = (StatusPanel)component;
		}

		public override bool CanBeParentedTo(System.ComponentModel.Design.IDesigner parentDesigner)
		{
			return (parentDesigner != null) && parentDesigner.Component is StatusContainer;
		}

		public override SelectionRules SelectionRules
		{
			get
			{
				return base.SelectionRules & (SelectionRules.BottomSizeable | SelectionRules.Visible);
			}
		}

		DesignerVerbCollection _verbs;
		public override System.ComponentModel.Design.DesignerVerbCollection Verbs
		{
			get
			{
				_verbs = new DesignerVerbCollection();
				return _verbs;
			}
		}
	}
}
