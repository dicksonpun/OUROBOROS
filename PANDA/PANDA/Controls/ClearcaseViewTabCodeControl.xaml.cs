using PANDA.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for ClearcaseViewTabCodeControl.xaml
    /// </summary>
    public partial class ClearcaseViewTabCodeControl : UserControl
    {
        public ClearcaseViewTabCodeControl()
        {
            InitializeComponent();
        }

        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseViewTabCodeControl
        // Method      : SearchResults_ListView_PreviewMouseWheel
        // Description : The preview mouse wheel event must be captured in the inner listview,
        //               and then stop the event from scrolling the listview and raise the event 
        //               in the parent (ScrollViewer).
        // Credit      : https://stackoverflow.com/questions/1585462/bubbling-scroll-events-from-a-listview-to-its-parent
        // ----------------------------------------------------------------------------------------
        private void SearchResults_ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var viewModel = DataContext as ClearcaseViewTabCodeViewModel;
                viewModel.EditCommand.Execute(null);
            }
        }

        private void SearchResultsListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.Handled && e.Key == Key.Enter)
            {
                e.Handled = true;
                var viewModel = DataContext as ClearcaseViewTabCodeViewModel;

                switch (e.Key)
                {
                    case Key.Enter: viewModel.EditCommand.Execute(null); break;
                }
            }
        }
    }
}
