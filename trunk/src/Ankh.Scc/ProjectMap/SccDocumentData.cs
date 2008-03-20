using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.Commands;
using AnkhSvn.Ids;

namespace Ankh.Scc.ProjectMap
{
    class SccDocumentData
    {
        readonly IAnkhServiceProvider _context;
        readonly string _name;
        uint _cookie;
        bool _isDirty;

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
            if (0 != (attributes & __VSRDTATTRIB.RDTA_DocDataIsDirty))
            {
                SetDirty(true);
            }
            else if (0 != (attributes & __VSRDTATTRIB.RDTA_DocDataIsNotDirty))
            {
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
            IAnkhCommandService cmd = _context.GetService<IAnkhCommandService>();

            if (cmd != null)
                cmd.PostExecCommand(AnkhCommand.MarkProjectDirty, this);
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

            _isDirty = data._isDirty;
        }
    }
}
