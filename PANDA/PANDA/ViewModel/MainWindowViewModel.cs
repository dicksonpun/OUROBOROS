using System.Collections.Generic;

namespace PANDA.ViewModel
{
    public class MainWindowViewModel : ViewModel
    {
        public MainWindow mainWindow;
        public SupportedNetworkModeHelper supportedNetworkModeHelper;
        public MessageHubHelper messageHubHelper;
        public NavigationHelper navigationHelper;


        public MainWindowViewModel(MainWindow currentMainWindow) : base()
        {
            mainWindow                 = currentMainWindow;
            supportedNetworkModeHelper = new SupportedNetworkModeHelper(mainWindow);
            messageHubHelper           = new MessageHubHelper(supportedNetworkModeHelper);
            navigationHelper           = new NavigationHelper(mainWindow, messageHubHelper);
        }

        public void Initialize()
        {
            // SupportedNetworkModeHelper
            supportedNetworkModeHelper.Determine_Network_Mode();
            supportedNetworkModeHelper.Update_Title_Based_On_Network();

            // MessageHubHelper
            //messageHubHelper.InitializeMessageHub();

            // NavigationHelper
            navigationHelper.InitializeNavigationDrawerNav();
        }
    }
}