using OUROBOROS.ViewModel;
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

namespace OUROBOROS.Controls
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

        private void CopyReferenceURL_Button_Click(object sender, RoutedEventArgs e)
        {
            // Handle the event for the selected button.
            if (DataContext is LicenseLogViewModel)
            {
                LicenseLogItem viewItem = (LicenseLogItem)(e.Source as Button).DataContext;
                Clipboard.SetText((viewItem.ReferenceURL));
            }
        }
    }
}
