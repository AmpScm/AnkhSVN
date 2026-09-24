using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

using Ankh.UI;
using Ankh.VS;
using Ankh.ExtensionPoints.UI;
using Ankh.Services;
using Ankh.Commands;

namespace Ankh.WpfPackage.Services
{
    [GlobalService(typeof(IWinFormsThemingService), MinVersion = VSInstance.VS2012)]
    sealed partial class ThemingService : AnkhService, IWinFormsThemingService
    {
        readonly ConditionalWeakTable<ComboBox, PaletteComboBoxPainter> _comboPainters =
            new ConditionalWeakTable<ComboBox, PaletteComboBoxPainter>();
        readonly ConditionalWeakTable<NumericUpDown, PaletteNumericUpDownPainter> _numericPainters =
            new ConditionalWeakTable<NumericUpDown, PaletteNumericUpDownPainter>();

        public ThemingService(IAnkhServiceProvider context)
            : base(context)
        {

        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }


        public Color GetThemedColorValue(ref Guid colorCategory, string colorName, bool foreground)
        {
            Type vsUIShell5 = typeof(IVsUIShell5);

            if (vsUIShell5 == null)
                throw new InvalidOperationException();

            IVsUIShell5 vs5 = GetService<IVsUIShell5>(typeof(SVsUIShell));
            MethodInfo method = vsUIShell5.GetMethod("GetThemedColor");
            
            uint clr = vs5.GetThemedColor(ref colorCategory, colorName, foreground ? (uint)1 : 0);
            // TODO: Use bitshifting

            byte[] colorComponents = BitConverter.GetBytes(clr);
            return System.Drawing.Color.FromArgb(colorComponents[3], colorComponents[0], colorComponents[1], colorComponents[2]);
        }

        bool VSThemeWindow(IntPtr hwnd, bool forDialog)
        {
            var ui6 = GetService<IVsUIShell6>(typeof(SVsUIShell));


            if (ui6 != null)
            {
                bool r = ui6.ThemeWindow(hwnd);
                if (r && forDialog)
                    ui6.SetFixedThemeColors(hwnd);
                return r;
            }
            else
                return false;
        }

        delegate IVsUIObject GetIconForFile(string filename, __VSUIDATAFORMAT desiredFormat);
        GetIconForFile _giff;

        delegate IVsUIObject GetIconForFileEx(string filename, __VSUIDATAFORMAT desiredFormat, out uint iconSource);
        GetIconForFileEx _giffEx;

        [Obsolete]
        public bool TryGetIcon(string path, out IntPtr hIcon)
        {
            hIcon = IntPtr.Zero;

            if (_giff == null)
            {
                var imgs = GetService<IVsImageService2>(typeof(SVsImageService));

                if (imgs != null)
                {
                    _giff = imgs.GetIconForFile;
                    _giffEx = imgs.GetIconForFileEx;
                }
            }

            try
            {
                IVsUIObject uiOb;
                uint src = 0;

                if (_giffEx != null)
                    uiOb = _giffEx(path, __VSUIDATAFORMAT.VSDF_WIN32, out src);
                else
                    uiOb = _giff(path, __VSUIDATAFORMAT.VSDF_WIN32);

                if (src == 2 || uiOb == null)
                    return false; // Just use the os directly. (Allows caching)

                object data;
                if (!VSErr.Succeeded(uiOb.get_Data(out data)))
                    return false;

                IVsUIWin32Icon vsIcon = data as IVsUIWin32Icon;

                if (vsIcon == null)
                    return false;

                if (!VSErr.Succeeded(vsIcon.GetHICON(out var iconHandle)))
                    return false;

                hIcon = (IntPtr)iconHandle;

                return (hIcon != IntPtr.Zero);
            }
            catch { }

            return false;
        }

        public void ThemeRecursive(System.Windows.Forms.Control control, bool forDialog)
        {
            bool ownsPalette = _activeThemePalette == null;
            if (ownsPalette)
                _activeThemePalette = CreateThemePalette();

            try
            {
                ThemeRecursiveCore(control, forDialog);
            }
            finally
            {
                if (ownsPalette)
                    _activeThemePalette = null;
            }
        }

        void ThemeRecursiveCore(System.Windows.Forms.Control control, bool forDialog)
        {
            bool recurse = true;
            bool autoTheme = true;
            ISupportsVSTheming themeControl = control as ISupportsVSTheming;
            if (themeControl != null)
            {
                CancelEventArgs ca = new CancelEventArgs(false);
                themeControl.OnThemeChange(this, ca);

                if (ca.Cancel)
                    recurse = autoTheme = false;
            }

            IThemedControl themed = control as IThemedControl;
            if (themed != null)
            {
                ApplyThemeEventArgs atea = new ApplyThemeEventArgs(this, forDialog);

                themed.OnApplyTheme(atea);

                recurse = !atea.NoRecurse;
                autoTheme = !atea.DontTheme;
                forDialog = atea.ForDialog;
            }

            if (autoTheme && control.IsHandleCreated)
                VSThemeWindow(control, forDialog);

            if (recurse)
            {
                foreach (Control c in control.Controls)
                    ThemeRecursiveCore(c, forDialog);
            }
        }

        bool MaybeTheme<T>(Action<T> how, Control control, bool forDialog) where T : class
        {
            T value = control as T;
            if (value != null)
            {
                how(value);
                return true;
            }
            return false;
        }

        bool MaybeTheme<T>(Action<T, bool> how, Control control, bool forDialog) where T : class
        {
            T value = control as T;
            if (value != null)
            {
                how(value, forDialog);
                return true;
            }
            return false;
        }

        void ApplyNativeControlTheme(IntPtr handle, string darkTheme, bool forDialog, Color background)
        {
            if (handle == IntPtr.Zero)
                return;

            if (WinFormsNativeThemeLogic.ShouldUseDarkTheme(background, SystemInformation.HighContrast))
            {
                VSThemeWindow(handle, forDialog);
                NativeMethods.SetWindowTheme(handle, darkTheme, null);
            }
            else
            {
                // Remove a dark native sub-theme after a VS theme switch, then
                // let the shell apply its current light/blue/high-contrast theme.
                NativeMethods.SetWindowTheme(handle, null, null);
                VSThemeWindow(handle, forDialog);
            }
        }

        void ApplyNativeCaptionTheme(Form form)
        {
            if (form == null || !form.IsHandleCreated)
                return;

            int enabled = WinFormsNativeThemeLogic.ShouldUseDarkTheme(
                form.BackColor, SystemInformation.HighContrast) ? 1 : 0;

            try
            {
                int hr = NativeMethods.DwmSetWindowAttribute(
                    form.Handle,
                    NativeMethods.DWMWA_USE_IMMERSIVE_DARK_MODE,
                    ref enabled,
                    Marshal.SizeOf(typeof(int)));

                if (hr != 0)
                {
                    NativeMethods.DwmSetWindowAttribute(
                        form.Handle,
                        NativeMethods.DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1,
                        ref enabled,
                        Marshal.SizeOf(typeof(int)));
                }
            }
            catch (DllNotFoundException)
            {
            }
            catch (EntryPointNotFoundException)
            {
            }
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

            LinkLabel ll = label as LinkLabel;
            if (ll != null)
            {
                Color clrLink;

                if (VSColors.TryGetColor((__VSSYSCOLOREX)__VSSYSCOLOREX3.VSCOLOR_STARTPAGE_TEXT_CONTROL_LINK_SELECTED, out clrLink))
                    ll.LinkColor = clrLink;
            }

            if (label.BorderStyle == BorderStyle.Fixed3D)
                label.BorderStyle = BorderStyle.FixedSingle;
        }

        void ThemeOne(TextBox textBox)
        {
            if (textBox.Font != DialogFont)
                textBox.Font = DialogFont;

            AnkhThemePalette palette = ThemePalette;
            Color backColor = textBox.ReadOnly
                ? palette.SurfaceBackground
                : palette.InputBackground;
            Color foreColor = textBox.ReadOnly
                ? palette.SurfaceForeground
                : palette.InputForeground;

            if (textBox.BackColor != backColor)
                textBox.BackColor = backColor;

            if (textBox.ForeColor != foreColor)
                textBox.ForeColor = foreColor;

            if (textBox.BorderStyle == BorderStyle.Fixed3D)
                textBox.BorderStyle = BorderStyle.FixedSingle;
        }

        void ThemeOne(ListView listView, bool forDialog)
        {
            ApplyNativeControlTheme(
                listView.Handle,
                WinFormsNativeThemeLogic.DarkExplorerTheme,
                forDialog, ThemePalette.SurfaceBackground);

            if (listView.Font != DialogFont)
                listView.Font = DialogFont;

            AnkhThemePalette palette = ThemePalette;
            Color oldBack = listView.BackColor;
            Color oldFore = listView.ForeColor;
            Color newBack = palette.SurfaceBackground;
            Color newFore = palette.SurfaceForeground;
            bool updateBack = false;
            bool updateFore = false;

            if (oldBack != newBack)
            {
                listView.BackColor = newBack;
                updateBack = true;
            }

            if (oldFore != newFore)
            {
                listView.ForeColor = newFore;
                updateFore = true;
            }

            RestoreNativeListColors(listView);

            if (!listView.VirtualMode)
            {
                foreach (ListViewItem lvi in listView.Items)
                {
                    if (SystemInformation.HighContrast)
                    {
                        // Let Windows choose the item foreground in high-contrast
                        // mode, preserving accessibility behavior.
                        lvi.ForeColor = Color.Empty;

                        if (updateBack && lvi.BackColor == oldBack)
                            lvi.BackColor = newBack;

                        continue;
                    }

                    if (updateBack && (lvi.BackColor.IsEmpty || lvi.BackColor == oldBack))
                        lvi.BackColor = newBack;

                    Color effectiveBack = lvi.BackColor.IsEmpty
                        ? listView.BackColor
                        : lvi.BackColor;

                    Color preferredFore = lvi.ForeColor;
                    if (preferredFore.IsEmpty || (updateFore && preferredFore == oldFore))
                        preferredFore = newFore;

                    // Never leave a normal themed row at Color.Empty. Native
                    // ListView painting can resolve an empty item color through
                    // Windows instead of the active VS palette (the History
                    // Viewer exposed this as black text on a dark background).
                    // Preserve readable status/accent colors, otherwise fall
                    // back to the explicit semantic VS foreground.
                    lvi.ForeColor = AnkhThemePalette.ResolveReadableForeground(
                        preferredFore,
                        newFore,
                        effectiveBack);
                }
            }

            if (listView.BorderStyle == BorderStyle.Fixed3D)
                listView.BorderStyle = BorderStyle.FixedSingle;

            IntPtr header = NativeMethods.SendMessage(
                listView.Handle,
                NativeMethods.LVM_GETHEADER,
                IntPtr.Zero,
                IntPtr.Zero);

            if (header != IntPtr.Zero)
            {
                ApplyNativeControlTheme(
                    header,
                    WinFormsNativeThemeLogic.DarkItemsViewTheme,
                    forDialog, palette.SurfaceBackground);
            }
        }

        void ThemeOne(TreeView treeView, bool forDialog)
        {
            ApplyNativeControlTheme(
                treeView.Handle,
                WinFormsNativeThemeLogic.DarkExplorerTheme,
                forDialog, ThemePalette.SurfaceBackground);

            if (treeView.Font != DialogFont)
                treeView.Font = DialogFont;

            AnkhThemePalette palette = ThemePalette;

            if (treeView.BackColor != palette.SurfaceBackground)
                treeView.BackColor = palette.SurfaceBackground;

            if (treeView.ForeColor != palette.SurfaceForeground)
                treeView.ForeColor = palette.SurfaceForeground;

            if (treeView.BorderStyle == BorderStyle.Fixed3D)
                treeView.BorderStyle = BorderStyle.FixedSingle;

            RestoreNativeTreeColors(treeView);
        }

        // VS ThemeWindow can replace native colors without updating the managed
        // properties. Setting an unchanged WinForms property is a no-op, so send
        // the colors explicitly even when the managed palette already matches.
        internal static void RestoreNativeTreeColors(TreeView tree)
        {
            NativeMethods.SendMessage(tree.Handle, 0x1100 + 29, IntPtr.Zero,
                (IntPtr)ColorTranslator.ToWin32(tree.BackColor)); // TVM_SETBKCOLOR
            NativeMethods.SendMessage(tree.Handle, 0x1100 + 30, IntPtr.Zero,
                (IntPtr)ColorTranslator.ToWin32(tree.ForeColor)); // TVM_SETTEXTCOLOR
            tree.Invalidate();
        }

        internal static void RestoreNativeListColors(ListView list)
        {
            NativeMethods.SendMessage(list.Handle, 0x1000 + 1, IntPtr.Zero,
                (IntPtr)ColorTranslator.ToWin32(list.BackColor)); // LVM_SETBKCOLOR
            NativeMethods.SendMessage(list.Handle, 0x1000 + 38, IntPtr.Zero,
                (IntPtr)ColorTranslator.ToWin32(list.BackColor)); // LVM_SETTEXTBKCOLOR
            NativeMethods.SendMessage(list.Handle, 0x1000 + 36, IntPtr.Zero,
                (IntPtr)ColorTranslator.ToWin32(list.ForeColor)); // LVM_SETTEXTCOLOR
            list.Invalidate();
        }

        void ThemeOne(GroupBox group)
        {
            group.BackColor = ThemePalette.SurfaceBackground;
            group.ForeColor = ThemePalette.SurfaceForeground;
        }

        void ThemeOne(UserControl userControl)
        {
            if (userControl.Parent != null && userControl.Font != userControl.Parent.Font)
                userControl.Font = userControl.Parent.Font;

            AnkhThemePalette palette = ThemePalette;

            if (userControl.BackColor != palette.SurfaceBackground)
                userControl.BackColor = palette.SurfaceBackground;

            if (userControl.ForeColor != palette.SurfaceForeground)
                userControl.ForeColor = palette.SurfaceForeground;

            if (userControl.BorderStyle == BorderStyle.Fixed3D)
                userControl.BorderStyle = BorderStyle.FixedSingle;
        }

        private void ThemeOne(ContainerControl container)
        {
            if (container.Parent != null)
            {
            }
            else
                ThemeForm(container);
        }

        private void ThemeForm(ContainerControl form)
        {
            if (form.Parent != null && form.Font != form.Parent.Font)
                form.Font = form.Parent.Font;

            AnkhThemePalette palette = ThemePalette;

            if (form.BackColor != palette.SurfaceBackground)
                form.BackColor = palette.SurfaceBackground;

            if (form.ForeColor != palette.SurfaceForeground)
                form.ForeColor = palette.SurfaceForeground;

            ApplyNativeCaptionTheme(form as Form);
        }

        private void ThemeOne(ScrollableControl one)
        {

        }


        void ThemeOne(Panel panel)
        {
            if (panel.Parent != null && panel.Font != panel.Parent.Font)
                panel.Font = panel.Parent.Font;

            AnkhThemePalette palette = ThemePalette;

            if (panel.BackColor != palette.SurfaceBackground)
                panel.BackColor = palette.SurfaceBackground;

            if (panel.ForeColor != palette.SurfaceForeground)
                panel.ForeColor = palette.SurfaceForeground;

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

        private void ThemeOne(Button button, bool forDialog)
        {
            if (button.Parent != null && button.Font != button.Parent.Font)
                button.Font = button.Parent.Font;

            AnkhThemePalette palette = ThemePalette;
            bool paletteButton = !SystemInformation.HighContrast;

            Color foreColor = button.Enabled
                ? palette.InputForeground
                : palette.DisabledText;

            if (button.ForeColor != foreColor)
                button.ForeColor = foreColor;

            if (paletteButton)
            {
                button.UseVisualStyleBackColor = false;
                button.FlatStyle = FlatStyle.Flat;

                Color borderColor = palette.Border;
                Form owner = button.FindForm();

                if (owner != null
                    && ReferenceEquals(owner.AcceptButton, button)
                    && button.Enabled)
                {
                    borderColor = palette.FocusBorder;
                }

                if (button.BackColor != palette.InputBackground)
                    button.BackColor = palette.InputBackground;

                button.FlatAppearance.BorderSize = 1;
                button.FlatAppearance.BorderColor = borderColor;
                button.FlatAppearance.MouseOverBackColor = palette.HoverBackground;
                button.FlatAppearance.MouseDownBackColor = palette.PressedBackground;
            }
            else
            {
                button.FlatStyle = FlatStyle.Standard;
                button.UseVisualStyleBackColor = true;

                if (button.BackColor != palette.SurfaceBackground)
                    button.BackColor = palette.SurfaceBackground;

                if (button.IsHandleCreated)
                    VSThemeWindow(button.Handle, forDialog);
            }
        }

        const __VSSYSCOLOREX VSCOLOR_BRANDEDUI_TITLE = (__VSSYSCOLOREX)__VSSYSCOLOREX2.VSCOLOR_BRANDEDUI_TITLE;
        const __VSSYSCOLOREX VSCOLOR_BRANDEDUI_BORDER = (__VSSYSCOLOREX)__VSSYSCOLOREX2.VSCOLOR_BRANDEDUI_BORDER;
        const __VSSYSCOLOREX VSCOLOR_BRANDEDUI_TEXT = (__VSSYSCOLOREX)__VSSYSCOLOREX2.VSCOLOR_BRANDEDUI_TEXT;
        const __VSSYSCOLOREX VSCOLOR_BRANDEDUI_BACKGROUND = (__VSSYSCOLOREX)__VSSYSCOLOREX2.VSCOLOR_BRANDEDUI_BACKGROUND;
        const __VSSYSCOLOREX VSCOLOR_BRANDEDUI_FILL = (__VSSYSCOLOREX)__VSSYSCOLOREX2.VSCOLOR_BRANDEDUI_FILL;
        const __VSSYSCOLOREX VSCOLOR_GRAYTEXT = (__VSSYSCOLOREX)__VSSYSCOLOREX3.VSCOLOR_GRAYTEXT;
        const __VSSYSCOLOREX VSCOLOR_COMMANDBAR_TOOLBAR_SEPARATOR = (__VSSYSCOLOREX)__VSSYSCOLOREX3.VSCOLOR_COMMANDBAR_TOOLBAR_SEPARATOR;
        const __VSSYSCOLOREX VSCOLOR_THREEDFACE = (__VSSYSCOLOREX)__VSSYSCOLOREX3.VSCOLOR_THREEDFACE;

        IAnkhVSColor _vsColors;
        IAnkhVSColor VSColors
        {
            get { return _vsColors ?? (_vsColors = GetService<IAnkhVSColor>()); }
        }

        void ThemeOne(PropertyGrid grid)
        {
            AnkhThemePalette palette = ThemePalette;
            Color clrTitle;

            if (!VSColors.TryGetColor(VSCOLOR_BRANDEDUI_TITLE, out clrTitle))
                clrTitle = palette.SurfaceForeground;

            grid.BackColor = palette.SurfaceBackground;
            grid.HelpBackColor = palette.SurfaceBackground;
            grid.ViewBackColor = palette.SurfaceBackground;

            grid.ViewForeColor = palette.SurfaceForeground;
            grid.HelpForeColor = palette.SurfaceForeground;
            grid.LineColor = palette.Border;
            grid.CategoryForeColor = clrTitle;

            if (VSVersion.VS2012OrLater)
            {
                SetProperty(grid, "HelpBorderColor", palette.SurfaceBackground);
                SetProperty(grid, "ViewBorderColor", palette.SurfaceBackground);
                SetProperty(grid, "DisabledItemForeColor", palette.DisabledText);
                SetProperty(grid, "CategorySplitterColor", palette.Border);
                SetProperty(grid, "CanShowVisualStyleGlyphs", false);
            }
        }

        void ThemeOne(ComboBox combo, bool forDialog)
        {
            ApplyNativeControlTheme(
                combo.Handle,
                WinFormsNativeThemeLogic.DarkComboTheme,
                forDialog, ThemePalette.InputBackground);

            if (combo.Font != DialogFont)
                combo.Font = DialogFont;

            AnkhThemePalette palette = ThemePalette;

            if (combo.BackColor != palette.InputBackground)
                combo.BackColor = palette.InputBackground;

            if (combo.ForeColor != palette.InputForeground)
                combo.ForeColor = palette.InputForeground;

            combo.DrawItem -= ThemeComboDrawItem;

            if (!SystemInformation.HighContrast)
            {
                combo.FlatStyle = FlatStyle.Flat;
                combo.DrawMode = DrawMode.OwnerDrawFixed;
                combo.DrawItem += ThemeComboDrawItem;
            }
            else
            {
                combo.DrawMode = DrawMode.Normal;
                combo.FlatStyle = FlatStyle.Standard;
            }

            PaletteComboBoxPainter painter = _comboPainters.GetValue(
                combo,
                delegate(ComboBox value) { return new PaletteComboBoxPainter(value); });
            painter.SetTheme(
                !SystemInformation.HighContrast,
                palette.Border,
                palette.DisabledText);
        }

        void ThemeComboDrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            if (combo == null)
                return;

            bool editPortion = (e.State & DrawItemState.ComboBoxEdit) != 0;
            bool selected = (e.State & DrawItemState.Selected) != 0 && !editPortion;
            AnkhThemePalette palette = ThemePalette;

            Color backColor = selected
                ? palette.SelectionBackground
                : combo.BackColor;
            Color foreColor = selected
                ? palette.SelectionForeground
                : (combo.Enabled ? combo.ForeColor : palette.DisabledText);

            using (SolidBrush background = new SolidBrush(backColor))
                e.Graphics.FillRectangle(background, e.Bounds);

            string text = e.Index >= 0 && e.Index < combo.Items.Count
                ? combo.GetItemText(combo.Items[e.Index])
                : combo.Text;

            Rectangle textBounds = new Rectangle(
                e.Bounds.Left + 3,
                e.Bounds.Top,
                Math.Max(0, e.Bounds.Width - 6),
                e.Bounds.Height);

            TextRenderer.DrawText(
                e.Graphics,
                text,
                combo.Font,
                textBounds,
                foreColor,
                TextFormatFlags.Left |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis |
                TextFormatFlags.NoPrefix |
                TextFormatFlags.SingleLine);

            if ((e.State & DrawItemState.Focus) != 0 && !editPortion)
                e.DrawFocusRectangle();
        }

        void ThemeNumericUpDown(NumericUpDown numeric, bool forDialog)
        {
            ApplyNativeControlTheme(
                numeric.Handle,
                WinFormsNativeThemeLogic.DarkComboTheme,
                forDialog, ThemePalette.InputBackground);

            if (numeric.Font != DialogFont)
                numeric.Font = DialogFont;

            AnkhThemePalette palette = ThemePalette;

            if (numeric.BackColor != palette.InputBackground)
                numeric.BackColor = palette.InputBackground;

            if (numeric.ForeColor != palette.InputForeground)
                numeric.ForeColor = palette.InputForeground;

            if (numeric.BorderStyle == BorderStyle.Fixed3D)
                numeric.BorderStyle = BorderStyle.FixedSingle;

            foreach (Control child in numeric.Controls)
            {
                child.BackColor = palette.InputBackground;
                child.ForeColor = palette.InputForeground;

                if (child.IsHandleCreated)
                {
                    ApplyNativeControlTheme(
                        child.Handle,
                        WinFormsNativeThemeLogic.DarkComboTheme,
                        forDialog, child.BackColor);
                }
            }

            PaletteNumericUpDownPainter painter = _numericPainters.GetValue(
                numeric,
                delegate(NumericUpDown value) { return new PaletteNumericUpDownPainter(value); });
            painter.SetTheme(
                !SystemInformation.HighContrast,
                palette.Border,
                palette.DisabledText);
        }

        void ThemeOne(SplitContainer panel)
        {
            IHasSplitterColor ex = panel as IHasSplitterColor;
            if (ex != null)
                ThemeOne(ex);

            if (panel.Parent != null && panel.Font != panel.Parent.Font)
                panel.Font = panel.Parent.Font;

            AnkhThemePalette palette = ThemePalette;

            if (panel.BackColor != palette.SurfaceBackground)
            {
                panel.BackColor = palette.SurfaceBackground;
                panel.Panel1.BackColor = palette.SurfaceBackground;
                panel.Panel2.BackColor = palette.SurfaceBackground;
            }

            if (panel.ForeColor != palette.SurfaceForeground)
            {
                panel.ForeColor = palette.SurfaceForeground;
                panel.Panel1.ForeColor = palette.SurfaceForeground;
                panel.Panel2.ForeColor = palette.SurfaceForeground;
            }

            if (panel.BorderStyle == BorderStyle.Fixed3D)
                panel.BorderStyle = BorderStyle.FixedSingle;
        }

        void ThemeOne(IHasSplitterColor splitter)
        {
            splitter.SplitterColor = ThemePalette.Border;
        }

        private void SetProperty(PropertyGrid grid, string propertyName, object value)
        {
            PropertyInfo pi = typeof(PropertyGrid).GetProperty(propertyName);

            Debug.Assert(pi != null, "Grid Property exists");
            if (pi != null)
                pi.SetValue(grid, value, null);
        }

        static class NativeMethods
        {
            public const Int32 LVM_GETHEADER = 0x1000 + 31; // LVM_FIRST + 31;
            public const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
            public const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
            public static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

            [DllImport("dwmapi.dll")]
            public static extern int DwmSetWindowAttribute(
                IntPtr hwnd,
                int dwAttribute,
                ref int pvAttribute,
                int cbAttribute);
        }

        void VSThemeWindow(Control control, bool forDialog)
        {
            bool ok =
                MaybeTheme<ToolStrip>(ThemeOne, control, forDialog)
                || MaybeTheme<Label>(ThemeOne, control, forDialog)
                || MaybeTheme<GroupBox>(ThemeOne, control, forDialog)
                || MaybeTheme<TextBox>(ThemeOne, control, forDialog)
                || MaybeTheme<ListView>(ThemeOne, control, forDialog)
                || MaybeTheme<TreeView>(ThemeOne, control, forDialog)
                || MaybeTheme<Panel>(ThemeOne, control, forDialog)
                || MaybeTheme<UserControl>(ThemeOne, control, forDialog)
                || MaybeTheme<PropertyGrid>(ThemeOne, control, forDialog)
                || MaybeTheme<ComboBox>(ThemeOne, control, forDialog)
                || MaybeTheme<NumericUpDown>(ThemeNumericUpDown, control, forDialog)
                || MaybeTheme<SplitContainer>(ThemeOne, control, forDialog)
                || MaybeTheme<IHasSplitterColor>(ThemeOne, control, forDialog)
                || MaybeTheme<Button>(ThemeOne, control, forDialog)
                || MaybeTheme<ContainerControl>(ThemeOne, control, forDialog)
                || MaybeTheme<ScrollableControl>(ThemeOne, control, forDialog);

            // Controls without an Ankh-specific color adapter (for example
            // CheckBox, RadioButton and TabControl) should still use
            // Visual Studio's native theming instead of retaining Windows
            // light-theme rendering inside a themed dialog.
            if (!ok && control.IsHandleCreated)
                VSThemeWindow(control.Handle, forDialog);
        }
    }
}
