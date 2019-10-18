using MaterialDesignThemes.Wpf;
using OUROBOROS.ViewModel;
using System.Windows.Controls;

namespace OUROBOROS.Controls
{
    /// <summary>
    /// Interaction logic for ProjectHelperStageFileDeliveryView.xaml
    /// </summary>
    public partial class ProjectHelperStageFileDeliveryView : UserControl
    {
        public ProjectHelperStageFileDeliveryView()
        {
            InitializeComponent();
        }
        private void ReviewerChip_OnDeleteClick(object sender, System.Windows.RoutedEventArgs e)
        {
            // Handle the event for the selected button.
            if (DataContext is ProjectHelperStageFileDeliveryViewModel viewModel)
            {
                // Get selected item
                ReviewerItem selectedItem = (ReviewerItem)(e.Source as Chip).DataContext;

                // Delete selected item
                viewModel.RemoveFromSelectedReviewers(selectedItem);
            }
        }
    }
}
