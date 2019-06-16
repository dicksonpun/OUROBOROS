﻿using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;
using PANDA.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PANDA
{
    public enum NAVIGATION_CATEGORY
    {
        DASHBOARD,
        VERSION_CONTROL,
        INTEGRATION,
        DOCUMENTATION
    };

    public partial class NavigationHelper
    {
        // Members
        private readonly MainWindow m_mainWindow;
        public List<ClearcaseManagerViewItem> navigationClearcaseViews;
        public string RequestedAddView;
        public string RequestedRemoveView;

        // Helpers
        private readonly SupportedNetworkModeHelper m_supportedNetworkModeHelper;
        private DriveMounter m_YDriveMounter;

        // Constructors
        public NavigationHelper(MainWindow currentMainWindow, 
                                SupportedNetworkModeHelper supportedNetworkModeHelper,
                                DriveMounter YDriveMounter)
        {
            m_mainWindow                 = currentMainWindow;
            m_supportedNetworkModeHelper = supportedNetworkModeHelper;
            m_YDriveMounter              = YDriveMounter;
            navigationClearcaseViews     = new List<ClearcaseManagerViewItem>();
            RequestedAddView             = string.Empty;
        }

        // ----------------------------------------------------------------------------------------
        // Class       : NavigationHelper
        // Method      : InitializeNavigationDrawerNav
        // Description : Helper function to initialize the navigation menu.
        // ----------------------------------------------------------------------------------------
        public void InitializeNavigationDrawerNav()
        {
            PopulateNavigation();
            SetNavigationSelection(1);
            StartAutoRefresh();
        }

        // ----------------------------------------------------------------------------------------
        // Class       : NavigationHelper
        // Method      : PopulateNavigation
        // Description : Populates the navigation menu based on a provided category order.
        // ----------------------------------------------------------------------------------------
        public void PopulateNavigation()
        {
            List<NAVIGATION_CATEGORY> navigationCategoryOrder = new List<NAVIGATION_CATEGORY> { NAVIGATION_CATEGORY.DASHBOARD,
                                                                                                NAVIGATION_CATEGORY.VERSION_CONTROL,
                                                                                                NAVIGATION_CATEGORY.INTEGRATION,
                                                                                                NAVIGATION_CATEGORY.DOCUMENTATION };
            // Populate navigation menu
            NavigationItems = new ObservableCollection<INavigationItem>();
            foreach (var supportedNavigationCategory in navigationCategoryOrder)
            {
                foreach (var NavItem in GetNavigationCategory(supportedNavigationCategory))
                {
                    NavigationItems.Add(NavItem);
                }
            }
        }

        // ----------------------------------------------------------------------------------------
        // Class       : NavigationHelper
        // Method      : SetNavigationSelection
        // Description : Sets the index of the navigation menu and DataContext of the mainwindow accordingly.
        // Parameters  :
        // - index (int) : Index of selected item. 
        // ----------------------------------------------------------------------------------------
        public void SetNavigationSelection(int index)
        {
            if (m_mainWindow.sideNav.SelectedItem != null)
            {
                // De-select previous item
                m_mainWindow.sideNav.SelectedItem.IsSelected = false;
            }

            // Select current item
            NavigationItems[index].IsSelected = true;
            m_mainWindow.sideNav.SelectedItem = NavigationItems[index];
            m_mainWindow.DataContext = this;
        }

        // ----------------------------------------------------------------------------------------
        // Class       : NavigationHelper
        // Method      : GetNavigationCategory
        // Description : Returns the requested category-related INavigation items in a list.
        //               If the requested category does not exist, throw an exception.
        // Parameters  :
        // - NavigationCategory (enum NAVIGATION_CATEGORY) : Navigation categories enumeration. 
        // ----------------------------------------------------------------------------------------
        public List<INavigationItem> GetNavigationCategory(NAVIGATION_CATEGORY NavigationCategory)
        {
            switch (NavigationCategory)
            {
                case NAVIGATION_CATEGORY.DASHBOARD:
                    return new List<INavigationItem>()
                    {
                        new SubheaderNavigationItem()  { Subheader = "DASHBOARD" },
                        new FirstLevelNavigationItem() { Label = "Task Overview",      Icon = PackIconKind.MonitorDashboard, NavigationItemSelectedCallback = item => "UNDER CONSTRUCTION: TASK OVERVIEW" },
                        new DividerNavigationItem(),
                    };
                case NAVIGATION_CATEGORY.VERSION_CONTROL:
                    // Instantiate ViewModels
                    GetViewModelFromMap("ClearcaseManagerViewModel");
                    return new List<INavigationItem>()
                    {
                        new SubheaderNavigationItem()  { Subheader = "VERSION CONTROL" },
                        new FirstLevelNavigationItem() { Label = "Clearcase Manager",  Icon = PackIconKind.GithubFace,       NavigationItemSelectedCallback = item => GetViewModelFromMap("ClearcaseManagerViewModel") },
                        new DividerNavigationItem(),
                    };
                case NAVIGATION_CATEGORY.INTEGRATION:
                    return new List<INavigationItem>()
                    {
                        new SubheaderNavigationItem()  { Subheader = "INTEGRATION" },
                        new FirstLevelNavigationItem() { Label = "Test Builder",       Icon = PackIconKind.TestTube,         NavigationItemSelectedCallback = item => "UNDER CONSTRUCTION: TEST BUILDER" },
                        new FirstLevelNavigationItem() { Label = "Queue ETA",          Icon = PackIconKind.Clock,            NavigationItemSelectedCallback = item => "UNDER CONSTRUCTION: QUEUE ETA" },
                        new FirstLevelNavigationItem() { Label = "Artifacts Analyzer", Icon = PackIconKind.Microscope,       NavigationItemSelectedCallback = item => "UNDER CONSTRUCTION: ARTIFACTS ANALYZER" },
                        new DividerNavigationItem(),
                    };
                case NAVIGATION_CATEGORY.DOCUMENTATION:
                    // Instantiate ViewModels
                    GetViewModelFromMap("VersionLogViewModel");
                    GetViewModelFromMap("LicenseLogViewModel");
                    return new List<INavigationItem>()
                    {
                        new SubheaderNavigationItem()  { Subheader = "DOCUMENTATION" },
                        new FirstLevelNavigationItem() { Label = "Version Log",        Icon = PackIconKind.Wunderlist,       NavigationItemSelectedCallback = item => GetViewModelFromMap("VersionLogViewModel") },
                        new FirstLevelNavigationItem() { Label = "Licenses",           Icon = PackIconKind.BarcodeScanner,   NavigationItemSelectedCallback = item => GetViewModelFromMap("LicenseLogViewModel") },
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // ---------------------- //
        //    FOR DATA BINDING    //
        // ---------------------- //
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

        private ObservableCollection<INavigationItem> m_navigationItems;
        public ObservableCollection<INavigationItem> NavigationItems
        {
            get { return m_navigationItems; }
            set { m_navigationItems = value; }
        }

        private ObservableCollection<string> m_availableViews;
        public ObservableCollection<string> AvailableViews
        {
            get { return m_availableViews; }
            set { m_availableViews = value; }
        }
    }
}

