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
using Ankh.VS;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.UI.PendingChanges
{
    partial class PendingConflictsPage : PendingChangesPage
    {
        public PendingConflictsPage()
        {
            InitializeComponent();
        }

        protected override Type PageType
        {
            get
            {
                return typeof(PendingConflictsPage);
            }
        }

        bool _loaded;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            conflictView.Context = Context;

            if (!SystemInformation.HighContrast)
            {
                IAnkhVSColor clr = Context.GetService<IAnkhVSColor>();
                Color c;
                if (clr != null && clr.TryGetColor(__VSSYSCOLOREX.VSCOLOR_TITLEBAR_INACTIVE, out c))
                {
                    resolvePannel.BackColor = c;
                }

                if (clr != null && clr.TryGetColor(__VSSYSCOLOREX.VSCOLOR_TITLEBAR_INACTIVE_TEXT, out c))
                {
                    resolvePannel.ForeColor = c;
                }
            }

            conflictView.ColumnWidthChanged += new ColumnWidthChangedEventHandler(conflictView_ColumnWidthChanged);
            IDictionary<string, int> widths = ConfigurationService.GetColumnWidths(GetType());
            conflictView.SetColumnWidths(widths);

            ResizeToFit();
            _loaded = true;
        }

        protected void conflictView_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            IDictionary<string, int> widths = conflictView.GetColumnWidths();
            ConfigurationService.SaveColumnsWidths(GetType(), widths);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (_loaded)
                ResizeToFit();
        }

        private void ResizeToFit()
        {
            conflictEditSplitter.SplitterDistance += conflictEditSplitter.Panel2.Height - resolveLinkLabel.Bottom - resolveLinkLabel.Margin.Bottom;
        }
    }
}
