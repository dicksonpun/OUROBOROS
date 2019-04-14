using System.Windows;

namespace PANDA
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SupportedNetworkModeHelper supportedNetworkModeHelper = InitializeSupportedNetworkModeHelper();
            MessageHubHelper messageHubHelper                     = InitializeMessageHubHelper(supportedNetworkModeHelper);
            InitializeNavigationDrawerNav(messageHubHelper); // make a helper class
        }

        // Main Window Helpers
        public SupportedNetworkModeHelper InitializeSupportedNetworkModeHelper()
        {
            SupportedNetworkModeHelper supportedNetworkModeHelper = new SupportedNetworkModeHelper(this);
            supportedNetworkModeHelper.Determine_Network_Mode();
            supportedNetworkModeHelper.Update_Title_Based_On_Network();
            return supportedNetworkModeHelper;
        }

        public MessageHubHelper InitializeMessageHubHelper(SupportedNetworkModeHelper supportedNetworkModeHelper)
        {
            MessageHubHelper messageHubHelper = new MessageHubHelper(supportedNetworkModeHelper);
            messageHubHelper.InitializeMessageHub();
            return messageHubHelper;
        }

    }
}
