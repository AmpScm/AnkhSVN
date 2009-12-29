using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Ankh.VS.LanguageServices.Core
{
    public class AnkhCodeWindowManager : AnkhService, IVsCodeWindowManager, IVsCodeWindowEvents
    {
        IVsCodeWindow _window;
        AnkhLanguageDropDownBar _bar;
        List<IVsTextView> _views;
        uint _cookie;

        [CLSCompliant(false)]
        public AnkhCodeWindowManager(AnkhLanguage language, IVsCodeWindow window)
            : base(language)
        {
            if (window == null)
                throw new ArgumentNullException("window");

            _window = window;

            if (!TryHookConnectionPoint<IVsCodeWindowEvents>(_window, this, out _cookie))
                _cookie = 0;
        }

        public AnkhLanguage Language
        {
            get { return (AnkhLanguage)Context; }
        }

        public void Close()
        {
            object window = _window;
            _window = null;
            if (window != null)
            {
                if (_cookie != 0)
                {
                    ReleaseHook<IVsCodeWindowEvents>(window, _cookie);
                    _cookie = 0;
                }

                if (Marshal.IsComObject(window))
                    try
                    {
                        Marshal.ReleaseComObject(window);
                    }
                    catch { }
            }
        }

        public int AddAdornments()
        {
            IVsTextView primaryView, secondaryView;
            if (ErrorHandler.Succeeded(_window.GetPrimaryView(out primaryView)) && primaryView != null)
                OnNewView(primaryView);

            if (ErrorHandler.Succeeded(_window.GetSecondaryView(out secondaryView)) && secondaryView != null)
                OnNewView(secondaryView);

            if (primaryView != null || secondaryView != null)
            {
                AnkhLanguageDropDownBar bar = Language.CreateDropDownBar(this);

                if (bar != null)
                {
                    _bar = bar;

                    bar.Initialize(this);
                }
            }

            return VSConstants.S_OK;
        }

        public int RemoveAdornments()
        {
            AnkhLanguageDropDownBar bar = _bar;
            _bar = null;

            if (bar != null)
                bar.Close();
            return VSConstants.S_OK;
        }

        [CLSCompliant(false)]
        public virtual int OnNewView(IVsTextView view)
        {
            if (!_views.Contains(view))
            {
                _views.Add(view);
                Language.OnNewView(this, view);

                if (_bar != null)
                    _bar.OnNewView(view);
            }

            return VSConstants.S_OK;
        }

        #region IVsCodeWindowEvents Members

        int IVsCodeWindowEvents.OnCloseView(IVsTextView view)
        {
            if (_views.Contains(view))
            {
                _views.Remove(view);

                if (_bar != null)
                    _bar.OnCloseView(view);


                Language.OnCloseView(this, view);

                OnCloseView(view);

                if (_views.Count == 0)
                    Close();
            }

            return VSConstants.S_OK;
        }

        protected virtual void OnCloseView(IVsTextView view)
        {
            
        }

        #endregion
    }
}
