using System;
using Ankh.Commands;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.VS.Services
{
    using UnregisterHandler = InternalDiff.UnregisterMergeWindow;
    using QueryHandler = InternalDiff.QueryMergeWindowState;

    sealed class DiffMergeInstance : AnkhService, IVsWindowFrameNotify, IVsWindowFrameNotify2, IVsWindowFrameNotify3, IDisposable
    {
        readonly bool _isMerge;
        UnregisterHandler _unregister;
        QueryHandler _query;
        int _mergeCookie;
        IVsWindowFrame _frame;
        IVsWindowFrame2 _frame2;
        uint _frameCookie;
        bool _frameHooked;
        bool _mergeHooked;


        public DiffMergeInstance(IAnkhServiceProvider context, IVsWindowFrame frame)
            : base(context)
        {
            _frame = frame;
            _frame2 = frame as IVsWindowFrame2;

            if (_frame2 != null && ErrorHandler.Succeeded(_frame2.Advise(this, out  _frameCookie)))
            {
                _frameHooked = true;
            }
        }

        public DiffMergeInstance(IAnkhServiceProvider context, IVsWindowFrame frame, UnregisterHandler unRegisterhandler, QueryHandler queryHandler, int mergeCookie)
            : this(context, frame)
        {
            _isMerge = true;
            _unregister = unRegisterhandler;
            _query = queryHandler;
            _mergeCookie = mergeCookie;
            _mergeHooked = true;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (_mergeHooked)
                    {
                        _mergeHooked = false;

                        UnregisterHandler unregister = _unregister;

                        IAnkhCommandService cs = GetService<IAnkhCommandService>();
                        if (cs != null)
                            cs.PostIdleAction(delegate
                                    {
                                        unregister(_mergeCookie);
                                    });
                    }

                    if (_frameHooked)
                    {
                        _frameHooked = false;
                        _frame2.Unadvise(_frameCookie);
                    }
                }

                // Unbreak possible circular dependency
                _frame = null;
                _frame2 = null;
                _unregister = null;
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public int OnDockableChange(int fDockable)
        {
            return VSConstants.S_OK;
        }

        public int OnMove()
        {
            return VSConstants.S_OK;
        }

        public int OnShow(int fShow)
        {
            if (fShow == (int)__FRAMESHOW.FRAMESHOW_WinClosed)
                Dispose(true);
            return VSConstants.S_OK;
        }

        public int OnSize()
        {
            return VSConstants.S_OK;
        }

        public int OnClose(ref uint pgrfSaveOptions)
        {
            if (_isMerge)
            {
                // Prompt for handled, etc?
            }
            return VSConstants.S_OK;
        }

        public int OnDockableChange(int fDockable, int x, int y, int w, int h)
        {
            return VSConstants.S_OK;
        }

        public int OnMove(int x, int y, int w, int h)
        {
            return VSConstants.S_OK;
        }

        public int OnSize(int x, int y, int w, int h)
        {
            return VSConstants.S_OK;
        }
    }
}
