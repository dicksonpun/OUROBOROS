using MaterialDesignExtensions.Model;
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
        SOURCE_CONTROL,
        FEATURE_MODULES,
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
        public SupportedNetworkModeHelper AccessSupportedNetworkModeHelper;
        public UserSettingsHelper AccessUserSettingsHelper;
        public DriveMounter AccessYDriveMounter;

        // Constructors
        public NavigationHelper(MainWindow mainWindow,
                                SupportedNetworkModeHelper supportedNetworkModeHelper,
                                UserSettingsHelper userSettingsHelper,
                                DriveMounter YDriveMounter)
        {
            m_mainWindow                     = mainWindow;
            AccessSupportedNetworkModeHelper = supportedNetworkModeHelper;
            AccessUserSettingsHelper         = userSettingsHelper;
            AccessYDriveMounter              = YDriveMounter;
            navigationClearcaseViews         = new List<ClearcaseManagerViewItem>();
            RequestedAddView                 = string.Empty;
        }

        // ----------------------------------------------------------------------------------------
        // Class       : NavigationHelper
        // Method      : InitializeNavigationDrawerNav
        // Description : Helper function to initialize the navigation menu.
        // ----------------------------------------------------------------------------------------
        public void InitializeNavigationDrawerNav()
        {
            PopulateNavigation();
            SetNavigationSelection(1); // Default selection to index of top menu item
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
                                                                                                NAVIGATION_CATEGORY.SOURCE_CONTROL,
                                                                                                NAVIGATION_CATEGORY.FEATURE_MODULES,
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
            if (m_mainWindow.navigationDrawerNav.SelectedItem != null)
            {
                // De-select previous item
                m_mainWindow.navigationDrawerNav.SelectedItem.IsSelected = false;
            }

            // Select current item
            NavigationItems[index].IsSelected = true;
            m_mainWindow.navigationDrawerNav.SelectedItem = NavigationItems[index];
            m_mainWindow.DataContext = this;
        }

        // ----------------------------------------------------------------------------------------
        // Class       : NavigationHelper
        // Method      : GetNavigationCategory
        // Description : Returns the requested category-related INavigation items in a list.
        //               If the requested category does not exist, throw an exception.
        //               NOTE: ALWAYS INSTANTIATE VIEWMODELS, otherwise they are not created until 
        //                     the function callback occurs which may produce poor responsiveness.
        //               NOTE: ClearcaseView ViewModels are handled dynamically and not in this function.
        // Parameters  :
        // - NavigationCategory (enum NAVIGATION_CATEGORY) : Navigation categories enumeration. 
        // ----------------------------------------------------------------------------------------
        public List<INavigationItem> GetNavigationCategory(NAVIGATION_CATEGORY NavigationCategory)
        {
            switch (NavigationCategory)
            {
                case NAVIGATION_CATEGORY.DASHBOARD:
                    // Instantiate ViewModels
                    GetViewModelFromMap("UserProfileViewModel", AccessUserSettingsHelper);
                    return new List<INavigationItem>()
                    {
                        new SubheaderNavigationItem()  { Subheader = "DASHBOARD" },
                        new FirstLevelNavigationItem() { Label = "User Profile",           Icon = PackIconKind.Account,          NavigationItemSelectedCallback = item => GetViewModelFromMap("UserProfileViewModel", AccessUserSettingsHelper) },
                        new DividerNavigationItem(),
                    };
                case NAVIGATION_CATEGORY.SOURCE_CONTROL:
                    GetViewModelFromMap("ClearcaseViewHelperViewModel", this);
                    return new List<INavigationItem>()
                    {
                        new SubheaderNavigationItem()  { Subheader = "SOURCE CONTROL" },
                        new FirstLevelNavigationItem() { Label = "Clearcase Manager",  Icon = PackIconKind.JackOLantern,     NavigationItemSelectedCallback = item => GetViewModelFromMap("ClearcaseViewHelperViewModel") },
                        new DividerNavigationItem(),
                    };
                case NAVIGATION_CATEGORY.FEATURE_MODULES:
                    // Instantiate ViewModels
                    GetViewModelFromMap("ClearcaseManagerViewModel"); 
                    //GetViewModelFromMap("ClearcaseTabControlViewModel"); 
                    GetViewModelFromMap("ClearcaseTabControlViewModel", "dickson-branchname-1"); 
                    return new List<INavigationItem>()
                    {
                        new SubheaderNavigationItem()  { Subheader = "FEATURE MODULES" },
                        new FirstLevelNavigationItem() { Label = "[Prototype] Clearcase Manager",      Icon = PackIconKind.GithubFace,       NavigationItemSelectedCallback = item => GetViewModelFromMap("ClearcaseManagerViewModel") },
                        new FirstLevelNavigationItem() { Label = "[Prototype] Clearcase TabControl",   Icon = PackIconKind.Git,              NavigationItemSelectedCallback = item => GetViewModelFromMap("ClearcaseTabControlViewModel") },

                        new FirstLevelNavigationItem() { Label = "Placeholder",            Icon = PackIconKind.TestTube,         NavigationItemSelectedCallback = item => "UNDER CONSTRUCTION: PLACEHOLDER" },
                        new DividerNavigationItem(),
                    };
                case NAVIGATION_CATEGORY.DOCUMENTATION:
                    // Instantiate ViewModels
                    GetViewModelFromMap("VersionLogViewModel");
                    GetViewModelFromMap("LicenseLogViewModel");
                    return new List<INavigationItem>()
                    {
                        new SubheaderNavigationItem()  { Subheader = "DOCUMENTATION" },
                        new FirstLevelNavigationItem() { Label = "Version Log",            Icon = PackIconKind.Wunderlist,       NavigationItemSelectedCallback = item => GetViewModelFromMap("VersionLogViewModel") },
                        new FirstLevelNavigationItem() { Label = "Licenses",               Icon = PackIconKind.BarcodeScanner,   NavigationItemSelectedCallback = item => GetViewModelFromMap("LicenseLogViewModel") },
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
                // Update Appbar Title
                m_mainWindow.appBar.Title = ((FirstLevelNavigationItem)navigationItem).Label;
            }
            else
            {
                // If NavigationItem is dynamically removed during runtime, default to user profile.
                SetNavigationSelection(1); // Default selection to index of top menu item
                //m_mainWindow.contentControl.Content = null;
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

