using System.Windows;

namespace PANDA
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DetermineApplicationMode();
            UpdateTitleBasedOnApplicationMode();
            InitializeNavigationDrawerNav();     // Note: Dependency on ApplicationMode data
            InitializeMessageHub();              // Note: Dependency on ApplicationMode data
        }
    }
}
