using MaterialDesignThemes.Wpf;
using OUROBOROS.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OUROBOROS.Controls
{
    /// <summary>
    /// Interaction logic for ProjectHelperView.xaml
    /// </summary>
    public partial class ProjectHelperView : UserControl
    {
        public ProjectHelperView()
        {
            InitializeComponent();
        }

        // ----------------------------------------------------------------------------------------
        // Class       : ProjectHelperViewModel
        // Method      : ListViewItem_MouseDoubleClick
        // Description : Double clicking a selected search result will open the result.
        // ----------------------------------------------------------------------------------------
        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var viewModel = DataContext as ProjectHelperViewModel;
                viewModel.OpenCommand.Execute(null);
            }
        }

        // ----------------------------------------------------------------------------------------
        // Class       : ProjectHelperViewModel
        // Method      : SearchResultsListView_KeyUp
        // Description : Upon a key up event, all selected search results will processed:
        //               Key.Enter - Opens all selected (and eligible) items
        // ----------------------------------------------------------------------------------------
        private void SearchResultsListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.Handled && e.Key == Key.Enter)
            {
                e.Handled = true;
                var viewModel = DataContext as ProjectHelperViewModel;

                switch (e.Key)
                {
                    case Key.Enter: viewModel.OpenCommand.Execute(null); break;
                }
            }
        }

        private void FilterTagChip_OnDeleteClick(object sender, RoutedEventArgs e)
        {
            // Handle the event for the selected button.
            if (DataContext is ProjectHelperViewModel viewModel)
            {
                // Get selected item
                VOBItem selectedItem = (VOBItem)(e.Source as Chip).DataContext;

                // Delete selected item
                viewModel.RemoveFromSelectedVOBs(selectedItem);
            }
        }

        private void Settings_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenSettings(sender);
        }

        private async void OpenSettings(object o)
        {
            var helperViewModel = (ProjectHelperViewModel)DataContext;

            // Save working copy of selection, in case user decides to cancel.
            ObservableCollection<VOBItem> tempSelectedVOBs = new ObservableCollection<VOBItem>(helperViewModel.SelectedVOBs);

            // Setup Settings Dialog Host
            var view = new ProjectHelperSettingsView
            {
                DataContext = new ProjectHelperSettingsViewModel()
                {
                    CurrentView = helperViewModel.CurrentView,
                    SelectedVOBs = tempSelectedVOBs
                }
            };

            // Show Settings Dialog Host
            if (!(bool)await DialogHost.Show(view, "dialogHost"))
            {
                return; // Cancel
            }

            // Save entries
            var settingsViewModel = (ProjectHelperSettingsViewModel)view.DataContext;

            if (settingsViewModel.SelectedView != null)
            {
                helperViewModel.SelectedView = settingsViewModel.SelectedView;
            }
            if (settingsViewModel.SelectedVOBs != null)
            {
                helperViewModel.SelectedVOBs = tempSelectedVOBs;
            }
        }

        private void MountOrUnmountY_Button_Click(object sender, RoutedEventArgs e)
        {
            // Handle the event for the selected button.
            if (DataContext is ProjectHelperViewModel viewModel)
            {
                DriveMounter YDriveMounter = new DriveMounter("Y:");

                if (viewModel.SelectedView != null)
                {
                    // Unmount existing Y-Drive mapping first, if any
                    YDriveMounter.Unmount();

                    // Mount current view to Y-Drive
                    YDriveMounter.Mount(viewModel.SelectedView.FullName);
                }
            }
        }
    }
}
