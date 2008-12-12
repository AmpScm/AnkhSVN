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
using System.IO;
using SharpSvn;
using Ankh.UI.PendingChanges;
using Ankh.UI.VSSelectionControls;
using Ankh.Scc;
using Ankh.Scc.UI;
using System.Collections.ObjectModel;
using Ankh.Ids;
using Microsoft.VisualStudio;

namespace Ankh.UI.Annotate
{
    public partial class AnnotateEditorControl : VSEditorControl, ISelectionMapOwner<IAnnotateSection>
    {
        List<AnnotateSection> blameSections = new List<AnnotateSection>();
        SelectionItemMap _map;
        SvnOrigin _origin;

        public AnnotateEditorControl()
        {
            InitializeComponent();
            logMessageEditor1.ReadOnly = true;
        }

        protected override void OnFrameCreated(EventArgs e)
        {
            base.OnFrameCreated(e);

            _map = SelectionItemMap.Create<IAnnotateSection>(this);
            _map.Context = Context;

            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);
            // Set Notify that we have a selection, otherwise the first selection request fails.
            _map.NotifySelectionUpdated();

            CommandContext = AnkhId.AnnotateContextGuid;
            KeyboardContext = new Guid(0x8B382828, 0x6202, 0x11d1, 0x88, 0x70, 0x00, 0x00, 0xF8, 0x75, 0x79, 0xD2); // Editor

            blameMarginControl1.Init(Context, this, blameSections);
        }

        public void LoadFile(string projectFile, string exportedFile)
        {
            this.Text = Path.GetFileName(projectFile) + " (Annotated)";
            logMessageEditor1.OpenFile(projectFile);
            logMessageEditor1.ReplaceContents(exportedFile);

        }

        internal int GetLineHeight()
        {
            return logMessageEditor1.LineHeight;
        }


        public void AddLines(SvnOrigin origin, Collection<SharpSvn.SvnBlameEventArgs> blameResult)
        {
            _origin = origin;

            AnnotateSection section = null;
            blameSections.Clear();

            foreach (SvnBlameEventArgs e in blameResult)
            {
                if (blameSections.Count == 0)
                {
                    section = new AnnotateSection(e, origin);
                    blameSections.Add(section);
                }
                else
                {
                    if(section.Revision == e.Revision)
                        section.EndLine = (int)e.LineNumber;
                    else
                    {
                        section = new AnnotateSection(e, origin);
                        blameSections.Add(section);
                    }
                }
            }
            blameMarginControl1.Invalidate();
        }

        private void logMessageEditor1_Scroll(object sender, TextViewScrollEventArgs e)
        {
            if (e.Orientation == ScrollOrientation.HorizontalScroll)
                return; // No need to update margin

            blameMarginControl1.NotifyScroll(e);
        }

        AnnotateSection _selected;
        internal AnnotateSection Selected
        {
            get { return _selected; }
        }

        internal void SetSelection(IAnnotateSection section)
        {
            // Check if necessary
            //Focus();
            //Select();

            _selected = (AnnotateSection)section;

            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);

            _map.NotifySelectionUpdated();
        }

        #region ISelectionMapOwner<IBlameSection> Members

        public event EventHandler SelectionChanged;

        public System.Collections.IList Selection
        {
            get
            {
                if (_selected == null)
                    return new IAnnotateSection[] { };

                return new IAnnotateSection[] { _selected };
            }
        }

        public System.Collections.IList AllItems
        {
            get { return blameSections; }
        }

        public IntPtr GetImageList()
        {
            return IntPtr.Zero;
        }

        public int GetImageListIndex(IAnnotateSection item)
        {
            return 0;
        }

        public string GetText(IAnnotateSection item)
        {
            return item.Revision.ToString();
        }

        public object GetSelectionObject(IAnnotateSection item)
        {
            return item;
        }

        public IAnnotateSection GetItemFromSelectionObject(object item)
        {
            return (IAnnotateSection)item;
        }

        public void SetSelection(IAnnotateSection[] items)
        {
            if (items.Length > 0)
                SetSelection(items[0]);
            else
                SetSelection((IAnnotateSection)null);
        }

        public string GetCanonicalName(IAnnotateSection item)
        {
            AnnotateSection section = (AnnotateSection)item;
            return section.Author + section.StartLine + section.Revision;
        }

        #endregion    
    }
}
