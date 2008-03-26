using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.Commands;
using AnkhSvn.Ids;
using Ankh.Selection;

namespace Ankh.Scc.ProjectMap
{
    class SccDocumentData
    {
        readonly IAnkhServiceProvider _context;
        readonly string _name;
        uint _cookie;
        bool _isDirty;
        bool _initialUpdateCompleted;

        public SccDocumentData(IAnkhServiceProvider context, string name)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            _context = context;
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
        }

        public uint Cookie
        {
            get { return _cookie; }
            internal set
            {
                if (_cookie != 0 && value != 0)
                    throw new InvalidOperationException();

                _cookie = value;
            }
        }

        IVsHierarchy _hierarchy;
        internal IVsHierarchy Hierarchy
        {
            get { return _hierarchy; }
            set { _hierarchy = value; }
        }

        uint _itemId;
        internal uint ItemId
        {
            get { return _itemId; }
            set { _itemId = value; }
        }

        /// <summary>
        /// Called when initialized from existing state; instead of document creation
        /// </summary>
        internal void OnCookieLoad()
        {
            _initialUpdateCompleted = true;
        }

        internal void OnSaved()
        {
        }

        internal void OnClosed(bool closedWithoutSaving)
        {
            if (closedWithoutSaving && _isDirty)
                UpdateGlyph();
            Dispose();
        }

        internal void OnAttributeChange(__VSRDTATTRIB attributes)
        {
            if (0 != (attributes & __VSRDTATTRIB.RDTA_DocDataReloaded))
            {
                if (_initialUpdateCompleted)
                {
                    IFileStatusCache statusCache = _context.GetService<IFileStatusCache>();

                    if (statusCache != null)
                    {
                        if (statusCache.IsValidPath(Name))
                        {
                            statusCache.MarkDirty(Name);
                            UpdateGlyph();
                        }
                    }
                }
                else
                    _initialUpdateCompleted = true;
            }

            if (0 != (attributes & __VSRDTATTRIB.RDTA_DocDataIsDirty))
            {
                _initialUpdateCompleted = true;
                SetDirty(true);

            }
            else if (0 != (attributes & __VSRDTATTRIB.RDTA_DocDataIsNotDirty))
            {
                _initialUpdateCompleted = true;
                SetDirty(false);
            }
        }

        void SetDirty(bool dirty)
        {
            if (dirty == _isDirty)
                return;

            _isDirty = dirty;
            UpdateGlyph();
        }

        void UpdateGlyph()
        {            
            IVsSccProject2 project = Hierarchy as IVsSccProject2;

            if (project != null)
            {
                IProjectNotifier pn = _context.GetService<IProjectNotifier>();

                pn.MarkDirty(new SvnProject(null, project));
            }
        }

        internal void Dispose()
        {
            OpenDocumentTracker tracker = (OpenDocumentTracker)_context.GetService<IAnkhOpenDocumentTracker>();

            if (tracker != null)
                tracker.DoDispose(this);
        }

        internal void CopyState(SccDocumentData data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            _initialUpdateCompleted = data._initialUpdateCompleted;
            _isDirty = data._isDirty;
            _itemId = data._itemId;
            _hierarchy = data._hierarchy;
        }        
    }
}
