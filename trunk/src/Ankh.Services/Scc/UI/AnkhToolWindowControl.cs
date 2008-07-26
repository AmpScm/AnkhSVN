using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Ankh.UI.Services;
using System.ComponentModel;

namespace Ankh.Scc.UI
{
    public class AnkhToolWindowControl : UserControl
    {
        IAnkhServiceProvider _context;
        IAnkhUISite _site;
        protected AnkhToolWindowControl()
        {
        }

        /// <summary>
        /// Gets or sets the site of the control.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.ComponentModel.ISite"/> associated with the <see cref="T:System.Windows.Forms.Control"/>, if any.</returns>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// 	<IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public override ISite Site
        {
            get { return base.Site; }
            set
            {
                base.Site = value;

                if (value == null || value is IAnkhUISite)
                {
                    _site = (IAnkhUISite)value;

                    if (_site != null)
                        _context = _site;

                    OnUISiteChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:UISiteChanged"/> event.
        /// </summary>
        /// <param name="eventArgs">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnUISiteChanged(EventArgs eventArgs)
        {

        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public IAnkhServiceProvider Context
        {
            get { return _context; }
        }
    }
}
