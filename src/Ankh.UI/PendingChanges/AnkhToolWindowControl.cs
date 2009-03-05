// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

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
        IAnkhToolWindowHost _host;
        protected AnkhToolWindowControl()
        {
            // ShowInTaskBar = false;
            //base.FormBorderStyle = FormBorderStyle.None;
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
			set 
            {
                if (_host != value)
                {
                    _host = value;
                    OnContextChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnContextChanged(EventArgs e)
        {
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public IAnkhServiceProvider Context
        {
            get { return _host; }
        }

        /// <summary>
        /// Returns an object that represents a service provided by the <see cref="T:System.ComponentModel.Component"/> or by its <see cref="T:System.ComponentModel.Container"/>.
        /// </summary>
        /// <param name="service">A service provided by the <see cref="T:System.ComponentModel.Component"/>.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents a service provided by the <see cref="T:System.ComponentModel.Component"/>, or null if the <see cref="T:System.ComponentModel.Component"/> does not provide the specified service.
        /// </returns>
        protected override object GetService(Type service)
        {
            object r;
            if (Context != null)
            {
                r = Context.GetService(service);

                if (r != null)
                    return r;
            }
    
            return base.GetService(service);
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
            switch (e.Show)
            {
                case Microsoft.VisualStudio.Shell.Interop.__FRAMESHOW.FRAMESHOW_WinClosed:                
                case Microsoft.VisualStudio.Shell.Interop.__FRAMESHOW.FRAMESHOW_DestroyMultInst:
                case Microsoft.VisualStudio.Shell.Interop.__FRAMESHOW.FRAMESHOW_TabDeactivated:
                    Visible = false;
                    break;
                // Why not Microsoft.VisualStudio.Shell.Interop.__FRAMESHOW.FRAMESHOW_WinHidden:
                //   This state is not reverted in non animated mode in VS2008 (and maybe other versions)
                //
                //   See http://ankhsvn.open.collab.net/ds/viewMessage.do?dsForumId=582&dsMessageId=303686
            }
            OnFrameShow(e);
            switch (e.Show)
            {
                case Microsoft.VisualStudio.Shell.Interop.__FRAMESHOW.FRAMESHOW_WinClosed:
                case Microsoft.VisualStudio.Shell.Interop.__FRAMESHOW.FRAMESHOW_WinHidden:
                case Microsoft.VisualStudio.Shell.Interop.__FRAMESHOW.FRAMESHOW_DestroyMultInst:
                case Microsoft.VisualStudio.Shell.Interop.__FRAMESHOW.FRAMESHOW_TabDeactivated:
                    break;
                default:
                    Visible = true;
                    break;
            }
        }

        /// <summary>
        /// Occurs when the frame show size changed
        /// </summary>
        public event EventHandler<FrameEventArgs> FrameSize;

        protected virtual void OnFrameSize(FrameEventArgs e)
        {
            if (FrameSize != null)
                FrameSize(this, e);
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
