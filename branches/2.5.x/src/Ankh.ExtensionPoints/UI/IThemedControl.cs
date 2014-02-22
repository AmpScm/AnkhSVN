using System;
using System.ComponentModel;

namespace Ankh.ExtensionPoints.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplyThemeEventArgs : EventArgs
    {
        readonly IServiceProvider _serviceProvider;
        bool _cancelRecurse;
        bool _dontTheme;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        public ApplyThemeEventArgs(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");

            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets a reference to a service provider that allows querying Visual Studio services
        /// </summary>
        public IServiceProvider ServiceProvider
        {
            get { return _serviceProvider; }
        }

        /// <summary>
        /// If set to true, the theming handling won't recurse into child controls
        /// </summary>
        [DefaultValue(false)]
        public bool NoRecurse
        {
            get { return _cancelRecurse; }
            set { _cancelRecurse = value; }
        }

        /// <summary>
        /// If set to true the control itself is not 'auto-themed'
        /// </summary>
        public bool DontTheme
        {
            get { return _dontTheme; }
            set { _dontTheme = value;  }
        }
    }

    /// <summary>
    /// Can be implemented to customize AnkhSVN's auto theming
    /// </summary>
    public interface IThemedControl
    {
        /// <summary>
        /// Called right before theming a control
        /// </summary>
        /// <param name="e"></param>
        void OnApplyTheme(ApplyThemeEventArgs e);
    }
}
