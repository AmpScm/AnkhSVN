// $Id$
//
// Copyright 2008 The AnkhSVN Project
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ankh.UI.Services;
using System.ComponentModel.Design;

namespace Ankh.UI.PendingChanges
{
    partial class PendingChangesPage : UserControl
    {
        bool _registered;
        public PendingChangesPage()
        {
            InitializeComponent();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(true)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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

        [Browsable(false)]
        protected virtual Type PageType
        {
            get { return null; }
        }

        IAnkhServiceProvider _context;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set { _context = value; }
        }

        PendingChangesToolControl _toolControl;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PendingChangesToolControl ToolControl
        {
            get { return _toolControl; }
            set { _toolControl = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Register(true);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            Register(true);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            Register(false);
        }

        IServiceContainer _container;
        private void Register(bool register)
        {
            if (_registered == register)
                return;

            if (register)
            {
                if (_container == null && Context == null)
                    return;

                _container = Context.GetService<IServiceContainer>();

                if (_container == null)
                    return;

                if (null == _container.GetService(PageType))
                {
                    _registered = true;
                    _container.AddService(PageType, this);
                }
            }
            else if (_container != null)
            {
                _registered = false;
                _container.RemoveService(PageType);
                _container = null;
            }
        }

        public virtual bool CanRefreshList
        {
            get { return false; }
        }

        public virtual void RefreshList()
        {
            throw new NotImplementedException();
        }
    }
}
