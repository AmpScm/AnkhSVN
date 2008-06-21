using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SharpSvn;
using Ankh.Scc;

namespace Ankh.UI
{
    public partial class LogMessageView : UserControl
    {
        ICurrentItemSource<ISvnLogItem> logItemSource;

        public LogMessageView()
        {
            InitializeComponent();
        }

        public LogMessageView(IContainer container)
            : this()
        {
            container.Add(this);
        }

        public ICurrentItemSource<ISvnLogItem> ItemSource
        {
            get { return logItemSource; }
            set 
            { 
                if(logItemSource != null)
                    logItemSource.FocusChanged -= new FocusChangedEventHandler<ISvnLogItem>(LogFocusChanged);

                logItemSource = value; 
                
                if(logItemSource != null)
                    logItemSource.FocusChanged += new FocusChangedEventHandler<ISvnLogItem>(LogFocusChanged);

            }
        }

        void LogFocusChanged(object sender, ISvnLogItem e)
        {
            if (ItemSource != null && ItemSource.FocusedItem != null)
                textBox1.Text = logItemSource.FocusedItem.LogMessage;
            else
                textBox1.Text = "";
        }

        internal void Reset()
        {
            textBox1.Text = "";
        }
    }
}
