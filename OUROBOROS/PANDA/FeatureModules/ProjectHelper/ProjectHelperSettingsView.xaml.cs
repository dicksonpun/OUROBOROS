using MaterialDesignThemes.Wpf;
using OUROBOROS.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace OUROBOROS.Controls
{
    /// <summary>
    /// Interaction logic for ProjectHelperSettingsView.xaml
    /// </summary>
    public partial class ProjectHelperSettingsView : UserControl
    {
        public ProjectHelperSettingsView()
        {
            InitializeComponent();
        }

        private void FilterTagChip_OnDeleteClick(object sender, RoutedEventArgs e)
        {
            // Handle the event for the selected button.
            if (DataContext is ProjectHelperSettingsViewModel viewModel)
            {
                // Get selected item
                VOBItem selectedItem = (VOBItem)(e.Source as Chip).DataContext;

                // Delete selected item
                viewModel.RemoveFromSelectedVOBs(selectedItem);
            }
        }
    }
}
