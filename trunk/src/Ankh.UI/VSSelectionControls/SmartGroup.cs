using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Ankh.UI.VSSelectionControls
{
    [DebuggerDisplay("{Name}:{HeaderText}")]
    public class SmartGroup
    {
        readonly SmartListView _lv;
        readonly ListViewGroup _group;

        public SmartGroup(SmartListView listView)
            : this(listView, new ListViewGroup())
        {
            if(listView == null)
                throw new ArgumentNullException("listView");
        }

        SmartGroup(SmartListView listView, ListViewGroup group)
        {
            if(listView == null)
                throw new ArgumentNullException("listView");
            else if(group == null)
                throw new ArgumentNullException("group");

            Debug.Assert(group.Tag == null);
            _lv = listView;
            _group = group;
            _group.Tag = this;
        }

        public SmartGroup(SmartListView listView, string header)
            : this(listView, new ListViewGroup(header))
        {
        }

        public SmartGroup(SmartListView listView, string header, HorizontalAlignment headerAlignment)
            : this(listView, new ListViewGroup(header, headerAlignment))
        {
        }

        public SmartGroup(SmartListView listView, string key, string headerText)
            : this(listView, new ListViewGroup(key, headerText))
        {
        }

        public string Header
        {
            get { return _group.Header; }
            set { _group.Header = value; }
        }

        public HorizontalAlignment HeaderAlignment
        {
            get { return _group.HeaderAlignment; }
            set { _group.HeaderAlignment = value; }
        }

        public string Name
        {
            get { return _group.Name; }
            set { _group.Name = value; }
        }

        public ListView.ListViewItemCollection Items
        {
            get { return _group.Items; }
        }

        public SmartListView ListView
        {
            get { return _lv; }
        }

        public override string ToString()
        {
            return _group.ToString();
        }

        public static implicit operator ListViewGroup(SmartGroup me)
        {
            if(me == null)
                return null;
                
            return me._group;
        }

        public static explicit operator SmartGroup(ListViewGroup group)
        {
            if (group == null)
                return null;

            SmartGroup sg = group.Tag as SmartGroup;

            if (sg != null)
                return sg;
            else if (group.Tag != null)
                throw new InvalidOperationException();
            else if (group.ListView == null)
                throw new InvalidOperationException("No SmartGroup and no listview");

            return new SmartGroup((SmartListView)group.ListView, group);
        }
    }
}
