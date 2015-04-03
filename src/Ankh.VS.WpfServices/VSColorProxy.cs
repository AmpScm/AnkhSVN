using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;

namespace Ankh.VS.WpfServices
{
    public enum VSColorType
    {
        ForegroundColor,
        BackgroundColor,
        ForegroundBrush,
        BackgroundBrush
    }
    public static class VSColorProxy
    {
        delegate object MakeKey(Guid id, string name, VSColorType type);
        static readonly MakeKey _makeKey;
        static VSColorProxy()
        {
            if (!VSVersion.VS2012OrLater)
            {
                VSColorProxyResolver resolver = new VSColorProxyResolver();
                Application.Current.Resources.MergedDictionaries.Add(resolver);
                _makeKey = resolver.MakeKey;
            }
            else
            {
                Type themeResourceKeyType = VSAssemblies.VSShell11Immutable.GetType("Microsoft.VisualStudio.Shell.ThemeResourceKey");
                Type themeResourceKeyTypeType = VSAssemblies.VSShell11Immutable.GetType("Microsoft.VisualStudio.Shell.ThemeResourceKeyType");

                ConstructorInfo cif = themeResourceKeyType.GetConstructor(new Type[] { typeof(Guid), typeof(String), themeResourceKeyTypeType });

                _makeKey = delegate(Guid id, string key, VSColorType v)
                {
                    return cif.Invoke(new object[] { id, key, Enum.ToObject(themeResourceKeyTypeType, (int)v) }) ?? 1;
                };
            }
        }

        #region TreeView
        static readonly Guid _treeViewCategory = new Guid("92ecf08e-8b13-4cf4-99e9-ae2692382185");

        public static object TreeViewSelectedItemActiveBrushKey
        {
            get { return _makeKey(_treeViewCategory, "SelectedItemActive", VSColorType.BackgroundBrush); }
        }

        public static object TreeViewSelectedItemActiveTextBrushKey
        {
            get { return _makeKey(_treeViewCategory, "SelectedItemActive", VSColorType.ForegroundBrush); }
        }
        public static object TreeViewSelectedItemInactiveBrushKey
        {
            get { return _makeKey(_treeViewCategory, "SelectedItemInactive", VSColorType.BackgroundBrush); }
        }
        public static object TreeViewSelectedItemInactiveTextBrushKey
        {
            get { return _makeKey(_treeViewCategory, "SelectedItemInactive", VSColorType.ForegroundBrush); }
        }

        public static object TreeViewBackgroundBrushKey
        {
            get { return _makeKey(_treeViewCategory, "Background", VSColorType.BackgroundBrush); }
        }
        #endregion

        #region Environment
        static readonly Guid _environmentCategory = new Guid("624ed9c3-bdfd-41fa-96c3-7c824ea32e3d");
        public static object EnvironmentCommandBarTextActiveBrushKey
        {
            get { return _makeKey(_environmentCategory, "CommandBarTextActive", VSColorType.BackgroundBrush); }
        }
        #endregion

        #region Header
        static readonly Guid _headerCategory = new Guid("4997f547-1379-456e-b985-2f413cdfa536");
        public static object HeaderDefaultBrushKey
        {
            get { return _makeKey(_headerCategory, "Default", VSColorType.BackgroundBrush); }
        }
        public static object HeaderDefaultTextBrushKey
        {
            get { return _makeKey(_headerCategory, "Default", VSColorType.ForegroundBrush); }
        }
        public static object HeaderGlyphBrushKey
        {
            get { return _makeKey(_headerCategory, "Glyph", VSColorType.BackgroundBrush); }
        }
        public static object HeaderSeparatorLineBrushKey
        {
            get { return _makeKey(_headerCategory, "SeparatorLine", VSColorType.BackgroundBrush); }
        }
        public static object HeaderMouseDownBrushKey
        {
            get { return _makeKey(_headerCategory, "MouseDown", VSColorType.BackgroundBrush); }
        }
        public static object HeaderMouseDownTextBrushKey
        {
            get { return _makeKey(_headerCategory, "MouseDown", VSColorType.ForegroundBrush); }
        }
        public static object HeaderMouseDownGlyphBrushKey
        {
            get { return _makeKey(_headerCategory, "MouseDownGlyph", VSColorType.BackgroundBrush); }
        }
        public static object HeaderMouseOverBrushKey
        {
            get { return _makeKey(_headerCategory, "MouseOver", VSColorType.BackgroundBrush); }
        }
        public static object HeaderMouseOverTextBrushKey
        {
            get { return _makeKey(_headerCategory, "MouseOver", VSColorType.ForegroundBrush); }
        }
        public static object HeaderMouseOverGlyphBrushKey
        {
            get { return _makeKey(_headerCategory, "MouseOverGlyph", VSColorType.BackgroundBrush); }
        }
        
        #endregion

        public static void Ensure()
        {
        }
    }
}
