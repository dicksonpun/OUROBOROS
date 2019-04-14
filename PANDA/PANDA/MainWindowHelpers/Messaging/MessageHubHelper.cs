using System;
using System.Windows;
using TinyMessenger;

namespace PANDA
{
    public partial class MessageHubHelper
    {
        public TinyMessengerHub MessageHub { get; private set; }
        public SupportedNetworkModeHelper m_supportedNetworkModeHelper;

        public MessageHubHelper(SupportedNetworkModeHelper supportedNetworkModeHelper)
        {
            MessageHub = new TinyMessengerHub();
            m_supportedNetworkModeHelper = supportedNetworkModeHelper;
        }

        public async void InitializeMessageHub()
        {
            // Subscribe and Publish to Messages based on network
            if (m_supportedNetworkModeHelper.CurrentNetworkMode.Name.Equals(SUPPORTED_NETWORK_MODES.DEBUG) ||
                m_supportedNetworkModeHelper.CurrentNetworkMode.Name.Equals(SUPPORTED_NETWORK_MODES.SERVER_001) ||
                m_supportedNetworkModeHelper.CurrentNetworkMode.Name.Equals(SUPPORTED_NETWORK_MODES.SERVER_002))
            {
                // Subscribe to Messages
                MessageHub.Subscribe<ClearcaseManagerMessage>((message) => { ProcessClearcaseManagerMessage(message); }); // temp callback

                // Publish Periodic Messages Asynchronously
                // Disable for now
                //await MainWindowToClearcaseManagerMessageUpdate(TimeSpan.FromSeconds(1.0));
            }
        }

        public void ProcessClearcaseManagerMessage(ClearcaseManagerMessage message)
        {
            if (message.Content.MessageCommand.Equals(ClearcaseManagerMessageCommand.REQUEST_VIEW_ADD))
            {
                Console.WriteLine("RECEIVED CLEARCASE MESSAGE COMMAND: " + message.Content.MessageCommand.ToString());
                Console.WriteLine("RECEIVED CLEARCASE MESSAGE CONTENT: " + message.Content.ClearcaseManagerViewItem.DirectoryName);
            }
        }
    }
}
