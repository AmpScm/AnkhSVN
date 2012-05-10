using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Ankh.UI;
using Ankh.VS;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.WpfPackage.Services
{
    enum EnvironmentColor
    {
    }

    enum TreeViewColor
    {
        BackgroundColor,
        BackgroundBrush,
        BackgroundTextColor,
        BackgroundTextBrush,
        DragOverItemColor,
        DragOverItemBrush,
        DragOverItemTextColor,
        DragOverItemTextBrush,
        DragOverItemGlyphColor,
        DragOverItemGlyphBrush,
        DragOverItemGlyphMouseOverColor,
        DragOverItemGlyphMouseOverBrush,
        FocusVisualBorderColor,
        FocusVisualBorderBrush,
        GlyphColor,
        GlyphBrush,
        GlyphMouseOverColor,
        GlyphMouseOverBrush,
        HighlightedSpanColor,
        HighlightedSpanBrush,
        HighlightedSpanTextColor,
        HighlightedSpanTextBrush,
        SelectedItemActiveColor,
        SelectedItemActiveBrush,
        SelectedItemActiveTextColor,
        SelectedItemActiveTextBrush,
        SelectedItemActiveGlyphColor,
        SelectedItemActiveGlyphBrush,
        SelectedItemActiveGlyphMouseOverColor,
        SelectedItemActiveGlyphMouseOverBrush,
        SelectedItemInactiveColor,
        SelectedItemInactiveBrush,
        SelectedItemInactiveTextColor,
        SelectedItemInactiveTextBrush,
        SelectedItemInactiveGlyphColor,
        SelectedItemInactiveGlyphBrush,
        SelectedItemInactiveGlyphMouseOverColor,
        SelectedItemInactiveGlyphMouseOverBrush,
    }

    [GlobalService(typeof(IWinFormsThemingService), MinVersion = VSInstance.VS11)]
    sealed class VS11ThemingService : AnkhService, IWinFormsThemingService
    {
        public VS11ThemingService(IAnkhServiceProvider context)
            : base(context)
        {

        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            GetService<AnkhServiceEvents>().ThemeChanged += OnThemeChanged;
        }

        // This class does a lot of trickery in order not to break on different VS11 pre-release versions.
        // VS11 doesn't follow the COM guidelines yet and *changes* com interfaces between builds.
        //
        // * We can't just link against Microsoft.VisualStudio.Shell.Interop.11.0 as that would break VS2010 support
        // * We can't use the automatic PIA imports of .Net 4.0 as that breaks against newer builds
        //
        // So for the time being we use reflection. After VS11 goes to RTM we switch to the final API and
        // break support of the Beta versions
        delegate uint GetThemedColorType(ref Guid colorCategory, string colorName, uint colorType);
        GetThemedColorType _getThemedColor;

        private GetThemedColorType GetThemedColor
        {
            get { return _getThemedColor ?? FetchThemedColor(); }
        }

        private GetThemedColorType FetchThemedColor()
        {
            Type vsUIShell5 = Type.GetType("Microsoft.VisualStudio.Shell.Interop.IVsUIShell5, Microsoft.VisualStudio.Shell.Interop.11.0, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false);

            if (vsUIShell5 == null)
                throw new InvalidOperationException();

            object uiShell = GetService(typeof(SVsUIShell));
            MethodInfo method = vsUIShell5.GetMethod("GetThemedColor");

            _getThemedColor = (GetThemedColorType)Delegate.CreateDelegate(typeof(GetThemedColorType), uiShell, method, false);

            if (_getThemedColor == null)
            {
                _getThemedColor = delegate(ref Guid colorCategory, string colorName, uint colorType)
                {
                    return (uint)method.Invoke(uiShell, new object[] { colorCategory, colorName, colorType });
                };
            }

            return _getThemedColor;
        }

        public Color GetThemedColorValue(ref Guid colorCategory, string colorName, bool foreground)
        {
            uint clr = GetThemedColor(ref colorCategory, colorName, foreground ? (uint)1 : 0);
            // TODO: Use bitshifting

            byte[] colorComponents = BitConverter.GetBytes(clr);
            return System.Drawing.Color.FromArgb(colorComponents[3], colorComponents[0], colorComponents[1], colorComponents[2]);
        }

        static Guid EnvironmentCategory = new Guid("624ed9c3-bdfd-41fa-96c3-7c824ea32e3d");

        readonly Dictionary<EnvironmentColor, Tuple<Color?, Color?>> _colorMap = new Dictionary<EnvironmentColor, Tuple<Color?, Color?>>();
        public void OnThemeChanged(object sender, EventArgs e)
        {
            _colorMap.Clear();
        }

        public Color GetColor(EnvironmentColor color, bool foreground)
        {
            Tuple<Color?, Color?> clr;
            if (!_colorMap.TryGetValue(color, out clr)
                || !((foreground ? clr.Item2 : clr.Item1).HasValue))
            {
                if (clr == null)
                    clr = new Tuple<Color?, Color?>(null, null);

                Color value = GetThemedColorValue(ref EnvironmentCategory, color.ToString(), foreground);
                if (foreground)
                    clr = new Tuple<Color?, Color?>(clr.Item1, value);
                else
                    clr = new Tuple<Color?, Color?>(value, clr.Item1);
                _colorMap[color] = clr;
                return value;
            }

            if (foreground)
                return clr.Item2.Value;
            else
                return clr.Item1.Value;
        }

        public Color GetColor(EnvironmentColor color)
        {
            return GetColor(color, false);
        }

        delegate bool ThemeWindowDelegate(IntPtr handle);
        ThemeWindowDelegate _twd;

        bool VSThemeWindow(IntPtr handle)
        {
            if (_twd == null)
            {
                // Create a Linq expression for the call, as the IVsUIShell5 interface is not stable yet.
                object uiShell = GetService(typeof(SVsUIShell));
                Type IVsUIShell5Type = Type.GetType("Microsoft.VisualStudio.Shell.Interop.IVsUIShell5, Microsoft.VisualStudio.Shell.Interop.11.0, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false);

                ParameterExpression prmHandle = Expression.Parameter(typeof(IntPtr), "handle");
                MethodInfo minfo = IVsUIShell5Type.GetMethod("ThemeWindow");
                MethodCallExpression mce = Expression.Call(Expression.Convert(Expression.Constant(uiShell), IVsUIShell5Type), minfo, prmHandle);

                _twd = Expression.Lambda<ThemeWindowDelegate>(mce, prmHandle).Compile();
            }

            return _twd(handle);
        }

        IUIService _ui;
        IUIService UI
        {
            get { return _ui ?? (_ui = GetService<IUIService>()); }
        }

        public void ThemeControl(System.Windows.Forms.Control control)
        {
            ISupportsVSTheming themeControl = control as ISupportsVSTheming;

            if (themeControl == null || themeControl.UseVSTheming)
            {
                VSThemeWindow(control.Handle);

                bool ok =
                    MaybeTheme<ToolStrip>(ThemeOne, control)
                    || MaybeTheme<Label>(ThemeOne, control)
                    || MaybeTheme<TextBox>(ThemeOne, control)
                    || MaybeTheme<ListView>(ThemeOne, control)
                    || MaybeTheme<TreeView>(ThemeOne, control)
                    || MaybeTheme<UserControl>(ThemeOne, control);

                if (themeControl != null)
                    themeControl.OnThemeChange(UI);
            }
            else if (themeControl != null)
                return; // No recurse!

            foreach (Control c in control.Controls)
            {
                ThemeControl(c);
            }
        }

        bool MaybeTheme<T>(Action<T> how, Control control) where T : Control
        {
            T value = control as T;
            if (value != null)
            {
                how(value);
                return true;
            }
            return false;
        }

        IAnkhVSColor _colorSvc;
        public IAnkhVSColor ColorSvc
        {
            get { return _colorSvc ?? (_colorSvc = GetService<IAnkhVSColor>()); }
        }

        IUIService _uiService;
        public IUIService UIService
        {
            get { return _uiService ?? (_uiService = GetService<IUIService>()); }
        }

        Font _dialogFont;
        public Font DialogFont
        {
            get { return _dialogFont ?? (_dialogFont = (Font)UIService.Styles["DialogFont"]); }
        }

        void ThemeOne(Label label)
        {
            if (label.Font != DialogFont)
                label.Font = DialogFont;

            if (label.BackColor != label.Parent.BackColor)
                label.BackColor = label.Parent.BackColor;

            if (label.ForeColor != label.Parent.ForeColor)
                label.ForeColor = label.Parent.ForeColor;
        }

        void ThemeOne(TextBox textBox)
        {
            if (textBox.Font != DialogFont)
                textBox.Font = DialogFont;

            if (textBox.ReadOnly && textBox.BackColor != textBox.Parent.BackColor)
                textBox.BackColor = textBox.Parent.BackColor;

            if (textBox.ForeColor != textBox.Parent.ForeColor)
                textBox.ForeColor = textBox.Parent.ForeColor;
        }

        void ThemeOne(ListView listView)
        {
            if (listView.Font != DialogFont)
                listView.Font = DialogFont;

            if (listView.BackColor != listView.Parent.BackColor)
                listView.BackColor = listView.Parent.BackColor;

            if (listView.ForeColor != listView.Parent.ForeColor)
                listView.ForeColor = listView.Parent.ForeColor;

            IntPtr header = NativeMethods.SendMessage(listView.Handle, NativeMethods.LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);

            if (header != IntPtr.Zero)
            {
                VSThemeWindow(header);

                // TODO: Force colors?
            }
            VSThemeWindow(listView.Handle);
        }

        void ThemeOne(TreeView treeView)
        {
            if (treeView.Font != DialogFont)
                treeView.Font = DialogFont;

            if (treeView.BackColor != treeView.Parent.BackColor)
                treeView.BackColor = treeView.Parent.BackColor;

            if (treeView.ForeColor != treeView.Parent.ForeColor)
                treeView.ForeColor = treeView.Parent.ForeColor;
        }

        void ThemeOne(UserControl userControl)
        {
            if (userControl.Parent != null && userControl.Font != userControl.Parent.Font)
                userControl.Font = userControl.Parent.Font;

            Color color;
            if (ColorSvc.TryGetColor(__VSSYSCOLOREX.VSCOLOR_TOOLWINDOW_BACKGROUND, out color))
            {
                if (userControl.BackColor != color)
                    userControl.BackColor = color;
            }

            if (ColorSvc.TryGetColor(__VSSYSCOLOREX.VSCOLOR_TOOLWINDOW_TEXT, out color))
            {
                if (userControl.ForeColor != color)
                    userControl.ForeColor = color;
            }
        }

        void ThemeOne(ToolStrip toolBar)
        {
            if (toolBar.Font != toolBar.Parent.Font)
                toolBar.Font = toolBar.Parent.Font;

            ToolStripRenderer renderer = UIService.Styles["VsRenderer"] as ToolStripRenderer;

            if (renderer != null)
                toolBar.Renderer = renderer;
        }

        static class NativeMethods
        {
            public const Int32 LVM_GETHEADER = 0x1000 + 31; // LVM_FIRST + 31

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        }
    }
}
