// Copyright 2026 The AnkhSVN Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ankh.Commands;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using SharpSvn;

namespace Ankh.UI.Annotate
{
    internal sealed class AnnotationMargin : Canvas, IWpfTextViewMargin
    {
        public const string MarginName = "AnkhSVNAnnotationMargin";

        readonly IWpfTextView _textView;
        readonly string _fileName;
        readonly IAnkhServiceProvider _context;
        readonly IVsTrackSelectionEx _selectionTracker;
        readonly AnnotationSelectionContainer _selectionContainer;
        readonly List<MarginRegion> _regions = new List<MarginRegion>();
        bool _disposed;
        MarginRegion _selectedRegion;

        public AnnotationMargin(IWpfTextView textView, string fileName, AnnotationDocument document)
        {
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            _context = document.Context;
            _selectionTracker = _context.GetService<IVsTrackSelectionEx>(typeof(SVsTrackSelectionEx));
            _selectionContainer = new AnnotationSelectionContainer(SelectSource);

            Width = 175;
            ClipToBounds = true;
            Background = SystemColors.ControlBrush;

            BuildRegions(document);

            PreviewMouseRightButtonDown += OnPreviewMouseRightButtonDown;
            ContextMenuOpening += OnContextMenuOpening;

            _textView.LayoutChanged += OnLayoutChanged;
            _textView.Closed += OnTextViewClosed;
            Loaded += OnLoaded;
            SizeChanged += OnMarginSizeChanged;
        }

        void BuildRegions(AnnotationDocument document)
        {
            Dictionary<long, AnnotateSource> sources = new Dictionary<long, AnnotateSource>();
            MarginRegion current = null;

            foreach (SvnBlameEventArgs blame in document.BlameResult)
            {
                AnnotateSource source;
                if (!sources.TryGetValue(blame.Revision, out source))
                {
                    source = new AnnotateSource(blame, document.Origin);
                    sources.Add(blame.Revision, source);
                }

                int line = checked((int)blame.LineNumber);
                if (current == null || !ReferenceEquals(current.Source, source))
                {
                    MarginRegion newRegion = new MarginRegion(line, source, CreateRegionElement(source));
                    newRegion.Element.MouseLeftButtonDown += delegate
                    {
                        SelectRegion(newRegion);
                    };
                    _regions.Add(newRegion);
                    Children.Add(newRegion.Element);
                    current = newRegion;
                }
                else
                {
                    current.EndLine = line;
                }
            }
        }

        Border CreateRegionElement(AnnotateSource source)
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(45) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            TextBlock revision = CreateTextBlock(source.Revision >= 0
                ? source.Revision.ToString(CultureInfo.CurrentCulture)
                : string.Empty);
            revision.TextAlignment = TextAlignment.Right;
            Grid.SetColumn(revision, 0);

            TextBlock author = CreateTextBlock(source.Author ?? string.Empty);
            author.Margin = new Thickness(5, 0, 5, 0);
            Grid.SetColumn(author, 1);

            TextBlock date = CreateTextBlock(source.Revision >= 0
                ? source.Time.ToShortDateString()
                : string.Empty);
            Grid.SetColumn(date, 2);

            grid.Children.Add(revision);
            grid.Children.Add(author);
            grid.Children.Add(date);

            return new Border
            {
                Background = SystemColors.ControlBrush,
                BorderBrush = SystemColors.ControlDarkBrush,
                BorderThickness = new Thickness(0, 0, 0, 1),
                Padding = new Thickness(3, 0, 3, 0),
                ClipToBounds = true,
                Child = grid,
                ToolTip = CreateToolTip(source)
            };
        }

        static TextBlock CreateTextBlock(string text)
        {
            return new TextBlock
            {
                Text = text,
                Foreground = SystemColors.ControlTextBrush,
                FontSize = 11,
                VerticalAlignment = VerticalAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis,
                TextWrapping = TextWrapping.NoWrap
            };
        }

        static string CreateToolTip(AnnotateSource source)
        {
            if (source.Revision < 0)
                return AnnotateResources.LocalChange;

            string text = string.Format(
                CultureInfo.CurrentCulture,
                "Revision: {0}\nAuthor: {1}\nTime: {2}",
                source.Revision,
                source.Author,
                source.Time);

            if (!string.IsNullOrEmpty(source.LogMessage))
                text += "\n\n" + source.LogMessage;

            return text;
        }

        void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_disposed || e == null)
                return;

            MarginRegion region = FindRegionAt(e.GetPosition(this));
            if (region == null)
                return;

            // Intercept the click before the native editor sees it. Otherwise VS can
            // open its normal editor context menu (Outlining/Breakpoints/etc.) over
            // the annotation margin.
            e.Handled = true;
            SelectRegion(region);
            ShowContextMenu(this, e);
        }

        void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            // The annotation margin owns right-click behavior. Suppress the editor's
            // fallback context menu if WPF subsequently raises ContextMenuOpening.
            e.Handled = true;
        }

        MarginRegion FindRegionAt(Point point)
        {
            foreach (MarginRegion region in _regions)
            {
                if (region.Element.Visibility != Visibility.Visible)
                    continue;

                double top = GetTop(region.Element);
                if (double.IsNaN(top))
                    continue;

                double height = region.Element.ActualHeight > 0
                    ? region.Element.ActualHeight
                    : region.Element.Height;

                if (height > 0 && point.Y >= top && point.Y < top + height)
                    return region;
            }

            return null;
        }

        void SelectRegion(MarginRegion region)
        {
            if (_selectedRegion != null)
            {
                _selectedRegion.Element.Background = SystemColors.ControlBrush;
                SetTextBrush(_selectedRegion.Element, SystemColors.ControlTextBrush);
            }

            _selectedRegion = region;
            if (_selectedRegion != null)
            {
                _selectedRegion.Element.Background = SystemColors.HighlightBrush;
                SetTextBrush(_selectedRegion.Element, SystemColors.HighlightTextBrush);
            }

            SelectSource(_selectedRegion != null ? _selectedRegion.Source : null);
        }

        void SelectSource(AnnotateSource source)
        {
            _selectionContainer.Selected = source;

            if (_selectionTracker != null)
                _selectionTracker.OnSelectChange(_selectionContainer);
        }

        void ShowContextMenu(FrameworkElement element, MouseButtonEventArgs e)
        {
            if (_context == null || element == null || e == null)
                return;

            IAnkhCommandService commandService = _context.GetService<IAnkhCommandService>();
            if (commandService == null)
                return;

            Point screenPoint = element.PointToScreen(e.GetPosition(element));
            commandService.ShowContextMenu(
                AnkhCommandMenu.AnnotateContextMenu,
                (int)Math.Round(screenPoint.X),
                (int)Math.Round(screenPoint.Y));
        }

        static void SetTextBrush(Border border, System.Windows.Media.Brush brush)
        {
            Grid grid = border.Child as Grid;
            if (grid == null)
                return;

            foreach (UIElement child in grid.Children)
            {
                TextBlock text = child as TextBlock;
                if (text != null)
                    text.Foreground = brush;
            }
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            RefreshLayout();
        }

        void OnMarginSizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshLayout();
        }

        void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            RefreshLayout();
        }

        void RefreshLayout()
        {
            if (_disposed || !_textView.VisualElement.IsVisible)
                return;

            ITextSnapshot snapshot = _textView.TextSnapshot;
            ITextViewLineCollection lines = _textView.TextViewLines;
            if (snapshot == null || lines == null || lines.Count == 0)
            {
                HideAllRegions();
                return;
            }

            Height = Math.Max(0, _textView.ViewportHeight);

            ITextViewLine firstVisibleLine = lines.FirstVisibleLine;
            ITextViewLine lastVisibleLine = lines.LastVisibleLine;
            if (firstVisibleLine == null || lastVisibleLine == null)
            {
                HideAllRegions();
                return;
            }

            foreach (MarginRegion region in _regions)
            {
                if (region.StartLine < 0 || region.EndLine < region.StartLine ||
                    region.StartLine >= snapshot.LineCount || region.EndLine >= snapshot.LineCount)
                {
                    region.Element.Visibility = Visibility.Collapsed;
                    continue;
                }

                ITextSnapshotLine startLine = snapshot.GetLineFromLineNumber(region.StartLine);
                ITextSnapshotLine endLine = snapshot.GetLineFromLineNumber(region.EndLine);

                if (endLine.EndIncludingLineBreak.Position <= firstVisibleLine.Start.Position ||
                    startLine.Start.Position >= lastVisibleLine.EndIncludingLineBreak.Position)
                {
                    region.Element.Visibility = Visibility.Collapsed;
                    continue;
                }

                double regionTop;
                if (startLine.Start.Position < firstVisibleLine.Start.Position)
                {
                    regionTop = _textView.ViewportTop;
                }
                else
                {
                    ITextViewLine startViewLine = _textView.GetTextViewLineContainingBufferPosition(startLine.Start);
                    if (startViewLine == null)
                    {
                        region.Element.Visibility = Visibility.Collapsed;
                        continue;
                    }
                    regionTop = startViewLine.Top;
                }

                double regionBottom;
                if (endLine.EndIncludingLineBreak.Position > lastVisibleLine.EndIncludingLineBreak.Position)
                {
                    regionBottom = _textView.ViewportTop + _textView.ViewportHeight;
                }
                else
                {
                    int endPosition = Math.Max(endLine.Start.Position, endLine.End.Position - 1);
                    SnapshotPoint endPoint = new SnapshotPoint(snapshot, endPosition);
                    ITextViewLine endViewLine = _textView.GetTextViewLineContainingBufferPosition(endPoint);
                    if (endViewLine == null)
                    {
                        region.Element.Visibility = Visibility.Collapsed;
                        continue;
                    }
                    regionBottom = endViewLine.Bottom;
                }

                AnnotationRegionLayout layout = AnnotationLayoutCalculator.Calculate(
                    regionTop,
                    regionBottom,
                    _textView.ViewportTop,
                    _textView.ViewportHeight);

                if (!layout.IsVisible)
                {
                    region.Element.Visibility = Visibility.Collapsed;
                    continue;
                }

                region.Element.Visibility = Visibility.Visible;
                region.Element.Width = Width;
                region.Element.Height = layout.Height;
                SetTop(region.Element, layout.Top);
            }
        }

        void HideAllRegions()
        {
            foreach (MarginRegion region in _regions)
                region.Element.Visibility = Visibility.Collapsed;
        }

        void OnTextViewClosed(object sender, EventArgs e)
        {
            AnnotationDocumentRegistry.Unregister(_fileName);
            Dispose();
        }

        public FrameworkElement VisualElement
        {
            get
            {
                ThrowIfDisposed();
                return this;
            }
        }

        public double MarginSize
        {
            get
            {
                ThrowIfDisposed();
                return ActualWidth > 0 ? ActualWidth : Width;
            }
        }

        public bool Enabled
        {
            get
            {
                ThrowIfDisposed();
                return true;
            }
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return string.Equals(marginName, MarginName, StringComparison.OrdinalIgnoreCase)
                ? this
                : null;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            PreviewMouseRightButtonDown -= OnPreviewMouseRightButtonDown;
            ContextMenuOpening -= OnContextMenuOpening;
            _textView.LayoutChanged -= OnLayoutChanged;
            _textView.Closed -= OnTextViewClosed;
            Loaded -= OnLoaded;
            SizeChanged -= OnMarginSizeChanged;
            Children.Clear();
            GC.SuppressFinalize(this);
        }

        void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(MarginName);
        }

        sealed class AnnotationSelectionContainer : ISelectionContainer
        {
            readonly Action<AnnotateSource> _selectSource;

            public AnnotationSelectionContainer(Action<AnnotateSource> selectSource)
            {
                _selectSource = selectSource ?? throw new ArgumentNullException(nameof(selectSource));
            }

            public AnnotateSource Selected { get; set; }

            public int CountObjects(uint dwFlags, out uint pc)
            {
                pc = Selected != null ? 1u : 0u;
                return 0;
            }

            public int GetObjects(uint dwFlags, uint cObjects, object[] apUnkObjects)
            {
                if (apUnkObjects == null)
                    return unchecked((int)0x80004003); // E_POINTER

                if (Selected == null)
                    return cObjects == 0 ? 0 : unchecked((int)0x80004005); // E_FAIL

                if (cObjects != 1 || apUnkObjects.Length < 1)
                    return unchecked((int)0x80004005); // E_FAIL

                apUnkObjects[0] = Selected;
                return 0;
            }

            public int SelectObjects(uint cSelect, object[] apUnkSelect, uint dwFlags)
            {
                if (cSelect == 0)
                {
                    _selectSource(null);
                    return 0;
                }

                if (cSelect != 1 || apUnkSelect == null || apUnkSelect.Length < 1)
                    return unchecked((int)0x80004005); // E_FAIL

                AnnotateSource source = apUnkSelect[0] as AnnotateSource;
                if (source == null)
                    return unchecked((int)0x80004002); // E_NOINTERFACE

                _selectSource(source);
                return 0;
            }
        }

        sealed class MarginRegion
        {
            public MarginRegion(int line, AnnotateSource source, Border element)
            {
                StartLine = EndLine = line;
                Source = source;
                Element = element;
            }

            public int StartLine { get; }
            public int EndLine { get; set; }
            public AnnotateSource Source { get; }
            public Border Element { get; }
        }
    }
}
