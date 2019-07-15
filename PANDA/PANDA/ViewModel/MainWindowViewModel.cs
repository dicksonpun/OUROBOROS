using MaterialDesignThemes.Wpf;
using PANDA.Command;
using PANDA.Controls;

namespace PANDA.ViewModel
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

            UserSettingsHelper.UserProfile.ColorSettings.SetTheme();
            UserSettingsHelper.UserProfile.ColorSettings.SetDarkMode();
        }
    }
}