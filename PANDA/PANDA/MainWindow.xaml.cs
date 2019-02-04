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
            LoadUserConfigurations();
        }

        void LoadUserConfigurations()
        {
            PaletteSelectorViewModel.LoadPalette();
        }
    }
}
