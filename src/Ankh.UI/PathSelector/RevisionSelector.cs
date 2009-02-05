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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ankh.Scc;
using System.Globalization;

namespace Ankh.UI.PathSelector
{
    partial class RevisionSelector : UserControl
    {
        public RevisionSelector()
        {
            InitializeComponent();
        }

        IAnkhServiceProvider _context;
        SvnOrigin _origin;

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set { _context = value; EnableBrowse(); }
        }

        /// <summary>
        /// Gets or sets the SVN origin.
        /// </summary>
        /// <value>The SVN origin.</value>
        public SvnOrigin SvnOrigin
        {
            get { return _origin; }
            set { _origin = value; EnableBrowse(); }
        }

        void EnableBrowse()
        {
            browseButton.Enabled = (SvnOrigin != null) && (Context != null);
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            using (LogViewerDialog lvd = new LogViewerDialog(SvnOrigin))
            {
                if (DialogResult.OK != lvd.ShowDialog(Context))
                    return;

                ISvnLogItem li = EnumTools.GetSingle(lvd.SelectedItems);

                if (li == null)
                    return;

                Revision = li.Revision;
            }
        }

        public event EventHandler Changed;

        public long? Revision
        {
            get
            {
                long rev;
                string text = revisionBox.Text;
                if (string.IsNullOrEmpty(text))
                    return null;

                if (long.TryParse(text.Trim(), out rev))
                    return rev;
                else
                    return null;
            }
            set
            {
                if (value.HasValue)
                    revisionBox.Text = value.Value.ToString(CultureInfo.InvariantCulture);
                else
                    revisionBox.Text = "";
            }
        }

        private void revisionBox_TextChanged(object sender, EventArgs e)
        {
            OnChanged(e);
        }

        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }
    }
}
