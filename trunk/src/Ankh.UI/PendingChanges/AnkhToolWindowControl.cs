using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Ankh.UI.Services;
using System.ComponentModel;
using Ankh.Scc.UI;

namespace Ankh.UI
{
    public class AnkhToolWindowControl : UserControl, IAnkhToolWindowControl, IAnkhCommandHookAccessor
    {
        IAnkhServiceProvider _context;
        IAnkhToolWindowSite _site;
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

                if (value == null || value is IAnkhToolWindowSite)
                {
                    _site = (IAnkhToolWindowSite)value;
                }
            }
        }

        /// <summary>
        /// Gets the UI site.
        /// </summary>
        /// <value>The UI site.</value>
        [CLSCompliant(false)]
        public IAnkhToolWindowSite ToolWindowSite
        {
            get { return _site; }
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public IAnkhServiceProvider Context
        {
            get { return _context; }
        }

        /// <summary>
        /// Sets the context.
        /// </summary>
        /// <value>The context.</value>
        IAnkhServiceProvider IAnkhToolWindowControl.Context
        {
            set { _context = value; }
        }

        #region IAnkhToolWindowControl Members

        /// <summary>
        /// Called when the frame is created
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnFrameCreated(EventArgs e)
        {
        }

        void IAnkhToolWindowControl.OnFrameCreated(EventArgs e)
        {
            OnFrameCreated(e);
        }

        /// <summary>
        /// Called when the frame is closed
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnFrameClose(EventArgs e)
        {
        }

        void IAnkhToolWindowControl.OnFrameClose(EventArgs e)
        {
            OnFrameClose(e);
        }

        /// <summary>
        /// Called when the dockstate is changing
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnFrameDockableChanged(FrameEventArgs e)
        {

        }

        void IAnkhToolWindowControl.OnFrameDockableChanged(FrameEventArgs e)
        {
            OnFrameDockableChanged(e);
        }

        void IAnkhToolWindowControl.OnFrameMove(FrameEventArgs e)
        {
        }

        protected virtual void OnFrameShow(FrameEventArgs e)
        {
        }

        void IAnkhToolWindowControl.OnFrameShow(FrameEventArgs e)
        {
            OnFrameShow(e);
        }

        protected virtual void OnFrameSize(FrameEventArgs e)
        {
        }


        void IAnkhToolWindowControl.OnFrameSize(FrameEventArgs e)
        {
            OnFrameSize(e);
        }

        #endregion

        #region IAnkhCommandHookAccessor Members

        AnkhCommandHook _hook;
        AnkhCommandHook IAnkhCommandHookAccessor.CommandHook
        {
            get { return _hook; }
            set { _hook = value; }
        }

        #endregion
    }
}
