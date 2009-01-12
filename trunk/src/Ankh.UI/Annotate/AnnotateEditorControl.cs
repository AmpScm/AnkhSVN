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
        List<AnnotateRegion> blameSections = new List<AnnotateRegion>();
        SelectionItemMap _map;
        SvnOrigin _origin;

        public AnnotateEditorControl()
        {
            InitializeComponent();
            editor.ReadOnly = true;
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
            SetFindTarget(editor.FindTarget);

            blameMarginControl1.Init(Context, this, blameSections);
        }

        public void LoadFile(string projectFile, string exportedFile)
        {
            this.Text = Path.GetFileName(projectFile) + " (Annotated)";
            editor.OpenFile(projectFile);
            editor.ReplaceContents(exportedFile);

        }

        internal int GetLineHeight()
        {
            return editor.LineHeight;
        }


        public void AddLines(SvnOrigin origin, Collection<SharpSvn.SvnBlameEventArgs> blameResult)
        {
            _origin = origin;

            SortedList<long, AnnotateSource> _sources = new SortedList<long, AnnotateSource>();

            AnnotateRegion section = null;
            blameSections.Clear();

            foreach (SvnBlameEventArgs e in blameResult)
            {
                AnnotateSource src;
                if (!_sources.TryGetValue(e.Revision, out src))
                    _sources.Add(e.Revision, src = new AnnotateSource(e, origin));

                int line = (int)e.LineNumber;

                if(section == null || section.Source != src)
                {
                    section = new AnnotateRegion(line, src);
                    blameSections.Add(section);
                }
                else
                {
                    section.EndLine = line;
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

        AnnotateSource _selected;
        internal AnnotateSource Selected
        {
            get { return _selected; }
        }

        internal void SetSelection(IAnnotateSection section)
        {
            // Check if necessary
            //Focus();
            //Select();

            _selected = (AnnotateSource)section;

            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);

            _map.NotifySelectionUpdated();
        }

        #region ISelectionMapOwner<IBlameSection> Members

        public event EventHandler SelectionChanged;

        System.Collections.IList ISelectionMapOwner<IAnnotateSection>.Selection
        {
            get
            {
                if (_selected == null)
                    return new AnnotateSource[0];

                return new AnnotateSource[] { _selected };
            }
        }

        System.Collections.IList ISelectionMapOwner<IAnnotateSection>.AllItems
        {
            get { return ((ISelectionMapOwner<IAnnotateSection>)this).Selection; }
        }

        IntPtr ISelectionMapOwner<IAnnotateSection>.GetImageList()
        {
            return IntPtr.Zero;
        }

        int ISelectionMapOwner<IAnnotateSection>.GetImageListIndex(IAnnotateSection item)
        {
            return 0;
        }

        string ISelectionMapOwner<IAnnotateSection>.GetText(IAnnotateSection item)
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

        void ISelectionMapOwner<IAnnotateSection>.SetSelection(IAnnotateSection[] items)
        {
            if (items.Length > 0)
                SetSelection(items[0]);
            else
                SetSelection((IAnnotateSection)null);
        }

        /// <summary>
        /// Gets the canonical (path / uri) of the item. Used by packages to determine a selected file
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>A canonical name or null</returns>
        string ISelectionMapOwner<IAnnotateSection>.GetCanonicalName(IAnnotateSection item)
        {
            return null;
        }

        #endregion
    }
}
