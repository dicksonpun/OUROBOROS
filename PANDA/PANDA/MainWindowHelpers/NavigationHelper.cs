using MaterialDesignExtensions.Controls;
using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;
using PANDA.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;

namespace PANDA
{
    public partial class MainWindow : Window
    {
        //========================//
        // Navigation Menu Helper //
        //========================//
        public const string DialogHostName = "dialogHost";
        private List<INavigationItem> m_navigationItems;
        private Dictionary<string, ViewModel.ViewModel> viewModelMap = new Dictionary<string,ViewModel.ViewModel>();

        private MessageHubHelper m_messageHubHelper;
        public enum NAVIGATION_CATEGORY
        {
            TESTING,
            DASHBOARD,
            VERSION_CONTROL,
            INTEGRATION,
            DOCUMENTATION
        };

        private void InitializeNavigationDrawerNav(MessageHubHelper messageHubHelper)
        {
            m_messageHubHelper = messageHubHelper;
            m_navigationItems = new List<INavigationItem>();
            List<NAVIGATION_CATEGORY> navigationCategoryOrder = new List<NAVIGATION_CATEGORY> { NAVIGATION_CATEGORY.DASHBOARD,
                                                                                                NAVIGATION_CATEGORY.VERSION_CONTROL,
                                                                                                NAVIGATION_CATEGORY.INTEGRATION,
                                                                                                NAVIGATION_CATEGORY.DOCUMENTATION };

#if DEBUG   
            // Override NavigationItem to default to viewModel undergoing testing for debug mode
            navigationCategoryOrder.Insert(0, NAVIGATION_CATEGORY.TESTING);
#endif

            foreach (var supportedNavigationCategory in navigationCategoryOrder)
            {
                foreach (var NavItem in GetNavigationCategory(supportedNavigationCategory))
                {
                    m_navigationItems.Add(NavItem);
                }
            }

            // Default the selected item to index 1
            m_navigationItems[1].IsSelected = true;
            navigationDrawerNav.SelectedItem = m_navigationItems[1];
            navigationDrawerNav.DataContext = this;
        }

        private ViewModel.ViewModel GetViewModelFromMap(string key)
        {
            // If the requested ViewModel instance does not exist, create it and then return it
            if (!viewModelMap.ContainsKey(key))
            {
                // The expected key pattern is <viewModel_Type>.<unique_Identifier> 
                // The .<unique_Identifier> is optional but required if multiple instances of the same viewModel are created
                if (key.Contains("ClearcaseManagerViewModel"))
                {
                    viewModelMap.Add(key, new ClearcaseManagerViewModel(m_messageHubHelper));
                }
                else if (key.Contains("VersionLogViewModel"))
                {
                    viewModelMap.Add(key, new VersionLogViewModel());
                }
                else if (key.Contains("LicenseLogViewModel"))
                {
                    viewModelMap.Add(key, new LicenseLogViewModel());
                }
            }
            return viewModelMap[key]; ;               
        }

        private List<INavigationItem> GetNavigationCategory(NAVIGATION_CATEGORY NavigationCategory)
        {
            switch (NavigationCategory)
            {
                case NAVIGATION_CATEGORY.TESTING:
                    return new List<INavigationItem>()
                    {
                        new SubheaderNavigationItem()  { Subheader = "TESTING" },
                        new FirstLevelNavigationItem() { Label = "Currently Testing",           Icon = PackIconKind.Beaker,           NavigationItemSelectedCallback = item => GetViewModelFromMap("ClearcaseManagerViewModel.TESTING") },
                        new DividerNavigationItem(),
                    };
                case NAVIGATION_CATEGORY.DASHBOARD:
                    return new List<INavigationItem>()
                    {
                        new SubheaderNavigationItem()  { Subheader = "DASHBOARD" },
                        new FirstLevelNavigationItem() { Label = "Task Overview",               Icon = PackIconKind.MonitorDashboard, NavigationItemSelectedCallback = item => "UNDER CONSTRUCTION: TASK OVERVIEW" },
                        new DividerNavigationItem(),
                    };
                case NAVIGATION_CATEGORY.VERSION_CONTROL:
                    return new List<INavigationItem>()
                    {
                        new SubheaderNavigationItem()  { Subheader = "VERSION CONTROL" },
                        new FirstLevelNavigationItem() { Label = "Clearcase View Manager",      Icon = PackIconKind.GithubFace,       NavigationItemSelectedCallback = item => GetViewModelFromMap("ClearcaseManagerViewModel") },
                        new FirstLevelNavigationItem() { Label = "username-branchname-random1", Icon = PackIconKind.Git,              NavigationItemSelectedCallback = item => "UNDER CONSTRUCTION: VIEW BRANCH TEST" },
                        new DividerNavigationItem(),
                    };
                case NAVIGATION_CATEGORY.INTEGRATION:
                    return new List<INavigationItem>()
                    {
                        new SubheaderNavigationItem()  { Subheader = "INTEGRATION" },
                        new FirstLevelNavigationItem() { Label = "Test Descriptor Manager",     Icon = PackIconKind.TestTube,         NavigationItemSelectedCallback = item => "UNDER CONSTRUCTION: TEST DESCRIPTOR MANAGER" },
                        new FirstLevelNavigationItem() { Label = "Queue ETA",                   Icon = PackIconKind.Timetable,        NavigationItemSelectedCallback = item => "UNDER CONSTRUCTION: QUEUE ETA" },
                        new FirstLevelNavigationItem() { Label = "Artifacts Analyzer",          Icon = PackIconKind.TableSearch,      NavigationItemSelectedCallback = item => "UNDER CONSTRUCTION: ARTIFACTS ANALYZER" },
                        new DividerNavigationItem(),
                    };
                case NAVIGATION_CATEGORY.DOCUMENTATION:
                    return new List<INavigationItem>()
                    {
                        new SubheaderNavigationItem()  { Subheader = "DOCUMENTATION" },
                        new FirstLevelNavigationItem() { Label = "Version Log",                 Icon = PackIconKind.Wunderlist,       NavigationItemSelectedCallback = item => GetViewModelFromMap("VersionLogViewModel") },
                        new FirstLevelNavigationItem() { Label = "Licenses",                    Icon = PackIconKind.BarcodeScanner,   NavigationItemSelectedCallback = item => GetViewModelFromMap("LicenseLogViewModel") },
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void NavigationItemSelectedHandler(object sender, NavigationItemSelectedEventArgs args)
        {
            SelectNavigationItem(args.NavigationItem);
        }

        private void SelectNavigationItem(INavigationItem navigationItem)
        {
            if (navigationItem != null)
            {
                contentControl.Content = navigationItem.NavigationItemSelectedCallback(navigationItem);
            }
            else
            {
                contentControl.Content = null;
            }

            if (appBar != null)
            {
                appBar.IsNavigationDrawerOpen = false;
            }
        }

        // REQUIRED FOR DATABINDING
        public List<INavigationItem> NavigationItems
        {
            get { return m_navigationItems; }
        }
    }
}
