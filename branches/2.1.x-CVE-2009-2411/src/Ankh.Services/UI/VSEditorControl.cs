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
using Ankh.VS;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;

namespace Ankh.UI
{
    public class VSEditorControl : UserControl, IVSEditorControlInit
    {
        IAnkhServiceProvider _context;
        IAnkhEditorPane _pane;
        IVsWindowFrame _frame;

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

        [Browsable(false),DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
        }
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
