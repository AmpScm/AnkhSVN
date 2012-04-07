using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Ankh.UI;
using Ankh.VS;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell.Interop;

#if NOT
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

    enum StyleType
    {
        Unknown,
        ToolWindow,
        Dialog
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

        public void ThemeControl(System.Windows.Forms.Control control)
        {
            StyleType type = StyleType.Unknown;
            if (control is AnkhToolWindowControl)
                type = StyleType.ToolWindow;

            ThemeControl(control, type);
        }

        void ThemeControl(System.Windows.Forms.Control control, StyleType type)
        {
            bool ok =
                MaybeTheme<ToolStrip>(ThemeOne, control, type)
                || MaybeTheme<Label>(ThemeOne, control, type)
                || MaybeTheme<TextBox>(ThemeOne, control, type)
                || MaybeTheme<ListView>(ThemeOne, control, type)
                || MaybeTheme<UserControl>(ThemeOne, control, type);

            foreach (Control c in control.Controls)
            {
                ThemeControl(c, type);
            }
        }

        bool MaybeTheme<T>(Action<T, StyleType> how, Control control, StyleType type) where T : Control
        {
            T value = control as T;
            if (value != null)
            {
                how(value, type);
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

        void ThemeOne(Label label, StyleType type)
        {
            if (label.Font != DialogFont)
                label.Font = DialogFont;

            if (label.BackColor != label.Parent.BackColor)
                label.BackColor = label.Parent.BackColor;

            if (label.ForeColor != label.Parent.ForeColor)
                label.ForeColor = label.Parent.ForeColor;
        }

        void ThemeOne(TextBox textBox, StyleType type)
        {
            if (textBox.Font != DialogFont)
                textBox.Font = DialogFont;

            if (textBox.ReadOnly && textBox.BackColor != textBox.Parent.BackColor)
                textBox.BackColor = textBox.Parent.BackColor;

            if (textBox.ForeColor != textBox.Parent.ForeColor)
                textBox.ForeColor = textBox.Parent.ForeColor;
        }

        void ThemeOne(ListView listView, StyleType type)
        {
            if (listView.Font != DialogFont)
                listView.Font = DialogFont;

            Color color;
            //if (ColorSvc.TryGetColor(__VSSYSCOLOREX.
            if (listView.BackColor != listView.Parent.BackColor)
                listView.BackColor = listView.Parent.BackColor;

            if (listView.ForeColor != listView.Parent.ForeColor)
                listView.ForeColor = listView.Parent.ForeColor;
        }

        void ThemeOne(UserControl userControl, StyleType type)
        {
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

            if (userControl.Font != DialogFont)
                userControl.Font = DialogFont;
        }

        void ThemeOne(ToolStrip toolBar, StyleType type)
        {
            ToolStripRenderer renderer = UIService.Styles["VsToolWindowRenderer"] as ToolStripRenderer;

            if (renderer != null)
                toolBar.Renderer = renderer;

            if (!SystemInformation.HighContrast)
            {
                // We should use the VS colors instead of the ones provided by the OS
                IAnkhVSColor colorSvc = GetService<IAnkhVSColor>();

                Color color;
                if (colorSvc.TryGetColor(__VSSYSCOLOREX.VSCOLOR_COMMANDBAR_GRADIENT_MIDDLE, out color))
                {
                    toolBar.BackColor = color;
                    toolBar.OverflowButton.BackColor = color;
                }

                if (renderer == null && colorSvc.TryGetColor(__VSSYSCOLOREX.VSCOLOR_COMMANDBAR_HOVEROVERSELECTED, out color))
                {
                    toolBar.ForeColor = color;
                    toolBar.OverflowButton.ForeColor = color;
                }
            }
        }
    }
}
#endif