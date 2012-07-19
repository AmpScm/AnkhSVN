using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Ankh.UI.VSSelectionControls
{
    public class SmartSplitContainer : SplitContainer, ISupportInitialize, IHasSplitterColor
    {
        public SmartSplitContainer()
        {
            InitializeComponent();
        }

        public SmartSplitContainer(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        void InitializeComponent()
        {
            base.SplitterWidth = 2;
        }

        void ISupportInitialize.BeginInit()
        {
            // Ignored for .Net 2.0 compatibility
            if (Environment.Version.Major >= 4)
            {
                GetType().InvokeMember("BeginInit", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance, null, this, null);
            }
        }

        void ISupportInitialize.EndInit()
        {
            // Ignored for .Net 2.0 compatibility
            if (Environment.Version.Major >= 4)
            {
                GetType().InvokeMember("EndInit", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance, null, this, null);
            }
        }

        // Handle Ctrl+(Shift+)Tab and Ctrl+Return as without Ctrl
        protected override bool ProcessDialogKey(Keys keyData)
        {
            Keys key = keyData & Keys.KeyCode;
            if (key == Keys.Tab && (keyData & Keys.Alt) == Keys.None)
            {
                keyData = keyData & ~Keys.Control;
            }
            else if (key == Keys.Return && (keyData & Keys.Modifiers) == Keys.Control)
            {
                keyData = keyData & ~Keys.Control;
            }
            return base.ProcessDialogKey(keyData);
        }

        bool _hasSplitterColor;
        Color _splitterColor;

        [DefaultValue(typeof(Color),"Transparent")]
        public System.Drawing.Color SplitterColor
        {
            get { return _hasSplitterColor ? _splitterColor : Color.Transparent; }
            set
            {
                if (value != Color.Transparent)
                {
                    _hasSplitterColor = true;
                    _splitterColor = value;
                }
                else
                    _hasSplitterColor = false;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            if (_hasSplitterColor)
            {
                using (SolidBrush sb = new SolidBrush(SplitterColor))
                    e.Graphics.FillRectangle(sb, SplitterRectangle);
            }
        }

        [DefaultValue(2)]
        [Localizable(true)]
        public new int SplitterWidth
        {
            get { return base.SplitterWidth; }
            set { base.SplitterWidth = value; }
        }
    }
}
