using MaterialDesignExtensions.Controls;
using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;
using PANDA.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;


using System.Diagnostics;
using System.Threading;

namespace PANDA
{
    public enum NAVIGATION_CATEGORY
    {
        DASHBOARD,
        VERSION_CONTROL,
        INTEGRATION,
        DOCUMENTATION
    };

    public class NavigationHelper
    {
        public const string DialogHostName = "dialogHost";
        private MessageHubHelper m_messageHubHelper;
        private SupportedNetworkModeHelper m_supportedNetworkModeHelper;
        private MainWindow m_mainWindow;

        public NavigationHelper(MainWindow currentMainWindow, MessageHubHelper messageHubHelper)
        {
            m_mainWindow       = currentMainWindow;
            m_messageHubHelper = messageHubHelper;
        }

        public void InitializeNavigationDrawerNav()
        {
            m_supportedNetworkModeHelper = m_messageHubHelper.SupportedNetworkModeHelper;

            NavigationItems = new List<INavigationItem>();
            List<NAVIGATION_CATEGORY> navigationCategoryOrder = new List<NAVIGATION_CATEGORY> { NAVIGATION_CATEGORY.DASHBOARD,
                                                                                                NAVIGATION_CATEGORY.VERSION_CONTROL,
                                                                                                NAVIGATION_CATEGORY.INTEGRATION,
                                                                                                NAVIGATION_CATEGORY.DOCUMENTATION };

            foreach (var supportedNavigationCategory in navigationCategoryOrder)
            {
                foreach (var NavItem in GetNavigationCategory(supportedNavigationCategory))
                {
                    NavigationItems.Add(NavItem);
                }
            }

            SetNavigationSelection(1);

            // Subscribe to updates for navigation items (ACTIVATED VIEWS)
            InitializeMessageHub();
        }

        public void SetNavigationSelection(int index)
        {
            // NOTE: This function was broken out of the Navigation initialization function to ensure object is created and in scope first
            NavigationItems[index].IsSelected = true;
            m_mainWindow.sideNav.SelectedItem = NavigationItems[index];
            m_mainWindow.DataContext = this;
        }

        private void InitializeMessageHub()
        {
            // Subscribe and Publish to Messages based on network
            if (m_supportedNetworkModeHelper.CurrentNetworkMode.Name.Equals(SUPPORTED_NETWORK_MODES.DEBUG) ||
                m_supportedNetworkModeHelper.CurrentNetworkMode.Name.Equals(SUPPORTED_NETWORK_MODES.SERVER_001) ||
                m_supportedNetworkModeHelper.CurrentNetworkMode.Name.Equals(SUPPORTED_NETWORK_MODES.SERVER_002))
            {
                // Subscribe to Messages
                m_messageHubHelper.MessageHub.Subscribe<ClearcaseManagerMessage>((message) => { ProcessClearcaseManagerMessage(message); }); 
            }
        }

        private void ProcessClearcaseManagerMessage(ClearcaseManagerMessage message)
        {
            List<string> currentActiveClearcaseViews  = GetCurrentNavigationClearcaseViews();
            List<string> reportedActiveClearcaseViews = m_messageHubHelper.GetListOfViews(message.Content.ClearcaseManagerViewItemsList);

            // Only parse and update if something changed.
            if (!currentActiveClearcaseViews.SequenceEqual(reportedActiveClearcaseViews))
            {
                IEnumerable<string> itemsToRemove = currentActiveClearcaseViews.Except(reportedActiveClearcaseViews);
                IEnumerable<string> itemsToAdd    = reportedActiveClearcaseViews.Except(currentActiveClearcaseViews);

                foreach (ClearcaseManagerViewItem clearcaseItem in message.Content.ClearcaseManagerViewItemsList)
                {
                    Console.WriteLine("RECEIVED CLEARCASE MESSAGE: " + " VIEW: " + clearcaseItem.ViewName);

                    if (itemsToRemove.Contains(clearcaseItem.ViewName))
                    {
                    }

                    if (itemsToAdd.Contains(clearcaseItem.ViewName))
                    {
                        // WHY WONT YOU DISPLAY???

                        // Add to viewModelMap
                        //GetViewModelFromMap("ClearcaseViewViewModel." + clearcaseItem.ViewName);

                        //INavigationItem navigationItem = new FirstLevelNavigationItem() { Label = clearcaseItem.ViewName, Icon = PackIconKind.Git, NavigationItemSelectedCallback = item => GetViewModelFromMap("ClearcaseViewViewModel." + clearcaseItem.ViewName) };
                        //m_navigationItems.Add(navigationItem);

                        //*
                        foreach (INavigationItem navItem in NavigationItems)
                        {
                            if (navItem.GetType().IsEquivalentTo(new FirstLevelNavigationItem().GetType()))
                            {
                                FirstLevelNavigationItem temp = (FirstLevelNavigationItem)navItem;
                                if (temp.Label.Equals("Clearcase View Manager"))
                                {
                                    // Add to viewModelMap
                                    GetViewModelFromMap("ClearcaseViewViewModel." + clearcaseItem.ViewName);

                                    // Add to navigation
                                    int index = NavigationItems.IndexOf(temp) + 1;
                                    INavigationItem navigationItem = new FirstLevelNavigationItem() { Label = clearcaseItem.ViewName, Icon = PackIconKind.Git, NavigationItemSelectedCallback = item => GetViewModelFromMap("ClearcaseViewViewModel." + clearcaseItem.ViewName) };
                                    NavigationItems.Insert(index, navigationItem);
                                    break;
                                }
                            }
                        }
                        //*/
                    }
                }
            }
        }

        private Dictionary<string, ViewModel.ViewModel> viewModelMap = new Dictionary<string, ViewModel.ViewModel>();

        public ViewModel.ViewModel GetViewModelFromMap(string key)
        {
            // If the requested ViewModel instance does not exist, create it and then return it
            if (!viewModelMap.ContainsKey(key))
            {
                // The expected key pattern is <viewModel_Type>.<unique_Identifier> 
                // The .<unique_Identifier> is optional but required if multiple instances of the same viewModel are created
                if (key.StartsWith("ClearcaseManagerViewModel"))
                {
                    viewModelMap.Add(key, new ClearcaseManagerViewModel(m_messageHubHelper));
                }
                if (key.StartsWith("ClearcaseViewViewModel"))
                {
                    viewModelMap.Add(key, new ViewModel.ViewModel()); // temp
                }
                else if (key.StartsWith("VersionLogViewModel"))
                {
                    viewModelMap.Add(key, new VersionLogViewModel());
                }
                else if (key.StartsWith("LicenseLogViewModel"))
                {
                    viewModelMap.Add(key, new LicenseLogViewModel());
                }
            }
            return viewModelMap[key]; ;
        }

        public List<string> GetCurrentNavigationClearcaseViews()
        {
            List<string> result = new List<string>();

            foreach (KeyValuePair<string, ViewModel.ViewModel> entry in viewModelMap)
            {
                if (entry.Key.StartsWith("ClearcaseViewViewModel."))
                    result.Add(entry.Key.Substring(entry.Key.LastIndexOf('.') + 1));
            }

            return result;
        }      

        public void AddToViewModelMap(List<string> listToAdd)
        {
            foreach (var itemToAdd in listToAdd)
                GetViewModelFromMap(itemToAdd);
        }

        public List<INavigationItem> GetNavigationCategory(NAVIGATION_CATEGORY NavigationCategory)
        {
            switch (NavigationCategory)
            {
                case NAVIGATION_CATEGORY.DASHBOARD:
                    return new List<INavigationItem>()
                    {
                        new SubheaderNavigationItem()  { Subheader = "DASHBOARD" },
                        new FirstLevelNavigationItem() { Label = "Task Overview",          Icon = PackIconKind.MonitorDashboard,    NavigationItemSelectedCallback = item => "UNDER CONSTRUCTION: TASK OVERVIEW" },
                        new DividerNavigationItem(),
                    };
                case NAVIGATION_CATEGORY.VERSION_CONTROL:
                    AddToViewModelMap(new List<string> { "ClearcaseManagerViewModel" });
                    return new List<INavigationItem>()
                    {
                        new SubheaderNavigationItem()  { Subheader = "VERSION CONTROL" },
                        new FirstLevelNavigationItem() { Label = "Clearcase View Manager", Icon = PackIconKind.GithubFace,          NavigationItemSelectedCallback = item => GetViewModelFromMap("ClearcaseManagerViewModel") },
                        new DividerNavigationItem(),
                    };
                case NAVIGATION_CATEGORY.INTEGRATION:
                    return new List<INavigationItem>()
                    {
                        new SubheaderNavigationItem()  { Subheader = "INTEGRATION" },
                        new FirstLevelNavigationItem() { Label = "Test Builder",           Icon = PackIconKind.TestTube,            NavigationItemSelectedCallback = item => "UNDER CONSTRUCTION: TEST BUILDER" },
                        new FirstLevelNavigationItem() { Label = "Queue ETA",              Icon = PackIconKind.FolderClockOutline,  NavigationItemSelectedCallback = item => "UNDER CONSTRUCTION: QUEUE ETA" },
                        new FirstLevelNavigationItem() { Label = "Artifacts Analyzer",     Icon = PackIconKind.FolderSearchOutline, NavigationItemSelectedCallback = item => "UNDER CONSTRUCTION: ARTIFACTS ANALYZER" },
                        new DividerNavigationItem(),
                    };
                case NAVIGATION_CATEGORY.DOCUMENTATION:
                    AddToViewModelMap(new List<string> { "VersionLogViewModel",
                                                         "LicenseLogViewModel" });
                    return new List<INavigationItem>()
                    {
                        new SubheaderNavigationItem()  { Subheader = "DOCUMENTATION" },
                        new FirstLevelNavigationItem() { Label = "Version Log",            Icon = PackIconKind.Wunderlist,          NavigationItemSelectedCallback = item => GetViewModelFromMap("VersionLogViewModel") },
                        new FirstLevelNavigationItem() { Label = "Licenses",               Icon = PackIconKind.BarcodeScanner,      NavigationItemSelectedCallback = item => GetViewModelFromMap("LicenseLogViewModel") },
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // DATA BINDING REQUIRED
        public void SelectNavigationItem(INavigationItem navigationItem)
        {
            if (navigationItem != null)
            {
                m_mainWindow.contentControl.Content = navigationItem.NavigationItemSelectedCallback(navigationItem);
            }
            else
            {
                m_mainWindow.contentControl.Content = null;
            }
        }

        private List<INavigationItem> m_navigationItems;

        public List<INavigationItem> NavigationItems
        {
            get { return m_navigationItems; }
            set { m_navigationItems = value; }
        }
    }
}

