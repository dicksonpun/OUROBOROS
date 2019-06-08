using MaterialDesignExtensions.Controls;
using MaterialDesignExtensions.Model;
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
        }

        public void NavigationItemSelectedHandler(object sender, NavigationItemSelectedEventArgs args)
        {
            mainWindowViewModel.NavigationHelper.SelectNavigationItem(args.NavigationItem);
        }
    }
}
