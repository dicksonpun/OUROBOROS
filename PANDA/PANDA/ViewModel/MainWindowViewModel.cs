using System.Collections.Generic;

namespace PANDA.ViewModel
{
    public class MainWindowViewModel : ViewModel
    {
        public MainWindow mainWindow;

        public SupportedNetworkModeHelper SupportedNetworkModeHelper;
        public NavigationHelper NavigationHelper;

        public MainWindowViewModel(MainWindow currentMainWindow) : base()
        {
            mainWindow                 = currentMainWindow;
            SupportedNetworkModeHelper = new SupportedNetworkModeHelper(mainWindow);
            NavigationHelper           = new NavigationHelper(mainWindow, SupportedNetworkModeHelper);
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