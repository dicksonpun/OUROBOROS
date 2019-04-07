using System.Collections.Generic;
using System.Windows;

namespace PANDA
{
    public partial class MainWindow : Window
    {
        //===========================================//
        // Application Mode Determination Processing //
        //===========================================//
        public ApplicationMode CurrentApplicationMode { get; private set; }

        private void UpdateTitleBasedOnApplicationMode()
        {
            this.Title = "[ " + CurrentApplicationMode.Name + " ]";
        }

        private void DetermineApplicationMode()
        {
            foreach (var SupportedApplicationMode in SupportedApplicationModes)
            {
                // Found network-specific path or reached end of the list.
                if (System.IO.Directory.Exists(SupportedApplicationMode.NetworkSpecificPath) ||
                    SupportedApplicationMode.Name.Equals(APPLICATION_MODES.OFFLINE))
                {
                    CurrentApplicationMode = SupportedApplicationMode;
                    break;
                }
            }
            // Override ApplicationMode after For-Loop for debug mode
            #if DEBUG
                CurrentApplicationMode = SupportedApplicationModes[0];
            #endif
        }

        public enum APPLICATION_MODES
        {
            DEBUG      = 0, // FIRST INDEX RESERVED FOR DEBUG 
            SERVER_001 = 1,
            SERVER_002 = 2,
            OFFLINE    = 3  // LAST INDEX RESERVED FOR OFFLINE 
        };

        public class ApplicationMode
        {
            public APPLICATION_MODES Name { get; set; }
            public string NetworkSpecificPath { get; set; } // Add @"filePath" on assignment to avoid escape sequence
            public List<NAVIGATION_CATEGORY> NavigationCategoryOrder { get; set; } // Sequential Order
            public ApplicationMode() { }
        }

        List<ApplicationMode> SupportedApplicationModes = new List<ApplicationMode>()
        {
            new ApplicationMode  {  Name = APPLICATION_MODES.DEBUG,
                                    NetworkSpecificPath = @"C:\Users\Dickson\Desktop\testViews",
                                    NavigationCategoryOrder = new List<NAVIGATION_CATEGORY> {   NAVIGATION_CATEGORY.TESTING ,
                                                                                                NAVIGATION_CATEGORY.DASHBOARD,
                                                                                                NAVIGATION_CATEGORY.VERSION_CONTROL,
                                                                                                NAVIGATION_CATEGORY.INTEGRATION,
                                                                                                NAVIGATION_CATEGORY.DOCUMENTATION } },
            new ApplicationMode  {  Name = APPLICATION_MODES.SERVER_001,
                                    NetworkSpecificPath = "ABC1",
                                    NavigationCategoryOrder = new List<NAVIGATION_CATEGORY>{    NAVIGATION_CATEGORY.DASHBOARD,
                                                                                                NAVIGATION_CATEGORY.VERSION_CONTROL,
                                                                                                NAVIGATION_CATEGORY.DOCUMENTATION } },
            new ApplicationMode  {  Name = APPLICATION_MODES.SERVER_002,
                                    NetworkSpecificPath = "ABC2",
                                    NavigationCategoryOrder = new List<NAVIGATION_CATEGORY>{    NAVIGATION_CATEGORY.DASHBOARD,
                                                                                                NAVIGATION_CATEGORY.VERSION_CONTROL,
                                                                                                NAVIGATION_CATEGORY.INTEGRATION,
                                                                                                NAVIGATION_CATEGORY.DOCUMENTATION } },
            new ApplicationMode  {  Name = APPLICATION_MODES.OFFLINE,
                                    NetworkSpecificPath = "",
                                    NavigationCategoryOrder = new List<NAVIGATION_CATEGORY>{    NAVIGATION_CATEGORY.DASHBOARD,
                                                                                                NAVIGATION_CATEGORY.VERSION_CONTROL,
                                                                                                NAVIGATION_CATEGORY.DOCUMENTATION } },
        };
    }
}
