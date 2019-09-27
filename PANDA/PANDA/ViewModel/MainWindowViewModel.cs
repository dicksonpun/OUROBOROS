using MaterialDesignThemes.Wpf;
using OUROBOROS.Command;
using OUROBOROS.Controls;

namespace OUROBOROS.ViewModel
{
    public class MainWindowViewModel : ViewModel
    {
        // Members
        public MainWindow MainWindow;
        public UserSettingsHelper UserSettingsHelper;
        public SupportedNetworkModeHelper SupportedNetworkModeHelper;
        public NavigationHelper NavigationHelper;
        public DriveMounter YDriveMounter;

        // Constructor
        public MainWindowViewModel(MainWindow mainWindow) : base()
        {
            MainWindow                 = mainWindow;
            UserSettingsHelper         = new UserSettingsHelper(mainWindow);
            YDriveMounter              = new DriveMounter("Y:");
            SupportedNetworkModeHelper = new SupportedNetworkModeHelper(mainWindow);
            NavigationHelper           = new NavigationHelper(mainWindow, 
                                                              SupportedNetworkModeHelper, 
                                                              UserSettingsHelper, 
                                                              YDriveMounter);
        }

        public void Initialize()
        {
            NavigationHelper.InitializeNavigationDrawerNav();

        }
    }
}