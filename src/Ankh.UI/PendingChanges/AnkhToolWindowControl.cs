using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Ankh.UI.Services;
using System.ComponentModel;
using Ankh.Scc.UI;

namespace Ankh.UI
{
    public class AnkhToolWindowControl : Form, IAnkhToolWindowControl, IAnkhCommandHookAccessor
    {
        IAnkhToolWindowHost _host;
        protected AnkhToolWindowControl()
        {
            base.FormBorderStyle = FormBorderStyle.None;
        }

        [DefaultValue(FormBorderStyle.None), Localizable(false)]
        public new FormBorderStyle FormBorderStyle
        {
            get { return base.FormBorderStyle; }
            set { base.FormBorderStyle = value; }
        }

        public override string Text
        {
            get 
            { 
                if(_host != null)
                    return _host.Title;
                else
                    return base.Text;
            }
            set
            {
                if (_host != null)
                    _host.Title = value;

                base.Text = value;
            }
        }

        /// <summary>
        /// Gets the UI site.
        /// </summary>
        /// <value>The UI site.</value>
        [CLSCompliant(false)]
        public IAnkhToolWindowHost ToolWindowHost
        {
            get { return _host; }
			set { _host = value; }
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public IAnkhServiceProvider Context
        {
            get { return _host; }
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

        /// <summary>
        /// Occurs when the frame show state changed
        /// </summary>
        public event EventHandler<FrameEventArgs> FrameShow;

        protected virtual void OnFrameShow(FrameEventArgs e)
        {
            if (FrameShow != null)
                FrameShow(this, e);
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
