using MaterialDesignExtensions.Controls;
using MaterialDesignThemes.Wpf;
using PANDA.Controls;
using PANDA.ViewModel;
using System;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Input;

namespace PANDA
{
    // Reference: https://stackoverflow.com/questions/2914819/what-is-the-purpose-of-the-permissionset-attribute-in-the-msdn-filesystemwatcher
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public partial class MainWindow : Window
    {
        public MainWindowViewModel mainWindowViewModel;
        public const string DialogHostName = "dialogHost";

        // Constructor
        public MainWindow()
        {
            InitializeComponent();
            mainWindowViewModel = new MainWindowViewModel(this);
            mainWindowViewModel.Initialize();
 
#if DEBUG
            // Suppresses benign binding errors like : 
            // "System.Windows.Data Error: 4 : Cannot find source for binding with reference 'RelativeSource FindAncestor, AncestorType=..."
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
#endif
        }

        public void NavigationItemSelectedHandler(object sender, NavigationItemSelectedEventArgs args)
        {
            mainWindowViewModel.NavigationHelper.SelectNavigationItem(args.NavigationItem);
        }


        // EXPERIMENTAL BUTTONS
        private void Button_Click_Mount(object sender, RoutedEventArgs e)
        {
            //mainWindowViewModel.NavigationHelper.StartAutoRefresh();
        }

        private void Button_Click_Unmount(object sender, RoutedEventArgs e)
        {
            //mainWindowViewModel.NavigationHelper.StopAutoRefresh();
        }

        private void AppBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        // AppBar Events
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            MaximizeToggle();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow(sender);
        }

        private async void CloseWindow(object o)
        {
            if (AnyNonPeriodicTasksRunning())
            {
                var view = new UserConfirmationControl
                {
                    DataContext = new UserConfirmationViewModel("Confirmation",
                    "Are you sure you want to exit?")
                };
                if (!(bool)await DialogHost.Show(view, "dialogHost"))
                {
                    return;
                }
            }
            ExitApplication();
        }

        public bool AnyNonPeriodicTasksRunning()
        {
            // TODO: create long running tasks manager for reference
            return true;
        }

        public void ExitApplication()
        {
            base.Close();
        }

        private void AppBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MaximizeToggle();
        }

        private void MaximizeToggle()
        {
            if (WindowState.ToString() == "Normal")
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState.ToString() == "Normal")
            {
                this.BorderThickness = new Thickness(0);
            }
            else
            {
                this.BorderThickness = new Thickness(5);
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
