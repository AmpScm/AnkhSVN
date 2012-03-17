using System;
using SystemPoint = System.Drawing.Point;
using System.Windows.Media;
using Ankh.VS;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.WpfPackage.Services
{
    [GlobalService(typeof(IGetWpfEditorInfo))]
    sealed class WpfEditorInfoService : AnkhService, IGetWpfEditorInfo
    {
        public WpfEditorInfoService(IAnkhServiceProvider context)
            : base(context)
        {
        }

        sealed class TheWpfEditorInfo : WpfEditorInfo
        {
            readonly IAnkhServiceProvider _context;
            readonly IComponentModel _componentModel;
            readonly IVsEditorAdaptersFactoryService _adapterFactory;
            readonly IVsTextView _textView;

            public TheWpfEditorInfo(IAnkhServiceProvider context, IComponentModel componentModel, IVsEditorAdaptersFactoryService adapterFactory, IVsTextView textView)
            {
                if (context == null)
                    throw new ArgumentNullException("context");
                else if (componentModel == null)
                    throw new ArgumentNullException("componentModel");
                else if (adapterFactory == null)
                    throw new ArgumentNullException("adapterFactory");
                else if (textView == null)
                    throw new ArgumentNullException("textView");

                _context = context;
                _componentModel = componentModel;
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

            public override System.Drawing.Point GetTopLeft()
            {
                IWpfTextView textView = WpfTextView;
                PresentationSource source = Source;

                if (textView == null || source == null)
                    return new SystemPoint();

                FrameworkElement el = textView.VisualElement;

                GeneralTransform toRoot = el.TransformToAncestor(source.RootVisual);

                Point p = toRoot.Transform(new Point(0, 0));

                return new SystemPoint((int)p.X, (int)p.Y);
            }

            public override int GetLineHeight()
            {
                IWpfTextView wpfTextView = WpfTextView;

                if (wpfTextView != null)
                    return (int)wpfTextView.LineHeight;

                return 1;
            }
        }

        public WpfEditorInfo GetWpfInfo(IVsTextView textView)
        {
            if (textView == null)
                throw new ArgumentNullException("textView");

            IComponentModel componentModel = GetService<IComponentModel>(typeof(SComponentModel));
            IVsEditorAdaptersFactoryService factory = componentModel.GetService<IVsEditorAdaptersFactoryService>();

            if (factory != null)
            {
                if (factory.GetWpfTextView(textView) != null)
                    return new TheWpfEditorInfo(this, componentModel, factory, textView);
            }

            return null;
        }
    }

}
