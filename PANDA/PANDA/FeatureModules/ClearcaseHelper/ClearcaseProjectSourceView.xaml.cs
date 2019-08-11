using MaterialDesignThemes.Wpf;
using PANDA.ViewModel;
using System.Windows.Controls;

namespace PANDA.Controls
{
    /// <summary>
    /// Interaction logic for ClearcaseProjectSourceView.xaml
    /// </summary>
    public partial class ClearcaseProjectSourceView : UserControl
    {
        public ClearcaseProjectSourceView()
        {
            InitializeComponent();
        }

        private void FilterTagChip_OnDeleteClick(object sender, System.Windows.RoutedEventArgs e)
        {
            // Handle the event for the selected button.
            if (DataContext is ClearcaseProjectSourceViewModel viewModel)
            {
                // Get selected item
                ClearcaseVOBItem selectedItem = (ClearcaseVOBItem)(e.Source as Chip).DataContext;

                // Delete selected item
                viewModel.RemoveFromSelectedVOBFilters(selectedItem);
            }
        }
    }
}
