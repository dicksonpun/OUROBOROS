using System.Collections.Generic;

namespace PANDA.ViewModel
{
    public class MainWindowViewModel : ViewModel
    {
        // Members
        public MainWindow mainWindow;
        public SupportedNetworkModeHelper SupportedNetworkModeHelper;
        public NavigationHelper NavigationHelper;
        public DriveMounter YDriveMounter;

        // Constructor
        public MainWindowViewModel(MainWindow currentMainWindow) : base()
        {
            mainWindow                 = currentMainWindow;
            YDriveMounter              = new DriveMounter("Y:");
            SupportedNetworkModeHelper = new SupportedNetworkModeHelper(mainWindow);
            NavigationHelper           = new NavigationHelper(mainWindow, SupportedNetworkModeHelper, YDriveMounter);
        }

        public void Initialize()
        {
            // SupportedNetworkModeHelper
            SupportedNetworkModeHelper.Determine_Network_Mode();
            SupportedNetworkModeHelper.Update_Title_Based_On_Network();

            // NavigationHelper
            NavigationHelper.InitializeNavigationDrawerNav();
        }
    }
}