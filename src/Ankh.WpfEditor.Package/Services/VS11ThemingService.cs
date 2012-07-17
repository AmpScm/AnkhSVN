using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Hashtable = System.Collections.Hashtable;
using Microsoft.VisualStudio.Shell.Interop;

using Ankh.UI;
using Ankh.VS;
using Ankh.Commands;
using Microsoft.VisualStudio;

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

        delegate bool ThemeWindow(IntPtr handle);
        ThemeWindow _twd;

        bool VSThemeWindow(IntPtr handle)
        {
            if (_twd == null)
                _twd = GetInterfaceDelegate<ThemeWindow>(Type.GetType("Microsoft.VisualStudio.Shell.Interop.IVsUIShell5, Microsoft.VisualStudio.Shell.Interop.11.0, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), GetService(typeof(SVsUIShell)));
            return _twd(handle);
        }

        delegate IVsUIObject GetIconForFile(string filename, __VSUIDATAFORMAT desiredFormat);
        GetIconForFile _giff;

        delegate IVsUIObject GetIconForFileEx(string filename, __VSUIDATAFORMAT desiredFormat, out uint iconSource);
        GetIconForFileEx _giffEx;

        public bool TryGetIcon(string path, out IntPtr hIcon)
        {
            hIcon = IntPtr.Zero;

            if (_giff == null)
            {
                Type type_SVsImageService = Type.GetType("Microsoft.VisualStudio.Shell.Interop.SVsImageService, Microsoft.VisualStudio.Shell.Interop.11.0, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false);

                if (type_SVsImageService == null)
                    return false;

                object service = GetService(type_SVsImageService);

                if (service == null)
                    return false;

                _giff = GetInterfaceDelegate<GetIconForFile>(Type.GetType("Microsoft.VisualStudio.Shell.Interop.IVsImageService, Microsoft.VisualStudio.Shell.Interop.11.0, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), service);

                if (_giff == null)
                    return false;

                _giffEx = GetInterfaceDelegate<GetIconForFileEx>(Type.GetType("Microsoft.VisualStudio.Shell.Interop.IVsImageService, Microsoft.VisualStudio.Shell.Interop.11.0, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), service);
            }

            try
            {
                IVsUIObject uiOb;
                uint src = 0;

                if (_giffEx != null)
                    uiOb = _giffEx(path, __VSUIDATAFORMAT.VSDF_WIN32, out src);
                else
                    uiOb = _giff(path, __VSUIDATAFORMAT.VSDF_WIN32);

                if (src == 2)
                    return false; // Just use the os directly then. (Allows caching)

                object data;
                if (!ErrorHandler.Succeeded(uiOb.get_Data(out data)))
                    return false;

                IVsUIWin32Icon vsIcon = data as IVsUIWin32Icon;

                if (vsIcon == null)
                    return false;

                int iconHandle;
                if (!ErrorHandler.Succeeded(vsIcon.GetHICON(out iconHandle)))
                    return false;

                hIcon = (IntPtr)iconHandle;

                return (iconHandle != 0);
            }
            catch { }

            return false;
        }


        IUIService _ui;
        IUIService UI
        {
            get { return _ui ?? (_ui = GetService<IUIService>()); }
        }

        public void ThemeControl(System.Windows.Forms.Control control)
        {            
            DoThemeControl(control, true);
        }

        void DoThemeControl(System.Windows.Forms.Control control, bool delay)
        {
            if (!control.IsHandleCreated)
            {
                if (delay)
                    GetService<IAnkhCommandService>().PostIdleAction(
                        delegate
                        {
                            ThemeControl(control);
                        });
                return;
            }

            ISupportsVSTheming themeControl = control as ISupportsVSTheming;

            if (themeControl == null || themeControl.UseVSTheming)
            {
                if (themeControl != null)
                    themeControl.OnThemeChange(UI, this);

                VSThemeWindow(control);
            }
            else if (themeControl != null)
            {
                themeControl.OnThemeChange(UI, this);
                return; // No recurse!
            }

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

            if (label.BorderStyle == BorderStyle.Fixed3D)
                label.BorderStyle = BorderStyle.FixedSingle;
        }

        void ThemeOne(TextBox textBox)
        {
            if (textBox.Font != DialogFont)
                textBox.Font = DialogFont;

            if (textBox.ReadOnly && textBox.BackColor != textBox.Parent.BackColor)
                textBox.BackColor = textBox.Parent.BackColor;

            if (textBox.ForeColor != textBox.Parent.ForeColor)
                textBox.ForeColor = textBox.Parent.ForeColor;

            if (textBox.BorderStyle == BorderStyle.Fixed3D)
                textBox.BorderStyle = BorderStyle.FixedSingle;
        }

        void ThemeOne(ListView listView)
        {
            if (listView.Font != DialogFont)
                listView.Font = DialogFont;

            if (listView.BackColor != listView.Parent.BackColor)
                listView.BackColor = listView.Parent.BackColor;

            if (listView.ForeColor != listView.Parent.ForeColor)
                listView.ForeColor = listView.Parent.ForeColor;

            if (listView.BorderStyle == BorderStyle.Fixed3D)
                listView.BorderStyle = BorderStyle.FixedSingle;

            IntPtr header = NativeMethods.SendMessage(listView.Handle, NativeMethods.LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);

            if (header != IntPtr.Zero)
            {
                VSThemeWindow(header);

                // TODO: Force colors?
            }
        }

        void ThemeOne(TreeView treeView)
        {
            VSThemeWindow(treeView.Handle);

            if (treeView.Font != DialogFont)
                treeView.Font = DialogFont;

            if (treeView.BackColor != treeView.Parent.BackColor)
                treeView.BackColor = treeView.Parent.BackColor;

            if (treeView.ForeColor != treeView.Parent.ForeColor)
                treeView.ForeColor = treeView.Parent.ForeColor;

            if (treeView.BorderStyle == BorderStyle.Fixed3D)
                treeView.BorderStyle = BorderStyle.FixedSingle;
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

            if (userControl.BorderStyle == BorderStyle.Fixed3D)
                userControl.BorderStyle = BorderStyle.FixedSingle;
        }

        void ThemeOne(Panel panel)
        {
            if (panel.Parent != null && panel.Font != panel.Parent.Font)
                panel.Font = panel.Parent.Font;

            Color color;
            if (ColorSvc.TryGetColor(__VSSYSCOLOREX.VSCOLOR_TOOLWINDOW_BACKGROUND, out color))
            {
                if (panel.BackColor != color)
                    panel.BackColor = color;
            }

            if (ColorSvc.TryGetColor(__VSSYSCOLOREX.VSCOLOR_TOOLWINDOW_TEXT, out color))
            {
                if (panel.ForeColor != color)
                    panel.ForeColor = color;
            }

            if (panel.BorderStyle == BorderStyle.Fixed3D)
                panel.BorderStyle = BorderStyle.FixedSingle;
        }

        void ThemeOne(ToolStrip toolBar)
        {
            if (toolBar.Font != toolBar.Parent.Font)
                toolBar.Font = toolBar.Parent.Font;

            ToolStripRenderer renderer = UIService.Styles["VsRenderer"] as ToolStripRenderer;

            if (renderer != null)
                toolBar.Renderer = renderer;
        }

        void ThemeOne(PropertyGrid grid)
        {
            IAnkhVSColor colorSvc = GetService<IAnkhVSColor>();

            const int VSCOLOR_BRANDEDUI_TITLE = -187;
            const int VSCOLOR_BRANDEDUI_BORDER = -188;
            const int VSCOLOR_BRANDEDUI_TEXT = -189;
            const int VSCOLOR_BRANDEDUI_BACKGROUND = -190;
            const int VSCOLOR_BRANDEDUI_FILL = -191;
            const int VSCOLOR_GRAYTEXT = -201;

            Color clrTitle, clrBorder, clrText, clrBackground, clrFill, clrGrayText;

            if (!colorSvc.TryGetColor((__VSSYSCOLOREX)VSCOLOR_BRANDEDUI_TITLE, out clrTitle))
                clrTitle = SystemColors.WindowText;
            if (!colorSvc.TryGetColor((__VSSYSCOLOREX)VSCOLOR_BRANDEDUI_BORDER, out clrBorder))
                clrBorder = SystemColors.WindowFrame;
            if (!colorSvc.TryGetColor((__VSSYSCOLOREX)VSCOLOR_BRANDEDUI_TEXT, out clrText))
                clrText = SystemColors.WindowText;
            if (!colorSvc.TryGetColor((__VSSYSCOLOREX)VSCOLOR_BRANDEDUI_BACKGROUND, out clrBackground))
                clrBackground = SystemColors.WindowFrame;
            if (!colorSvc.TryGetColor((__VSSYSCOLOREX)VSCOLOR_BRANDEDUI_FILL, out clrFill))
                clrFill = SystemColors.WindowText;
            if (!colorSvc.TryGetColor((__VSSYSCOLOREX)VSCOLOR_GRAYTEXT, out clrGrayText))
                clrGrayText = SystemColors.WindowText;

            grid.BackColor = clrFill;

            grid.HelpBackColor = clrFill;
            grid.HelpBorderColor = clrFill;
            grid.ViewBackColor = clrFill;
            grid.ViewBorderColor = clrFill;

            grid.ViewForeColor = clrText;
            grid.HelpForeColor = clrText;
            grid.DisabledItemForeColor = clrGrayText;

            grid.CategorySplitterColor = clrBackground;
            grid.LineColor = clrBackground;
            grid.CategoryForeColor = clrTitle;

            // The OS glyphs don't work in the dark theme. VS uses the same trick. (Unavailable in 4.0)
            if (VSVersion.VS11OrLater)
                typeof(PropertyGrid).GetProperty("CanShowVisualStyleGlyphs").SetValue(grid, false);
        }

        static class NativeMethods
        {
            public const Int32 LVM_GETHEADER = 0x1000 + 31; // LVM_FIRST + 31

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        }

        public void VSThemeWindow(Control control)
        {
            bool ok =
                MaybeTheme<ToolStrip>(ThemeOne, control)
                || MaybeTheme<Label>(ThemeOne, control)
                || MaybeTheme<TextBox>(ThemeOne, control)
                || MaybeTheme<ListView>(ThemeOne, control)
                || MaybeTheme<TreeView>(ThemeOne, control)
                || MaybeTheme<Panel>(ThemeOne, control)
                || MaybeTheme<UserControl>(ThemeOne, control)
                || MaybeTheme<PropertyGrid>(ThemeOne, control);
        }
    }
}
