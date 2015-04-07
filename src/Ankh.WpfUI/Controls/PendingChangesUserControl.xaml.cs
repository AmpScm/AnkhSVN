using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinPoint = System.Drawing.Point;
using Ankh.Commands;

namespace Ankh.WpfUI.Controls
{
    /// <summary>
    /// Interaction logic for PendingChangesUserControl.xaml
    /// </summary>
    partial class PendingChangesUserControl : UserControl
    {
        public PendingChangesUserControl()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public IAnkhServiceProvider Context { get; set; }

        private void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = true;
            UIElement el = e.OriginalSource as UIElement ?? PendingChangesList;

            Point wp = el.PointToScreen(new Point(e.CursorLeft, e.CursorTop));
            WinPoint p = new WinPoint((int)wp.X, (int)wp.Y);
            bool showSort = false;

            if (PendingChangesList.ContainerFromElement(el) == null)
                showSort = true;

            IAnkhCommandService mcs = Context.GetService<IAnkhCommandService>();
            if (mcs != null)
            {
                if (showSort)
                    mcs.ShowContextMenu(AnkhCommandMenu.PendingCommitsHeaderContextMenu, p);
                else
                    mcs.ShowContextMenu(AnkhCommandMenu.PendingCommitsContextMenu, p);
            }
        }
    }
}
