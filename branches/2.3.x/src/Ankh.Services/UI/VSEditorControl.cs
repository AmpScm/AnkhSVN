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
using System.ComponentModel;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;

using IOleConnectionPoint = Microsoft.VisualStudio.OLE.Interop.IConnectionPoint;
using IOleConnectionPointContainer = Microsoft.VisualStudio.OLE.Interop.IConnectionPointContainer;

using Ankh.VS;
using Ankh.Scc.UI;
using System.Drawing;


namespace Ankh.UI
{
    public class VSEditorControl : UserControl, IVSEditorControlInit, IVsWindowFrameNotify, IVsWindowFrameNotify2, IVsWindowFrameNotify3, IContextControl
    {
        IAnkhServiceProvider _context;
        IAnkhEditorPane _pane;
        IVsWindowFrame _frame;
        uint _frameNotifyCookie;

        public VSEditorControl()
        {
            /*ShowInTaskbar = false;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = false;
            FormBorderStyle = FormBorderStyle.None;*/
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set { _context = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public virtual object DocumentInstance
        {
            get { return null; }
        }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Localizable(true)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        private Guid GetGuid(__VSFPROPID __VSFPROPID)
        {
            if (_frame == null)
                throw new InvalidOperationException();

            Guid value;
            Marshal.ThrowExceptionForHR((_frame.GetGuidProperty((int)__VSFPROPID, out value)));

            return value;
        }

        private void SetGuid(__VSFPROPID propId, Guid value)
        {
            if (_frame == null)
                throw new InvalidOperationException();

            Marshal.ThrowExceptionForHR((_frame.SetGuidProperty((int)propId, ref value)));
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Guid KeyboardContext
        {
            get { return GetGuid(__VSFPROPID.VSFPROPID_InheritKeyBindings); }
            set { SetGuid(__VSFPROPID.VSFPROPID_InheritKeyBindings, value); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Guid CommandContext
        {
            get { return GetGuid(__VSFPROPID.VSFPROPID_CmdUIGuid); }
            set { SetGuid(__VSFPROPID.VSFPROPID_CmdUIGuid, value); }
        }
        protected void SetFindTarget(object findTarget)
        {
            if (findTarget == null)
                throw new ArgumentNullException("findTarget");

            _pane.SetFindTarget(findTarget);
        }

        IAnkhDynamicEditorFactory _dynamicFactory;
        [CLSCompliant(false)]
        protected IAnkhDynamicEditorFactory DynamicFactory
        {
            get { return _dynamicFactory ?? (_dynamicFactory = Context.GetService<IAnkhDynamicEditorFactory>()); }
        }

        public void Create(IAnkhServiceProvider context, string fullPath)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;

            DynamicFactory.CreateEditor(fullPath, this);
            OnFrameCreated(EventArgs.Empty);
        }

        protected virtual void OnFrameCreated(EventArgs e)
        {
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // VSDocumentForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Name = "VSDocumentForm";
            this.ResumeLayout(false);
        }

        List<IOleCommandTarget> _targets;

        [CLSCompliant(false)]
        public void AddCommandTarget(IOleCommandTarget commandTarget)
        {
            if (commandTarget == null)
                throw new ArgumentNullException("commandTarget");
            else if (_pane == null)
            {
                if (_targets == null)
                    _targets = new List<IOleCommandTarget>();

                _targets.Add(commandTarget);
            }
            else
                _pane.AddCommandTarget(commandTarget);
        }


        void IVSEditorControlInit.InitializedForm(Microsoft.VisualStudio.Shell.Interop.IVsUIHierarchy hier, uint id, IVsWindowFrame frame, IAnkhEditorPane pane)
        {
            _frame = frame;
            _pane = pane;

            if (_targets != null && pane != null)
                foreach (IOleCommandTarget t in _targets)
                    _pane.AddCommandTarget(t);

            IVsWindowFrame2 frame2 = _frame as IVsWindowFrame2;

            if (frame2 != null)
            {
                if (frame2.Advise(this, out _frameNotifyCookie) != 0)
                    _frameNotifyCookie = 0;
            }

            OnFrameLoaded(EventArgs.Empty);
        }

        protected virtual void OnFrameLoaded(EventArgs e)
        {
        }

        protected virtual void OnFrameSize(FrameEventArgs e)
        {
        }

        protected virtual void OnFrameDockableChanged(FrameEventArgs e)
        {
        }

        protected virtual void OnFrameClose(EventArgs e)
        {
        }

        protected virtual void OnFrameMove(FrameEventArgs e)
        {
        }

        protected virtual void OnFrameShow(FrameEventArgs e)
        {
        }

        #region IVsWindowFrameNotify* Members

        int IVsWindowFrameNotify2.OnClose(ref uint pgrfSaveOptions)
        {
            if (_frameNotifyCookie != 0)
                try
                {
                    IVsWindowFrame2 frame2 = _frame as IVsWindowFrame2;

                    if (frame2 != null)
                        frame2.Unadvise(_frameNotifyCookie);
                }
                finally
                {
                    _frameNotifyCookie = 0;
                }

            OnFrameClose(EventArgs.Empty);

            return 0;
        }

        int IVsWindowFrameNotify3.OnDockableChange(int fDockable, int x, int y, int w, int h)
        {
            OnFrameDockableChanged(new FrameEventArgs(fDockable != 0, new Rectangle(x, y, w, h), (__FRAMESHOW)0));

            return 0;
        }

        int IVsWindowFrameNotify3.OnMove(int x, int y, int w, int h)
        {
            OnFrameMove(new FrameEventArgs(false, new Rectangle(x, y, w, h), (__FRAMESHOW)0));

            return 0;
        }

        int IVsWindowFrameNotify.OnShow(int fShow)
        {
            OnFrameShow(new FrameEventArgs(false, Rectangle.Empty, (__FRAMESHOW)fShow));

            return 0;
        }

        int IVsWindowFrameNotify3.OnShow(int fShow)
        {
            OnFrameShow(new FrameEventArgs(false, Rectangle.Empty, (__FRAMESHOW)fShow));

            return 0;
        }

        int IVsWindowFrameNotify3.OnSize(int x, int y, int w, int h)
        {
            OnFrameSize(new FrameEventArgs(false, new Rectangle(x, y, w, h), (__FRAMESHOW)0));

            return 0;
        }

        #endregion

        #region IVsWindowFrameNotify Members

        int IVsWindowFrameNotify.OnDockableChange(int fDockable)
        {
            OnFrameDockableChanged(new FrameEventArgs(fDockable != 0, new Rectangle(0, 0, 0, 0), (__FRAMESHOW)0));
            return 0;
        }

        int IVsWindowFrameNotify.OnMove()
        {
            OnFrameMove(new FrameEventArgs(false, new Rectangle(0, 0, 0, 0), (__FRAMESHOW)0));
            return 0;
        }

        int IVsWindowFrameNotify.OnSize()
        {
            OnFrameSize(new FrameEventArgs(false, new Rectangle(0, 0, 0, 0), (__FRAMESHOW)0));
            return 0;
        }

        #endregion

        #region IVsWindowFrameNotify3 Members

        int IVsWindowFrameNotify3.OnClose(ref uint pgrfSaveOptions)
        {
            OnFrameClose(EventArgs.Empty);
            return 0;
        }

        #endregion
    }

    [CLSCompliant(false)]
    public interface IAnkhEditorPane
    {
        void AddCommandTarget(IOleCommandTarget target);
        void SetFindTarget(object findTarget);
    }

    [CLSCompliant(false)]
    public interface IVSEditorControlInit
    {
        void InitializedForm(IVsUIHierarchy hier, uint id, IVsWindowFrame frame, IAnkhEditorPane pane);
    }

}
