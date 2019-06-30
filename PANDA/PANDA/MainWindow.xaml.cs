using MaterialDesignExtensions.Controls;
using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;
using PANDA.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;

namespace PANDA
{
    public partial class MainWindow : Window
    {
        public MainWindowViewModel mainWindowViewModel;
        public const string DialogHostName = "dialogHost";

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

            // Update Appbar Title
            mainWindowViewModel.mainWindow.appBar.Title = ((FirstLevelNavigationItem)args.NavigationItem).Label;
        }

        // EXPERIMENTAL BUTTONS
        private void Button_Click_Mount(object sender, RoutedEventArgs e)
        {
            //mainWindowViewModel.YDriveMounter.Mount(@"C:\Users\Dickson\Desktop\testViews\dickson-branchname-1");
            //mainWindowViewModel.NavigationHelper.StartAutoRefresh();

            // TEST ADDING NON USER VIEW
            mainWindowViewModel.NavigationHelper.AddSelectedView("aaaaa-tehee");
            //AddSelectedView("username-branchname-3");
        }

        private void Button_Click_Unmount(object sender, RoutedEventArgs e)
        {
            //mainWindowViewModel.YDriveMounter.Mount(@"C:\Users\Dickson\Desktop\testViews\dickson-branchname-3");
            //mainWindowViewModel.YDriveMounter.Mount("");
            //mainWindowViewModel.NavigationHelper.StopAutoRefresh();

            // TEST ADDING NON USER VIEW
            mainWindowViewModel.NavigationHelper.RemoveSelectedView("aaaaa-tehee");
        }
    }
}
