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

namespace Ankh.WpfUI.Controls
{
    /// <summary>
    /// Interaction logic for PendingChangesUserControl.xaml
    /// </summary>
    public partial class PendingChangesUserControl : UserControl
    {
        public PendingChangesUserControl()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public IAnkhServiceProvider Context { get; set; }
    }
}
