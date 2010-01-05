using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;

using Ankh.UI;

namespace Ankh.VS.LanguageServices.Core
{
    public class AnkhLanguageDropDownBar : AnkhService, IVsDropdownBarClient, IAnkhIdleProcessor
    {
        AnkhCodeWindowManager _manager;
        bool _added;
        readonly Dictionary<IVsTextView, ComboTextView> _comboViews = new Dictionary<IVsTextView, ComboTextView>();
        readonly List<ComboMemberCollection> _combos = new List<ComboMemberCollection>();
        IVsDropdownBar _bar;

        public AnkhLanguageDropDownBar(AnkhLanguage language, AnkhCodeWindowManager manager)
            : base(language)
        {
            if (manager == null)
                throw new ArgumentNullException("manager");

            _manager = manager;
        }

        protected AnkhCodeWindowManager Manager
        {
            get { return _manager; }
        }

        protected ComboMemberCollection DropDownTypes
        {
            get { return _combos.Count > 0 ? _combos[0] : null; }
        }

        protected ComboMemberCollection DropDownMembers
        {
            get { return _combos.Count > 1 ? _combos[1] : null; }
        }

        protected virtual int NumberOfCombos
        {
            get { return 2; }
        }

        protected internal virtual void Initialize()
        {
            IVsDropdownBarManager dbm = Manager.CodeWindow as IVsDropdownBarManager;

            if (dbm == null)
                return;

            int combos = NumberOfCombos;
            _combos.Clear();
            for (int i = 0; i < combos; i++)
                _combos.Add(new ComboMemberCollection(this, i));

            if (!ErrorHandler.Succeeded(dbm.AddDropdownBar(combos, this)))
                return;

            IVsDropdownBar bar;
            if (!ErrorHandler.Succeeded(dbm.GetDropdownBar(out bar)))
                return;

            _added = true;
            GetService<IAnkhPackage>().RegisterIdleProcessor(this);

            foreach(IVsTextView v in Manager.GetViews())
            {
                _comboViews.Add(v, new ComboTextView(this, v));

                if (_activeView == null)
                    _activeView = v;
            }

            ScheduleSynchronize();
        }

        protected ComboMemberCollection GetData(int iCombo)
        {
            return _combos[iCombo];
        }

        internal void Close()
        {
            if (Manager == null)
                return;

            try
            {
                if (_added)
                {
                    GetService<IAnkhPackage>().UnregisterIdleProcessor(this);
                    IVsDropdownBarManager dbm = Manager.CodeWindow as IVsDropdownBarManager;

                    dbm.RemoveDropdownBar();
                }

                OnClose();

                List<ComboTextView> m = new List<ComboTextView>(_comboViews.Values);
                _comboViews.Clear();
                foreach (ComboTextView ctv in m)
                    ctv.Dispose();
            }
            finally
            {
                _added = false;
                _manager = null;
                _activeView = null;
                _bar = null;
            }
        }

        protected virtual void OnClose()
        {
        }

        IVsTextView _activeView;
        bool _shouldSynchronize, _shouldUpdate;
        internal void OnChangeCaretLine(IVsTextView view, int iNewLine)
        {
            _shouldSynchronize = true;
        }
        internal void OnSetFocus(IVsTextView view)
        {
            _activeView = view;
            _shouldSynchronize = true;
        }

        [CLSCompliant(false)]
        protected virtual void SynchronizeCombos(IVsTextView view, int line, int col)
        {
            foreach (ComboMemberCollection c in _combos)
            {
                foreach (ComboMember cm in c)
                {
                    if (cm.ContainsPoint(line, col))
                    {
                        c.Current = c.IndexOf(cm);
                        break;
                    }
                }
            }
        }

        public void ScheduleSynchronize()
        {
            _shouldSynchronize = true;
        }

        bool _shouldSendSync;
        void IAnkhIdleProcessor.OnIdle(AnkhIdleArgs e)
        {
            OnIdle(e);
            if (_shouldSynchronize || _shouldUpdate)
            {
                _shouldSynchronize = false;
                _shouldUpdate = false;

                if (_activeView != null)
                {
                    int line, col;
                    if (ErrorHandler.Succeeded(_activeView.GetCaretPos(out line, out col)))
                    {
                        SynchronizeCombos(_activeView, line, col);
                    }
                }

                _shouldSendSync = true;
            }

            if (_shouldSendSync && _bar != null)
            {
                _shouldSendSync = false;

                foreach (ComboMemberCollection cmc in _combos)
                {
                    if (cmc.IsDirty(true))
                        _bar.RefreshCombo(cmc.Index, cmc.Current);
                }
            }
        }

        protected virtual void OnIdle(AnkhIdleArgs e)
        {
        }

        #region IVsDropdownBarClient Members

        [CLSCompliant(false)]
        public int GetComboAttributes(int iCombo, out uint pcEntries, out uint puEntryType, out IntPtr phImageList)
        {
            int numberOfEntries;
            DROPDOWNENTRYTYPE entryType;
            ImageList imageList;
            GetSettings(iCombo, out numberOfEntries, out entryType, out imageList);
            pcEntries = (uint)numberOfEntries;
            puEntryType = (uint)entryType;

            if (imageList == null)
            {
                puEntryType &= ~(uint)DROPDOWNENTRYTYPE.ENTRY_IMAGE;
                phImageList = IntPtr.Zero;
            }
            else
                phImageList = imageList.Handle;

            return VSConstants.S_OK;
        }

        [CLSCompliant(false)]
        protected virtual void GetSettings(int iCombo,out int numberOfEntries, out DROPDOWNENTRYTYPE entryType,out ImageList imageList)
        {
            numberOfEntries = _combos[iCombo].Count;
            entryType = DROPDOWNENTRYTYPE.ENTRY_TEXT | DROPDOWNENTRYTYPE.ENTRY_IMAGE;
            imageList = null;
        }

        public int GetComboTipText(int iCombo, out string pbstrText)
        {
            ComboMemberCollection list = _combos[iCombo];

            if (list._current >= 0 && list._current < list.Count)
                pbstrText = list[list._current].TipText;
            else
                pbstrText = null;

            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.GetEntryAttributes(int iCombo, int iIndex, out uint pAttr)
        {
            if (iIndex < 0)
            {
                pAttr = 0;
                return VSConstants.S_OK;
            }
            pAttr = (uint)_combos[iCombo][iIndex].Attributes;
            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.GetEntryImage(int iCombo, int iIndex, out int piImageIndex)
        {
            piImageIndex = GetEntryImage(iCombo, iIndex);
            return VSConstants.S_OK;
        }

        protected virtual int GetEntryImage(int iCombo, int iIndex)
        {
            return _combos[iCombo][iIndex].Image;
        }

        int IVsDropdownBarClient.GetEntryText(int iCombo, int iIndex, out string ppszText)
        {
            if (iIndex < 0)
            {
                ppszText = "";
                return VSConstants.S_OK;
            }
            ppszText = GetEntryText(iCombo, iIndex);
            return VSConstants.S_OK;
        }

        protected virtual string GetEntryText(int iCombo, int iIndex)
        {
            return _combos[iCombo][iIndex].Text;
        }

        int IVsDropdownBarClient.OnComboGetFocus(int iCombo)
        {
            return VSConstants.S_OK;
        }

        int IVsDropdownBarClient.OnItemChosen(int iCombo, int iIndex)
        {
            _combos[iCombo]._current = iIndex;

            if (iIndex >= 0 && _activeView != null)
                OnItemChosen(iCombo, iIndex);
            return VSConstants.S_OK;
        }

        protected virtual void OnItemChosen(int iCombo, int iIndex)
        {
            GetData(iCombo)[iIndex].OnSelect(_activeView);
        }

        public int OnItemSelected(int iCombo, int iIndex)
        {
            return VSConstants.S_OK;
        }

        [CLSCompliant(false)]
        public int SetDropdownBar(IVsDropdownBar pDropdownBar)
        {
            _bar = pDropdownBar;
            return VSConstants.S_OK;
        }

        #endregion


        internal void OnNewView(IVsTextView view)
        {
            if (_comboViews.ContainsKey(view))
                return;

            _comboViews.Add(view, new ComboTextView(this, view));
        }

        internal void OnCloseView(IVsTextView view)
        {
            ComboTextView v;
            if (_comboViews.TryGetValue(view, out v))
            {
                _comboViews.Remove(view);
                v.Dispose();
            }
        }

        internal void OnDataUpdated(ComboMemberCollection comboMemberCollection,AnkhLanguageDropDownBar _bar,int _index)
        {
            _shouldUpdate = true;
        }

        class ComboTextView : AnkhService, IVsTextViewEvents, IDisposable
        {
            IVsTextView _view;
            uint _cookie;

            public ComboTextView(AnkhLanguageDropDownBar bar, IVsTextView view)
                : base(bar)
            {
                _view = view;

                if (!TryHookConnectionPoint<IVsTextViewEvents>(view, this, out _cookie))
                    _cookie = 0;
            }

            protected AnkhLanguageDropDownBar Bar
            {
                get { return (AnkhLanguageDropDownBar)Context; }
            }

            public void Dispose()
            {
                if (_view == null)
                    return;

                if (_cookie != 0)
                    ReleaseHook<IVsTextViewEvents>(_view, _cookie);

                _view = null;
            }

            public void OnChangeCaretLine(IVsTextView view, int iNewLine, int iOldLine)
            {
                Bar.OnChangeCaretLine(view, iNewLine);
            }

            public void OnChangeScrollInfo(IVsTextView pView, int iBar, int iMinUnit, int iMaxUnits, int iVisibleUnits, int iFirstVisibleUnit)
            {
            }

            public void OnKillFocus(IVsTextView pView)
            {
            }

            public void OnSetBuffer(IVsTextView pView, IVsTextLines pBuffer)
            {
            }

            public void OnSetFocus(IVsTextView pView)
            {
                Bar.OnSetFocus(pView);
            }
        }

        public class ComboMember
        {
            string _text;
            int _image;
            TextSpan _span;
            DROPDOWNFONTATTR _attr;

            [CLSCompliant(false)]
            public ComboMember(string text, int image, DROPDOWNFONTATTR attr)
            {
                if (string.IsNullOrEmpty(text))
                    throw new ArgumentNullException("text");

                _text = text;
                _image = image > 0 ? image : 0;
                _attr = attr;
            }

            public ComboMember(string text, int image)
                : this(text, image, DROPDOWNFONTATTR.FONTATTR_PLAIN)
            {
            }

            [CLSCompliant(false)]
            public ComboMember(string text, int image, DROPDOWNFONTATTR attr, TextSpan span)
                : this(text, image, attr)
            {
                _span = span;
            }

            public virtual string Text
            {
                get { return _text; }
            }

            public virtual int Image
            {
                get { return _image; }
            }

            public virtual string TipText
            {
                get { return null; }
            }

            [CLSCompliant(false)]
            public DROPDOWNFONTATTR Attributes
            {
                get { return _attr; }
            }

            internal bool ContainsPoint(int line, int col)
            {
                if ((line > _span.iStartLine || line == _span.iStartLine && col >= _span.iStartIndex) &&
                    (line < _span.iEndLine || line == _span.iEndLine && col < _span.iEndIndex))
                    return true;

                return false;
            }

            [CLSCompliant(false)]
            public virtual void OnSelect(IVsTextView view)
            {
                /* Just ignore errors */
                view.SetCaretPos(_span.iStartLine, _span.iStartIndex);
                view.EnsureSpanVisible(_span);
                view.SendExplicitFocus();
            }
        }

        public class ComboMemberCollection : Collection<ComboMember>
        {
            AnkhLanguageDropDownBar _bar;
            readonly int _index;
            internal int _current;
            bool _dirty;

            public ComboMemberCollection(AnkhLanguageDropDownBar bar, int index)
            {
                _bar = bar;
                _index = index;
                _current = -1;
            }

            public int Index
            {
                get { return _index; }
            }

            public int Current
            {
                get { return _current; }
                set
                {
                    if (value < 0 || value >= Count)
                        value = 0;
                    if (value == _current)
                        return;

                    _current = value;
                    _bar._shouldSendSync = true;
                    _dirty = true;
                }
            }

            internal bool IsDirty(bool clear)
            {
                if (!_dirty)
                    return false;

                if (clear)
                    _dirty = false;

                return true;
            }

            protected override void ClearItems()
            {
                base.ClearItems();
                _bar.OnDataUpdated(this, _bar, _index);
            }

            protected override void InsertItem(int index, ComboMember item)
            {
                base.InsertItem(index, item);
                _bar.OnDataUpdated(this, _bar, _index);
            }

            protected override void SetItem(int index, ComboMember item)
            {
                base.SetItem(index, item);
                _bar.OnDataUpdated(this, _bar, _index);
            }

            protected override void RemoveItem(int index)
            {
                base.RemoveItem(index);
                _bar.OnDataUpdated(this, _bar, _index);
            }
        }
}


}
