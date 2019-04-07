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
        //==========================================//
        // Navigation Menu Determination Processing //
        //==========================================//
        public const string DialogHostName = "dialogHost";
        private List<INavigationItem> m_navigationItems;
        public List<INavigationItem> NavigationItems
        {
            set { this.m_navigationItems = value; } // In TwoWay Binding, there must be a setter defined. Need to test if this works!!!
            get { return m_navigationItems; }
        }

        public enum NAVIGATION_CATEGORY
        {
            TESTING         = 0,
            DASHBOARD       = 1,
            VERSION_CONTROL = 2,
            INTEGRATION     = 3,
            DOCUMENTATION   = 4
        };

        private void InitializeNavigationDrawerNav()
        {
            m_navigationItems = new List<INavigationItem>();

            foreach (var supportedNavigationCategory in CurrentApplicationMode.NavigationCategoryOrder)
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

        private List<INavigationItem> GetNavigationCategory(NAVIGATION_CATEGORY NavigationCategory)
        {
            switch (NavigationCategory)
            {
                case NAVIGATION_CATEGORY.TESTING:
                    return new List<INavigationItem>()
                    {
                        new SubheaderNavigationItem()  { Subheader = "TESTING" },
                        new FirstLevelNavigationItem() { Label = "Currently Testing",           Icon = PackIconKind.Beaker,           NavigationItemSelectedCallback = item => new ClearcaseManagerViewModel() },
                        new DividerNavigationItem(),
                    };
                case NAVIGATION_CATEGORY.DASHBOARD:
                    return new List<INavigationItem>()
                    {
                        new SubheaderNavigationItem()  { Subheader = "DASHBOARD" },
                        new FirstLevelNavigationItem() { Label = "Task Overview",               Icon = PackIconKind.MonitorDashboard, NavigationItemSelectedCallback = item => new ClearcaseManagerViewModel() },
                        new DividerNavigationItem(),
                    };
                case NAVIGATION_CATEGORY.VERSION_CONTROL:
                    return new List<INavigationItem>()
                    {
                        new SubheaderNavigationItem()  { Subheader = "VERSION CONTROL" },
                        new FirstLevelNavigationItem() { Label = "View Manager",                Icon = PackIconKind.GithubFace,       NavigationItemSelectedCallback = item => new ClearcaseManagerViewModel() },
                        new FirstLevelNavigationItem() { Label = "username-branchname-random1", Icon = PackIconKind.Git,              NavigationItemSelectedCallback = item => new ClearcaseManagerViewModel() },
                        new DividerNavigationItem(),
                    };
                case NAVIGATION_CATEGORY.INTEGRATION:
                    return new List<INavigationItem>()
                    {
                        new SubheaderNavigationItem()  { Subheader = "INTEGRATION" },
                        new FirstLevelNavigationItem() { Label = "Test Descriptor Helper",      Icon = PackIconKind.TestTube,         NavigationItemSelectedCallback = item => new ClearcaseManagerViewModel() },
                        new FirstLevelNavigationItem() { Label = "Queue ETA",                   Icon = PackIconKind.Timetable,        NavigationItemSelectedCallback = item => new ClearcaseManagerViewModel() },
                        new FirstLevelNavigationItem() { Label = "Artifacts Analyzer",          Icon = PackIconKind.TableSearch,      NavigationItemSelectedCallback = item => new ClearcaseManagerViewModel() },
                        new DividerNavigationItem(),
                    };
                case NAVIGATION_CATEGORY.DOCUMENTATION:
                    return new List<INavigationItem>()
                    {
                        new SubheaderNavigationItem()  { Subheader = "DOCUMENTATION" },
                        new FirstLevelNavigationItem() { Label = "Version Log",                 Icon = PackIconKind.Wunderlist,       NavigationItemSelectedCallback = item => new VersionLogViewModel() },
                        new FirstLevelNavigationItem() { Label = "Licenses",                    Icon = PackIconKind.BarcodeScanner,   NavigationItemSelectedCallback = item => new LicenseListViewModel() },
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
    }
}
