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
    /// Interaction logic for ClearcaseManagerView.xaml
    /// </summary>
    public partial class ClearcaseManagerView : UserControl
    {
        public ClearcaseManagerView()
        {
            InitializeComponent();
        }
        
        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseViewTabCodeControl
        // Method      : ListViewItem_MouseDoubleClick
        // Description : Double clicking a selected search result will open the result.
        // ----------------------------------------------------------------------------------------
        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                //var viewModel = DataContext as ClearcaseViewTabCodeViewModel;
                //viewModel.EditCommand.Execute(null);
            }
        }

        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseViewTabCodeControl
        // Method      : SearchResultsListView_KeyUp
        // Description : Upon a key up event, all selected search results will processed:
        //               Key.Enter - Opens all selected (and eligible) items
        // ----------------------------------------------------------------------------------------
        private void SearchResultsListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.Handled && e.Key == Key.Enter)
            {
                e.Handled = true;
                //var viewModel = DataContext as ClearcaseViewTabCodeViewModel;

                //switch (e.Key)
                //{
                //    case Key.Enter: viewModel.EditCommand.Execute(null); break;
                //}
            }
        }
        
    }
}
