using Ankh.Configuration;
using Microsoft.Win32;
using System;
using System.Reflection;

namespace Ankh.WpfPackage.Services
{

    partial class ThemingService
    {
        public bool GetCurrentTheme(out Guid themeGuid)
        {
            if (VSVersion.VS2013OrLater)
            {
                return GetThemeViaApi(out themeGuid);
            }
            else
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

                try
                {
                    if (!string.IsNullOrEmpty(currentTheme))
                    {
                        themeGuid = new Guid(currentTheme);
                        return true;
                    }
                }
                catch { }
                {
                    currentTheme = null;
                }

                return true;
            }
        }

        object _themeService;
        PropertyInfo _currentThemeProperty;
        PropertyInfo _themeIdProperty;
        private bool GetThemeViaApi(out Guid themeGuid)
        {
            if (_themeService == null)
            {
                _themeService = GetService<IAnkhQueryService>().QueryService<object>(new Guid("0D915B59-2ED7-472A-9DE8-9161737EA1C5"));
            }

            if (_themeService != null && _currentThemeProperty == null)
            {
                _currentThemeProperty = _themeService.GetType().GetProperty("CurrentTheme");
            }

            if (_currentThemeProperty != null)
            {
                object currentTheme = _currentThemeProperty.GetValue(_themeService, null);

                if (currentTheme != null)
                {
                    if (_themeIdProperty == null)
                        _themeIdProperty = currentTheme.GetType().GetProperty("ThemeId");

                    if (_themeIdProperty != null)
                    {
                        themeGuid = (Guid)_themeIdProperty.GetValue(currentTheme, null);
                        return true;
                    }
                }
            }

            themeGuid = Guid.Empty;
            return false;
        }
    }
}
