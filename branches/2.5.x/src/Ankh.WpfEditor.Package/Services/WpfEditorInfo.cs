using System;
using System.Windows;
using System.Windows.Media;
using SystemPoint = System.Drawing.Point;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Text.Formatting;

using Ankh.VS;

namespace Ankh.WpfPackage.Services
{
    [GlobalService(typeof(IGetWpfEditorInfo))]
    sealed class WpfEditorInfoService : AnkhService, IGetWpfEditorInfo
    {
        public WpfEditorInfoService(IAnkhServiceProvider context)
            : base(context)
        {
        }

        sealed class TheWpfEditorInfo : IWpfEditorInfo
        {
            readonly IAnkhServiceProvider _context;
            readonly IVsEditorAdaptersFactoryService _adapterFactory;
            readonly IVsTextView _textView;

            public TheWpfEditorInfo(IAnkhServiceProvider context, IVsEditorAdaptersFactoryService adapterFactory, IVsTextView textView)
            {
                if (context == null)
                    throw new ArgumentNullException("context");
                else if (adapterFactory == null)
                    throw new ArgumentNullException("adapterFactory");
                else if (textView == null)
                    throw new ArgumentNullException("textView");

                _context = context;
                _textView = textView;
                _adapterFactory = adapterFactory;
            }

            IWpfTextView _wpfTextView;
            PresentationSource _source;

            IWpfTextView WpfTextView
            {
                get
                {
                    Refresh();

                    return _wpfTextView;
                }
            }

            public PresentationSource Source
            {
                get
                {
                    Refresh();

                    return _source;
                }
            }

            void Refresh()
            {
                if (_wpfTextView != null && !_wpfTextView.IsClosed)
                    return;

                _wpfTextView = null;
                _source = null;
                    
                IWpfTextView wpfTextView = _adapterFactory.GetWpfTextView(_textView);
                if (wpfTextView.IsClosed)
                    return;

                _wpfTextView = wpfTextView;
                _source = PresentationSource.FromVisual(wpfTextView.VisualElement);
            }

            public System.Drawing.Point GetTopLeft()
            {
                IWpfTextView textView = WpfTextView;
                PresentationSource source = Source;

                if (textView == null || source == null)
                    return new SystemPoint();

                FrameworkElement el = textView.VisualElement;
                IWpfTextViewLine line = textView.TextViewLines.FirstVisibleLine;
                int textTop = (int)line.TextTop;
                int visibleTop = (int)line.VisibleArea.Top;
                int diff = (textTop - visibleTop) % (int)textView.LineHeight;
                GeneralTransform toRoot = el.TransformToAncestor(source.RootVisual);

                Point p = toRoot.Transform(new Point(0, diff));

                return new SystemPoint((int)p.X, (int)p.Y);
            }

            public int GetLineHeight()
            {
                IWpfTextView wpfTextView = WpfTextView;

                if (wpfTextView != null)
                    return (int)wpfTextView.LineHeight;

                return 1;
            }
        }

        public IWpfEditorInfo GetWpfInfo(IVsTextView textView)
        {
            if (textView == null)
                throw new ArgumentNullException("textView");

            IComponentModel componentModel = GetService<IComponentModel>(typeof(SComponentModel));
            IVsEditorAdaptersFactoryService factory = componentModel.GetService<IVsEditorAdaptersFactoryService>();

            if (factory != null)
            {
                if (factory.GetWpfTextView(textView) != null)
                    return new TheWpfEditorInfo(this, factory, textView);
            }

            return null;
        }
    }

}
