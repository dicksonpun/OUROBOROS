using System;
using System.Collections.Generic;

namespace PANDA
{
    public enum SUPPORTED_NETWORK_MODES
    {
        DEBUG      = 0, // FIRST INDEX RESERVED FOR DEBUG 
        SERVER_001 = 1,
        SERVER_002 = 2,
        OFFLINE    = 3  // LAST INDEX RESERVED FOR OFFLINE 
    }

    public class NetworkMode
    {
        public SUPPORTED_NETWORK_MODES Name { get; set; }
        public string NetworkSpecificPath { get; set; } // Add @"filePath" on assignment to avoid escape sequence
        public NetworkMode() { }
    }

    public class SupportedNetworkModeHelper
    {
        // Class members
        public NetworkMode CurrentNetworkMode   { get; private set; }
        public List<NetworkMode> SupportedNetworkModes { get; private set; }
        private MainWindow m_mainWindow;

        // Constructor
        public SupportedNetworkModeHelper(MainWindow mainWindow)
        {
            m_mainWindow = mainWindow;

            SupportedNetworkModes = new List<NetworkMode>()
            {
                new NetworkMode  {  Name = SUPPORTED_NETWORK_MODES.DEBUG,      NetworkSpecificPath = @"C:\Users\Dickson\Desktop\testViews"},
                new NetworkMode  {  Name = SUPPORTED_NETWORK_MODES.SERVER_001, NetworkSpecificPath = "ABC1"},
                new NetworkMode  {  Name = SUPPORTED_NETWORK_MODES.SERVER_002, NetworkSpecificPath = "ABC2"},
                new NetworkMode  {  Name = SUPPORTED_NETWORK_MODES.OFFLINE,    NetworkSpecificPath = "" },
            };

            Determine_Network_Mode();
            Update_Title_Based_On_Network();
        }

        // Helper functions
        public void Determine_Network_Mode()
        {
            foreach (var SupportedNetworkMode in SupportedNetworkModes)
            {
                // Found network-specific path or reached end of the list.
                if (System.IO.Directory.Exists(SupportedNetworkMode.NetworkSpecificPath) ||
                    SupportedNetworkMode.Name.Equals(SUPPORTED_NETWORK_MODES.OFFLINE))
                {
                    CurrentNetworkMode = SupportedNetworkMode;
                    break;
                }
            }
#if DEBUG
            // Override NetworkMode after For-Loop for debug mode
            CurrentNetworkMode = SupportedNetworkModes[Convert.ToInt32(SUPPORTED_NETWORK_MODES.DEBUG)];
#endif
        }

        public void Update_Title_Based_On_Network()
        {
            m_mainWindow.Title = "[ " + CurrentNetworkMode.Name + " ]" + "   Partially Automated Network Dependent Assistant";
        }
    }
}
