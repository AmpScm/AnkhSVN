using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Ankh.UI.VSSelectionControls
{
    public class SmartTreeView : TreeView
    {
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ExtendSelection(e.Location, true);
            }

            base.OnMouseDown(e);
        }

        protected virtual void ExtendSelection(Point p, bool rightClick)
        {
            TreeViewHitTestInfo hi = HitTest(p);

            bool onItem = hi.Node != null && hi.Location != TreeViewHitTestLocations.None;

            if (rightClick)
            {
                if (hi.Node != SelectedNode)
                {
                    SelectedNode = hi.Node;
                }
            }
        }
    }
}
