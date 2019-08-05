using MaterialDesignExtensions.Controls;
using MaterialDesignThemes.Wpf;
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

        private void Button_Click_MountOrUnmountY(object sender, RoutedEventArgs e)
        {
            // Handle the event for the selected button.
            if (DataContext is ClearcaseViewHelperViewModel viewModel)
            {
                ClearcaseManagerViewItem selectedViewItem = (ClearcaseManagerViewItem)(e.Source as Button).DataContext;

                // Unmount the current viewPath in upfront to cover both mount and unmount cases
                viewModel.AccessNavigationHelper.AccessYDriveMounter.Unmount();

                // Only mount if the current selectedViewItem is not already mapped
                if (!selectedViewItem.Icon.Equals(PackIconKind.LetterYBox))
                {
                    viewModel.AccessNavigationHelper.AccessYDriveMounter.Mount(selectedViewItem.ViewPath);
                }
            }
        }

        private void Button_Click_RemoveView(object sender, RoutedEventArgs e)
        {
            // Handle the event for the selected button.
            if (DataContext is ClearcaseViewHelperViewModel viewModel)
            {
                // Get username
                string username = viewModel.AccessNavigationHelper.AccessUserSettingsHelper.UserProfile.Username.ToString();

                ClearcaseManagerViewItem selectedViewItem = (ClearcaseManagerViewItem)(e.Source as Button).DataContext;
                // Determine if user or non-user view
                if (selectedViewItem.ViewName.Split('-').First().Equals(username))
                {
                    // Username view is deleted
                    MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Delete Confirmation", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        // Do nothing for now.
                        return;
                    }
                }
                else
                {
                    // Non-user view is simply removed from navigation menu
                    viewModel.AccessNavigationHelper.RemoveSelectedView(selectedViewItem.ViewName);
                }
            }
        }

        private async void OpenDirectoryDialogButtonClickHandler(object sender, RoutedEventArgs args)
        {
            OpenDirectoryDialogArguments dialogArgs = new OpenDirectoryDialogArguments()
            {
                Width = 1000,
                Height = 650,
                CreateNewDirectoryEnabled = true
            };

            OpenDirectoryDialogResult result = await OpenDirectoryDialog.ShowDialogAsync(MainWindow.DialogHostName, dialogArgs);
            
            if (DataContext is ClearcaseViewHelperViewModel viewModel)
            {
                if (!result.Canceled)
                {
                    viewModel.SelectedAction = "Selected directory: " + result.DirectoryInfo.FullName;
                }
                else
                {
                    viewModel.SelectedAction = "Cancel open directory";
                }
            }
        }

    }
}
