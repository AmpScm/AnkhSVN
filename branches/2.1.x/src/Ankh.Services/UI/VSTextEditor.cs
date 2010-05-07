using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;
using Microsoft.VisualStudio.OLE.Interop;

namespace Ankh.UI
{
    public partial class VSTextEditor : ContainerControl, IVSTextEditorImplementation, IAnkhHasVsTextView
    {
        Guid? _forceLanguageService;
        IVSTextEditorImplementation _implementation;
        bool _disableWordWrap;
        bool _hideHorizontalScrollBar;
        bool _enableSplitter;
        bool _enableNavBar;
        bool _readOnly;
        BorderStyle _borderStyle;
        string _text;

        public VSTextEditor()
        {
            InitializeComponent();
        }

        public VSTextEditor(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        void InitializeComponent()
        {
            BackColor = SystemColors.Window;
        }

        [Localizable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        [Category("Appearance"), DefaultValue(BorderStyle.None)]
        public BorderStyle BorderStyle
        {
            get
            {
                return _borderStyle;
            }
            set
            {
                if (_borderStyle != value)
                {
                    if (!Enum.IsDefined(typeof(BorderStyle), _borderStyle))
                    {
                        throw new InvalidEnumArgumentException("value", (int)value, typeof(BorderStyle));
                    }
                    _borderStyle = value;
                    if (!DesignMode)
                        base.UpdateStyles();
                }
            }
        }

        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                CreateParams createParams = base.CreateParams;
                // style = 0x56010000 = WS_CHILD | WS_VISIBLE | WS_CLIPSIBLINGS | WS_CLIPCHILDREN | WS_TABSTOP
                // exstyle = 0x10000 = WS_EX_CONTROLPARENT

                // Remove border settings
                createParams.ExStyle &= ~0x00000200; // WS_EX_CLIENTEDGE
                createParams.Style &= ~0x00800000; // WS_BORDER
                switch (_borderStyle)
                {
                    case BorderStyle.FixedSingle:
                        createParams.Style |= 0x00800000; // WS_BORDER
                        return createParams;

                    case BorderStyle.Fixed3D:
                        createParams.ExStyle |= 0x00000200; // WS_EX_CLIENTEDGE
                        return createParams;
                }
                return createParams;
            }
        }

        [DefaultValue(null), Localizable(false), DesignOnly(true)]
        public virtual Guid? ForceLanguageService
        {
            get { return _forceLanguageService; }
            set
            {
                _forceLanguageService = value;

                if (_implementation != null)
                    _implementation.ForceLanguageService = value;
            }
        }

        [DefaultValue(false), Localizable(false)]
        public bool DisableWordWrap
        {
            get { return _disableWordWrap; }
            set
            {
                _disableWordWrap = value;

                if (_implementation != null)
                    _implementation.DisableWordWrap = true;
            }
        }

        [DefaultValue(false), Localizable(false)]
        public bool HideHorizontalScrollBar
        {
            get { return _hideHorizontalScrollBar; }
            set
            {
                _hideHorizontalScrollBar = value;

                if (_implementation != null)
                    _implementation.HideHorizontalScrollBar = value;
            }
        }

        [DefaultValue(false), Localizable(false), DesignOnly(true)]
        public bool EnableSplitter
        {
            get { return _enableSplitter; }
            set
            {
                _enableSplitter = value;
                if (_implementation != null)
                    _implementation.EnableSplitter = value;
            }
        }

        [DefaultValue(false), Localizable(false), DesignOnly(true)]
        public bool EnableNavigationBar
        {
            get { return _enableNavBar; }
            set
            {
                _enableNavBar = value;

                if (_implementation != null)
                    _implementation.EnableNavigationBar = value;
            }
        }

        [DefaultValue(false), Localizable(false)]
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;

                if (_implementation != null)
                    _implementation.ReadOnly = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int LineHeight
        {
            get
            {
                if (DesignMode || _implementation == null)
                    return 0; // Designer scenario

                return _implementation.LineHeight;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true), DefaultValue("")]
        public override string Text
        {
            get
            {
                if (_implementation == null)
                    return _text ?? "";

                return (_text = _implementation.Text) ?? "";
            }
            set
            {
                _text = value;

                if (_implementation != null)
                    _implementation.Text = value;
            }
        }

        public void Clear(bool clearUndo)
        {
            _text = null;

            if (_implementation != null)
                _implementation.Clear(clearUndo);
        }

        public void SetInitialText(string text)
        {
            _text = text;

            if (_implementation != null)
                _implementation.SetInitialText(text);
        }

        public event EventHandler<VSTextEditorScrollEventArgs> HorizontalTextScroll;
        public event EventHandler<VSTextEditorScrollEventArgs> VerticalTextScroll;

        public void LoadFile(string path, bool monitorChanges)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (_implementation == null)
                throw new InvalidOperationException();

            _implementation.LoadFile(path, monitorChanges);
        }

        public Point EditorClientTopLeft
        {
            get
            {
                if (_implementation != null)
                    return _implementation.EditorClientTopLeft;

                return PointToScreen(new Point(0, 0));
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (DesignMode)
                return;

            IContextControl cc = TopLevelControl as IContextControl;
            IAnkhServiceProvider context = null;

            if (cc != null && cc.Context != null)
                context = cc.Context;
            else
            {
                Control c = Parent;

                while(c != null)
                {
                    cc = c as IContextControl;
                    if (cc != null)
                    {
                        context = cc.Context;
                        break;
                    }

                    context = c as IAnkhServiceProvider;
                    if (context != null)
                        break;

                    c = c.Parent;
                }
            }

            if (context == null)
                return;

            IVSTextEditorFactory factory = context.GetService<IVSTextEditorFactory>();

            if (factory != null && factory.TryInstantiateIn(this, out _implementation))
            {
                _implementation.HorizontalTextScroll += ImplementOnHorizontalTextScroll;
                _implementation.VerticalTextScroll += ImplementOnVerticalTextScroll;
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            try
            {
                if (_implementation != null)
                {
                    _text = _implementation.Text;
                    _implementation.HorizontalTextScroll -= ImplementOnHorizontalTextScroll;
                    _implementation.VerticalTextScroll -= ImplementOnVerticalTextScroll;
                }
                Controls.Clear();
                base.OnHandleDestroyed(e);
            }
            finally
            {
                _implementation = null;
            }
        }

        void ImplementOnHorizontalTextScroll(object sender, VSTextEditorScrollEventArgs e)
        {
            OnHorizontalTextScroll(e);
        }

        protected virtual void OnHorizontalTextScroll(VSTextEditorScrollEventArgs e)
        {
            if (HorizontalTextScroll != null)
                HorizontalTextScroll(this, e);
        }

        void ImplementOnVerticalTextScroll(object sender, VSTextEditorScrollEventArgs e)
        {
            OnVerticalTextScroll(e);
        }

        private void OnVerticalTextScroll(VSTextEditorScrollEventArgs e)
        {
            if (VerticalTextScroll != null)
                VerticalTextScroll(this, e);
        }

        public void PasteText(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (_implementation != null)
                _implementation.PasteText(text);
        }

        #region IAnkhHasVsTextView Members

        Microsoft.VisualStudio.TextManager.Interop.IVsTextView IAnkhHasVsTextView.TextView
        {
            get 
            {
                IAnkhHasVsTextView v = ActiveControl as IAnkhHasVsTextView;
                if (v != null)
                    return v.TextView;
                else
                    return null;
            }
        }

        [CLSCompliant(false), Browsable(false)]
        public Microsoft.VisualStudio.TextManager.Interop.IVsFindTarget FindTarget
        {
            get 
            {
                IAnkhHasVsTextView v = ActiveControl as IAnkhHasVsTextView;
                if (v != null)
                    return v.FindTarget;
                else
                    return null;
            }
        }

        #endregion

        [CLSCompliant(false), Browsable(false)]
        public IOleCommandTarget EditorCommandTarget
        {
            get
            {
                if (_implementation != null)
                    return _implementation.EditorCommandTarget;

                return null;
            }
        }
    }

    [CLSCompliant(false)]
    public interface IVSTextEditorImplementation
    {
        Guid? ForceLanguageService { get; set; }
        bool DisableWordWrap { get; set; }
        bool HideHorizontalScrollBar { get; set; }
        bool EnableSplitter { get; set; }
        bool EnableNavigationBar { get; set; }
        bool ReadOnly { get; set; }
        string Text { get; set; }

        int LineHeight { get; }
        void Clear(bool clearUndo);
        void SetInitialText(string text);
        Point EditorClientTopLeft { get; }

        event EventHandler<VSTextEditorScrollEventArgs> HorizontalTextScroll;
        event EventHandler<VSTextEditorScrollEventArgs> VerticalTextScroll;

        void LoadFile(string path, bool monitorChanges);

        void PasteText(string text);
        IOleCommandTarget EditorCommandTarget { get; }
    }

    [CLSCompliant(false)]
    public interface IVSTextEditorFactory
    {
        bool TryInstantiateIn(VSTextEditor editor, out IVSTextEditorImplementation implementation);
    }

    public class VSTextEditorScrollEventArgs : EventArgs
    {
        readonly int _iMinUnit;
        readonly int _iMaxUnits;
        readonly int _iVisibleUnits;
        readonly int _iFirstVisibleUnit;

        public VSTextEditorScrollEventArgs(int iMinUnit, int iMaxUnits, int iVisibleUnits, int iFirstVisibleUnit)
        {
            _iMinUnit = iMinUnit;
            _iMaxUnits = iMaxUnits;
            _iVisibleUnits = iVisibleUnits;
            _iFirstVisibleUnit = iFirstVisibleUnit;
        }

        public int MinUnit
        {
            get { return _iMinUnit; }
        }

        public int MaxUnit
        {
            get { return _iMaxUnits; }
        }

        public int VisibleUnits
        {
            get { return _iVisibleUnits; }
        }

        public int FirstVisibleUnit
        {
            get { return _iFirstVisibleUnit; }
        }
    }
}
