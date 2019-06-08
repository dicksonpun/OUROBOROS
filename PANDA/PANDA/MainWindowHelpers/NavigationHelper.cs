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
using System.Threading.Tasks;
using System.IO;
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

    public class NavigationHelper
    {
        // Instantiate a Singleton of the Semaphore with a value of 1. This means that only 1 thread can be granted access at a time.
        private static SemaphoreSlim m_semaphore = new SemaphoreSlim(1, 1);

        // Helpers
        private readonly SupportedNetworkModeHelper m_supportedNetworkModeHelper;

        // Members
        private readonly MainWindow m_mainWindow;
        public List<ClearcaseManagerViewItem> navigationClearcaseViews;

        public NavigationHelper(MainWindow currentMainWindow, SupportedNetworkModeHelper supportedNetworkModeHelper)
        {
            m_mainWindow                 = currentMainWindow;
            m_supportedNetworkModeHelper = supportedNetworkModeHelper;
            navigationClearcaseViews     = new List<ClearcaseManagerViewItem>();
        }

        public void InitializeNavigationDrawerNav()
        {
            NavigationItems = new ObservableCollection<INavigationItem>();
            List<NAVIGATION_CATEGORY> navigationCategoryOrder = new List<NAVIGATION_CATEGORY> { NAVIGATION_CATEGORY.DASHBOARD,
                                                                                                NAVIGATION_CATEGORY.VERSION_CONTROL,
                                                                                                NAVIGATION_CATEGORY.INTEGRATION,
                                                                                                NAVIGATION_CATEGORY.DOCUMENTATION };

            // Populate navigation menu
            foreach (var supportedNavigationCategory in navigationCategoryOrder)
            {
                foreach (var NavItem in GetNavigationCategory(supportedNavigationCategory))
                {
                    NavigationItems.Add(NavItem);
                }
            }

            SetNavigationSelection(1);
            StartAutoRefresh();
        }

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

        public List<string> GetListOfViews(List<ClearcaseManagerViewItem> viewList)
        {
            List<string> result = new List<string>();

            foreach (var view in viewList)
                result.Add(view.ViewName);

            return result;
        }

        public CancellationToken PeriodicUpdateCancellationToken { get; set; }
        public async void StartAutoRefresh()
        {
            await RefreshClearcaseViewsPeriodically(TimeSpan.FromSeconds(3), PeriodicUpdateCancellationToken);
        }

        public async Task RefreshClearcaseViewsPeriodically(TimeSpan interval, CancellationToken cancellationToken)
        {
            string directoryPath = m_supportedNetworkModeHelper.CurrentNetworkMode.NetworkSpecificPath;

            while (true)
            {
                // Asynchronously wait to enter the Semaphore. If no-one has been granted access to the Semaphore, code execution will proceed, otherwise this thread waits here until the m_semaphore is released 
                await m_semaphore.WaitAsync();
                try
                {
                    List<ClearcaseManagerViewItem> updatedClearcaseViews = new List<ClearcaseManagerViewItem>(); // Used to populate the active views
                    List<string> navigationViews = GetListOfViews(navigationClearcaseViews);

                    // Ensure connection is not lost.  Only perform update if directory is accessible.
                    if (Directory.Exists(directoryPath))
                    {
                        string myUsername = "dickson"; // TODO: NEED to build user settings profile

                        foreach (var currentDirectory in Directory.GetDirectories(directoryPath))
                        {
                            var dir = new DirectoryInfo(currentDirectory);

                            // Build list of eligible user views (User views + User previously selected views)
                            if (dir.Name.StartsWith(myUsername) || navigationViews.Contains(dir.Name))
                            {
                                updatedClearcaseViews.Add(new ClearcaseManagerViewItem() { Icon = PackIconKind.SourceBranch, ViewName = dir.Name, ViewPath = dir.FullName });
                            }
                        }

                        // Generate list of strings for easier comparison
                        List<string> newClearcaseViews = GetListOfViews(updatedClearcaseViews);
                        IEnumerable<string> itemsToRemove = navigationViews.Except(newClearcaseViews);
                        IEnumerable<string> itemsToAdd = newClearcaseViews.Except(navigationViews);

                        // Sort by alphabetical order by viewnames
                        updatedClearcaseViews.OrderBy(p => p.ViewName);
                        navigationClearcaseViews.OrderBy(p => p.ViewName);

                        // Loop through all navigation items 
                        for (int i = 0; i < NavigationItems.Count; i++)
                        {
                            INavigationItem navItem = NavigationItems[i];

                            if (navItem.GetType().IsEquivalentTo(new SubheaderNavigationItem().GetType()))
                            {
                                SubheaderNavigationItem temp = (SubheaderNavigationItem)navItem;
                                if (temp.Subheader.Equals("VERSION CONTROL"))
                                {
                                    // Set index to entry after "VERSION CONTROL"
                                    int index = NavigationItems.IndexOf(temp) + 1;

                                    // Remove views to the navigation menu
                                    foreach (ClearcaseManagerViewItem clearcaseItem in navigationClearcaseViews)
                                    {
                                        string key = "ClearcaseViewViewModel." + clearcaseItem.ViewName;
                                        if (itemsToRemove.Contains(clearcaseItem.ViewName))
                                        {
                                            // Remove from navigation
                                            NavigationItems.RemoveAt(index);

                                            // Remove from ViewModelMap
                                            RemoveViewModelFromMap(key);
                                        }
                                        else
                                        {
                                            // Only increment if current iteration did not remove item, to prevent index mismatch from shifting
                                            index++;
                                        }
                                    }

                                    // Reset index to entry after "VERSION CONTROL"
                                    index = NavigationItems.IndexOf(temp) + 1;

                                    // Add views to the navigation menu
                                    foreach (ClearcaseManagerViewItem clearcaseItem in updatedClearcaseViews)
                                    {
                                        string key = "ClearcaseViewViewModel." + clearcaseItem.ViewName;

                                        if (itemsToAdd.Contains(clearcaseItem.ViewName))
                                        {
                                            // Add to ViewModelMap
                                            GetViewModelFromMap(key);

                                            // Add to navigation
                                            INavigationItem navigationItem = new FirstLevelNavigationItem() { Label = clearcaseItem.ViewName, Icon = PackIconKind.Git, NavigationItemSelectedCallback = item => GetViewModelFromMap(key) };
                                            NavigationItems.Insert(index, navigationItem);
                                        }
                                        // Increment index regardless of insertion
                                        index++;
                                    }
                                    // Update to latest clearcase views for navigation menu
                                    navigationClearcaseViews = updatedClearcaseViews;
                                    break;
                                }
                            }
                        }
                    }
                    await Task.Delay(interval, cancellationToken);
                }
                finally
                {
                    // When the task is ready, release the m_semaphore. It is vital to ALWAYS release the m_semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                    // This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                    m_semaphore.Release();
                }
            };
        }

        public Dictionary<string, ViewModel.ViewModel> ViewModelMap = new Dictionary<string, ViewModel.ViewModel>();

        public ViewModel.ViewModel GetViewModelFromMap(string key)
        {
            // If the requested ViewModel instance does not exist, create it and then return it
            if (!ViewModelExist(key))
            {
                // The expected key pattern is <viewModel_Type>.<unique_Identifier> 
                // The .<unique_Identifier> is optional but required if multiple instances of the same viewModel are created
                if (key.StartsWith("ClearcaseViewViewModel"))
                {
                    ViewModelMap.Add(key, new ViewModel.ViewModel()); // temp
                }
                else if (key.StartsWith("VersionLogViewModel"))
                {
                    ViewModelMap.Add(key, new VersionLogViewModel());
                }
                else if (key.StartsWith("LicenseLogViewModel"))
                {
                    ViewModelMap.Add(key, new LicenseLogViewModel());
                }
            }
            return ViewModelMap[key]; ;
        }

        public void RemoveViewModelFromMap(string key)
        {
            ViewModelMap.Remove(key);
        }

        public bool ViewModelExist(string key)
        {
            return ViewModelMap.ContainsKey(key);
        }

        public List<string> GetCurrentNavigationClearcaseViews()
        {
            List<string> result = new List<string>();

            foreach (KeyValuePair<string, ViewModel.ViewModel> entry in ViewModelMap)
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
                    return new List<INavigationItem>()
                    {
                        new SubheaderNavigationItem()  { Subheader = "VERSION CONTROL" },
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
                    AddToViewModelMap(new List<string> { "VersionLogViewModel",            // Instantiate viewModels
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

        private ObservableCollection<INavigationItem> m_navigationItems;

        public ObservableCollection<INavigationItem> NavigationItems
        {
            get { return m_navigationItems; }
            set { m_navigationItems = value; }
        }
    }
}

