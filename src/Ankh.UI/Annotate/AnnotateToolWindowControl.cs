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

namespace Ankh.UI.Blame
{
    public partial class BlameToolWindowControl : AnkhToolWindowControl, ISelectionMapOwner<IBlameSection>, IBlameControl
    {
        List<BlameSection> blameSections = new List<BlameSection>();
        SelectionItemMap _map;
        SvnItem _wcItem;

        public BlameToolWindowControl()
        {
            InitializeComponent();
            logMessageEditor1.ReadOnly = true;
        }

        public void Init()
        {
            this.blameMarginControl1.Init(ToolWindowHost, this, blameSections);
        }

        protected override void OnFrameCreated(EventArgs e)
        {
            base.OnFrameCreated(e);

            _map = SelectionItemMap.Create<IBlameSection>(this);
            _map.Context = ToolWindowHost;

            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);
            // Set Notify that we have a selection, otherwise the first selection request fails.
            _map.NotifySelectionUpdated();
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


        public void AddLines(SvnItem workingCopyItem, System.Collections.ObjectModel.Collection<SharpSvn.SvnBlameEventArgs> blameResult)
        {
            _wcItem = workingCopyItem;

            BlameSection section = null;
            blameSections.Clear();

            foreach (SvnBlameEventArgs e in blameResult)
            {
                if (blameSections.Count == 0)
                {
                    section = new BlameSection(e);
                    blameSections.Add(section);
                }
                else
                {
                    if(section.Revision == e.Revision)
                        section.EndLine = (int)e.LineNumber;
                    else
                    {
                        section = new BlameSection(e);
                        blameSections.Add(section);
                    }
                }

                
            }
            blameMarginControl1.Invalidate();
        }

        private void logMessageEditor1_Scroll(object sender, TextViewScrollEventArgs e)
        {
            blameMarginControl1.NotifyScroll(e.MinUnit, e.MaxUnit, e.VisibleUnits, e.FirstVisibleUnit);
        }

        BlameSection _selected;
        internal BlameSection Selected
        {
            get { return _selected; }
        }

        internal void SetSelection(IBlameSection section)
        {
            // Check if necessary
            //Focus();
            //Select();

            _selected = (BlameSection)section;

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
                    return new IBlameSection[] { };

                return new IBlameSection[] { _selected };
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

        public int GetImageListIndex(IBlameSection item)
        {
            return 0;
        }

        public string GetText(IBlameSection item)
        {
            return item.Revision.ToString();
        }

        public object GetSelectionObject(IBlameSection item)
        {
            return item;
        }

        public IBlameSection GetItemFromSelectionObject(object item)
        {
            return (IBlameSection)item;
        }

        public void SetSelection(IBlameSection[] items)
        {
            if (items.Length > 0)
                SetSelection(items[0]);
            else
                SetSelection((IBlameSection)null);
        }

        public string GetCanonicalName(IBlameSection item)
        {
            BlameSection section = (BlameSection)item;
            return section.Author + section.StartLine + section.Revision;
        }

        #endregion

        #region IBlameControl Members

        public bool HasWorkingCopyItems
        {
            get { return true; }
        }

        public SvnItem[] WorkingCopyItems
        {
            get { return new SvnItem[] { _wcItem }; }
        }

        #endregion
    }
}
