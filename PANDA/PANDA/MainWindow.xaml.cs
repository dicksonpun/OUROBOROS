using MaterialDesignExtensions.Controls;
using MaterialDesignExtensions.Model;
using System;
using System.Collections.Generic;
using System.Windows;

namespace PANDA
{
    public partial class MainWindow : Window
    {
        public PANDA.ViewModel.MainWindowViewModel mainWindowViewModel;

        public MainWindow()
        {
            InitializeComponent();
            mainWindowViewModel = new ViewModel.MainWindowViewModel(this);
            mainWindowViewModel.Initialize();

        }

        public void NavigationItemSelectedHandler(object sender, NavigationItemSelectedEventArgs args)
        {
            mainWindowViewModel.navigationHelper.SelectNavigationItem(args.NavigationItem);
        }
    }
}
