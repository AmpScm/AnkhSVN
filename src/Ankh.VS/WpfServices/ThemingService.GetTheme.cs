using Ankh.Configuration;
using Microsoft.Win32;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Ankh.WpfPackage.Services
{

    partial class ThemingService
    {
        public bool GetCurrentTheme(out Guid themeGuid)
        {
            if (VSVersion.VS2013OrLater
                && GetThemeViaApi(out themeGuid))
            {
                return true;
            }

            return GetThemeViaRegistry(out themeGuid);
        }

        bool GetThemeViaRegistry(out Guid themeGuid)
        {
            IAnkhConfigurationService config = GetService<IAnkhConfigurationService>();
            themeGuid = Guid.Empty;

            if (config == null)
                return false;

            string currentTheme;

            using (RegistryKey rk = config.OpenVSUserKey("General"))
            {
                if (rk != null)
                    currentTheme = rk.GetValue("CurrentTheme") as string;
                else
                    currentTheme = null;
            }

            Guid parsed;
            if (!string.IsNullOrEmpty(currentTheme)
                && Guid.TryParse(currentTheme, out parsed))
            {
                themeGuid = parsed;
            }

            // Preserve the historical behavior: a readable VS configuration
            // with no explicit CurrentTheme still represents a known theme
            // state and is reported as Guid.Empty.
            return true;
        }

        object _themeService;
        PropertyInfo _currentThemeProperty;
        PropertyInfo _themeIdProperty;

        private bool GetThemeViaApi(out Guid themeGuid)
        {
            themeGuid = Guid.Empty;

            try
            {
                if (_themeService == null)
                {
                    IAnkhQueryService queryService = GetService<IAnkhQueryService>();
                    if (queryService == null)
                        return false;

                    _themeService = queryService.QueryService<object>(
                        new Guid("0D915B59-2ED7-472A-9DE8-9161737EA1C5"));
                }

                if (_themeService == null)
                    return false;

                object currentTheme;
                if (!ThemeReflectionLogic.TryGetPropertyValue(
                    _themeService,
                    "CurrentTheme",
                    ref _currentThemeProperty,
                    out currentTheme)
                    || currentTheme == null)
                {
                    return false;
                }

                object rawThemeId;
                if (!ThemeReflectionLogic.TryGetPropertyValue(
                    currentTheme,
                    "ThemeId",
                    ref _themeIdProperty,
                    out rawThemeId))
                {
                    return false;
                }

                if (rawThemeId is Guid)
                {
                    themeGuid = (Guid)rawThemeId;
                    return true;
                }

                string text = rawThemeId as string;
                Guid parsed;
                if (!string.IsNullOrEmpty(text)
                    && Guid.TryParse(text, out parsed))
                {
                    themeGuid = parsed;
                    return true;
                }
            }
            catch (COMException)
            {
                ResetThemeReflectionCache();
            }
            catch (TargetException)
            {
                ResetThemeReflectionCache();
            }
            catch (TargetInvocationException)
            {
                ResetThemeReflectionCache();
            }
            catch (AmbiguousMatchException)
            {
                ResetThemeReflectionCache();
            }
            catch (ArgumentException)
            {
                ResetThemeReflectionCache();
            }

            return false;
        }

        void ResetThemeReflectionCache()
        {
            _currentThemeProperty = null;
            _themeIdProperty = null;
        }
    }

    internal static class ThemeReflectionLogic
    {
        internal static bool TryGetPropertyValue(
            object target,
            string propertyName,
            ref PropertyInfo cachedProperty,
            out object value)
        {
            value = null;

            if (target == null || string.IsNullOrEmpty(propertyName))
                return false;

            Type targetType = target.GetType();

            if (!CanUseProperty(cachedProperty, targetType, propertyName))
            {
                cachedProperty = targetType.GetProperty(
                    propertyName,
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.Static);

                if (!CanUseProperty(cachedProperty, targetType, propertyName))
                    return false;
            }

            MethodInfo getter = cachedProperty.GetGetMethod();
            if (getter == null)
            {
                cachedProperty = null;
                return false;
            }

            object invocationTarget = getter.IsStatic ? null : target;

            try
            {
                value = cachedProperty.GetValue(invocationTarget, null);
                return true;
            }
            catch (TargetException)
            {
                cachedProperty = null;
                return false;
            }
            catch (TargetInvocationException)
            {
                cachedProperty = null;
                return false;
            }
            catch (ArgumentException)
            {
                cachedProperty = null;
                return false;
            }
        }

        static bool CanUseProperty(
            PropertyInfo property,
            Type targetType,
            string propertyName)
        {
            if (property == null
                || property.Name != propertyName
                || property.GetIndexParameters().Length != 0)
            {
                return false;
            }

            MethodInfo getter = property.GetGetMethod();
            if (getter == null)
                return false;

            if (getter.IsStatic)
                return true;

            Type declaringType = property.DeclaringType;
            return declaringType != null
                && declaringType.IsAssignableFrom(targetType);
        }
    }
}
