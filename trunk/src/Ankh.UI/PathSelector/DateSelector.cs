using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.PathSelector
{
    partial class DateSelector : UserControl
    {
        public DateSelector()
        {
            InitializeComponent();
        }

        DateTime _date;

        public DateTime Value
        {
            get { return _date = (datePicker.Value.Date + timePicker.Value.TimeOfDay); }
            set
            {
                _date = value;
                datePicker.Value = _date.Date;
                timePicker.Value = DateTime.Today + _date.TimeOfDay;
            }
        }

        public event EventHandler Changed;

        private void datePicker_ValueChanged(object sender, EventArgs e)
        {
            OnChanged(e);
        }

        private void timePicker_ValueChanged(object sender, EventArgs e)
        {
            OnChanged(e);
        }

        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }
    }
}
