using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.VSSelectionControls
{
    public class SmartListViewItem : ListViewItem
    {
        readonly SmartListView _view;

        public SmartListViewItem(SmartListView listView)
        {
            if (listView == null)
                throw new ArgumentNullException("listView");

            _view = listView;

        }
        string[] _values;

        public new SmartListView ListView
        {
            get { return _view; }
        }

        public void SetValues(params string[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            _values = values;

            if(ListView.AllColumns.Count > 0)
            {
                IEnumerator<SmartColumn> cols = ListView.AllColumns.GetEnumerator();
                System.Collections.IEnumerator vals = values.GetEnumerator();

                while(cols.MoveNext() && vals.MoveNext())
                {
                    int n = cols.Current.Index;

                    if(n < 0)
                        continue;

                    while(n >= SubItems.Count)
                        SubItems.Add("");

                    SubItems[n].Text = (string)vals.Current;
                }                
            }
            else
            {
                SubItems.Clear();

                int n = 0;
                foreach(string v in values)
                {
                    if(n < SubItems.Count)
                        SubItems[n++].Text = v;
                    else
                    {
                        SubItems.Add(v);
                        n++;
                    }
                }
            }

            UpdateGroup();
        }

        internal void UpdateGroup()
        {
            if(ListView.GroupColumns.Count > 0)
                ListView.UpdateGroup(this, _values);
            else
                Group = null;            
        }

        public void SetValue(int column, string value)
        {
            if (column < 0)
                throw new ArgumentOutOfRangeException("column");
            // Allow null values!

            bool checkGroup = false;
            int c = -1;
            if (ListView.AllColumns.Count > 0)
            {
                if (column >= ListView.AllColumns.Count)
                    throw new ArgumentOutOfRangeException("column");

                checkGroup = true;
                SmartColumn sc = ListView.AllColumns[column];

                if (sc.Index >= 0)
                    c = sc.Index;
            }
            else
            {
                if(column >= ListView.Columns.Count)
                    throw new ArgumentOutOfRangeException("column");

                c = column;
            }

            if(c >= 0)
            {
                while (c >= SubItems.Count)
                    SubItems.Add("");

                SubItems[c].Text = value;
            }

            if (_values == null || _values.Length < column)
            {
                string[] values = new string[column + 1];

                if (_values != null)
                    _values.CopyTo(values, 0);

                _values = values;
            }

            _values[column] = value;

            if (checkGroup && ListView.GroupColumns.Contains(ListView.AllColumns[column]))
            {
                UpdateGroup();
            }
        }

        public string GetValue(int column)
        {
            if (column < 0)
                throw new ArgumentOutOfRangeException("column");

            if (_values != null && column < _values.Length)
                return _values[column];

            SmartListView lv = ListView as SmartListView;
            if (lv != null && lv.AllColumns.Count > 0)
            {
                if (column >= lv.AllColumns.Count)
                    throw new ArgumentOutOfRangeException("column");

                column = lv.AllColumns[column].Index;
            }
            else if(ListView != null && ListView.Columns.Count <= column)
                throw new ArgumentOutOfRangeException("column");

            if (column < 0 || column > SubItems.Count)
                return null;

            return SubItems[column].Text;
        }
    }
}
