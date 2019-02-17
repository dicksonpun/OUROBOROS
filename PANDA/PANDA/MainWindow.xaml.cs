using System;
using System.Windows;

namespace PANDA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadApplicationPalette();
            LoadUserConfigurations();
        }

        void LoadApplicationPalette()
        {
            PaletteSelectorViewModel.ApplyDarkBase();
            PaletteSelectorViewModel.LoadPrimarySwatch();
        }

        void LoadUserConfigurations()
        {

        }

    }
}
