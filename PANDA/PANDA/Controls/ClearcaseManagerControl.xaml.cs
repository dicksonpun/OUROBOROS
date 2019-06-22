using PANDA.ViewModel;
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

namespace PANDA.Controls
{
    /// <summary>
    /// Interaction logic for ClearcaseManagerControl.xaml
    /// </summary>
    public partial class ClearcaseManagerControl : UserControl
    {
        public ClearcaseManagerControl()
        {
            InitializeComponent();
        }
        private void BackgroundRefreshBadgeButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Handle the event for the selected button.
            ClearcaseManagerViewItem selectedViewItem = (ClearcaseManagerViewItem)(e.Source as Button).DataContext;
            string selectedView = selectedViewItem.ViewName;
        }
    }
}
