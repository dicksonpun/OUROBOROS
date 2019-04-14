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
    /// Interaction logic for LicensesControl.xaml
    /// </summary>
    public partial class LicenseLogControl : UserControl
    {
        public LicenseLogControl()
        {
            InitializeComponent();
        }

        
        private void ContextMenu_CopyReferenceURL_Button_Click(object sender, RoutedEventArgs e)
        {
            // Handle the event for the selected ListViewItem. Access it by casting to its datatype:
            var selectedListViewItem = this.LicenseDataGrid.SelectedItem as PANDA.ViewModel.LicenseLogItem;

            if (this.LicenseDataGrid.SelectedIndex > -1)
            {
                // Copy the reference URL
                Clipboard.SetText((selectedListViewItem.ReferenceURL.ToString()));
            }            
        }
    }
}
