using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;
using PANDA.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PANDA
{
    public partial class NavigationHelper
    {
        // Instantiate a Singleton of the Semaphore with a value of 1. This means that only 1 thread can be granted access at a time.
        private static readonly SemaphoreSlim m_mutex = new SemaphoreSlim(1, 1);

        private CancellationTokenSource cancellationTokenSource;

        public Dictionary<string, ClearcaseManagerViewItem> ClearcaseViewDictionary = new Dictionary<string, ClearcaseManagerViewItem>();

        public async void RequestLock()
        {
            // Asynchronously wait to enter the Semaphore. 
            // If no-one has been granted access to the Semaphore, code execution will proceed.
            // Otherwise, this thread waits here until the m_mutex is released.
            await m_mutex.WaitAsync();
        }

        public void ReleaseLock()
        {
            // TLDR: Make sure you ALWAYS call this function from the 'finally' clause of a try-catch-finally statement.
            // When the task is ready, release the m_mutex. It is vital to ALWAYS release the 
            // m_mutex when we are ready, or else we will end up with a Semaphore that is forever locked.
            // This is why it is important to do the Release within a try...finally clause; program execution may crash or 
            // take a different path, this way you are guaranteed execution.
            m_mutex.Release();
        }

        public async void StartAutoRefresh()
        {
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken PeriodicUpdateCancellationToken = cancellationTokenSource.Token;
            try
            {
                await RefreshClearcaseViewsPeriodically(TimeSpan.FromSeconds(3), PeriodicUpdateCancellationToken);
            }
            catch
            {
                cancellationTokenSource.Dispose();
            }
        }
        public void StopAutoRefresh()
        {
            cancellationTokenSource.Cancel();
        }

        public List<string> GetListOfViews(List<ClearcaseManagerViewItem> viewList)
        {
            List<string> result = new List<string>();

            foreach (var view in viewList)
                result.Add(view.ViewName);

            return result;
        }

        public bool IsViewMountedToYDrive(string viewname)
        {
            bool result = false;

            string yDrivePath = m_YDriveMounter.volumeFunctions.DriveIsMappedTo("Y:");
            string mountedView = yDrivePath.Split('\\').Last();
            // Check if view is mounted to Y-Drive
            if (mountedView.Equals(viewname))
            {
                result = true;
            }
            return result;
        }

        public void GetLatestAvailableViews(string directoryPath)
        {
            ClearcaseViewDictionary.Clear(); // Clear all entries to account for removals.
            foreach (var currentDirectory in Directory.GetDirectories(directoryPath))
            {
                var dir = new DirectoryInfo(currentDirectory);
                // The Add method throws an exception if the new key is already in the dictionary. (Unlikely since all entries were cleared prior.)
                try
                {
                    PackIconKind icon = PackIconKind.Git;
                    if (IsViewMountedToYDrive(dir.Name))
                    {
                        icon = PackIconKind.AlphaYBox;
                    }
                    ClearcaseManagerViewItem viewItem = new ClearcaseManagerViewItem { Icon = icon, ViewName = dir.Name, ViewPath = dir.FullName };

                    ClearcaseViewDictionary.Add(dir.Name, viewItem);
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("An element with Key = " + dir.Name + " already exists.");
                }
            }
        }

        public int GetIndexAfterSubheader(string subheader)
        {
            int result = 0;
            // Loop through all navigation items 
            for (int i = 0; i < NavigationItems.Count; i++)
            {
                INavigationItem navItem = NavigationItems[i];
                if (navItem.GetType().IsEquivalentTo(new SubheaderNavigationItem().GetType()))
                {
                    SubheaderNavigationItem temp = (SubheaderNavigationItem)navItem;
                    if (temp.Subheader.Equals(subheader))
                    {
                        // Set index to entry after subheader
                        result = NavigationItems.IndexOf(temp) + 1;
                        break;
                    }
                }
            }
            return result;
        }

        public void UpdateNavigationRemovals()
        {
            List<string> allViews = GetListOfViews(ClearcaseViewDictionary.Values.ToList());
            IEnumerable<string> viewsToRemove = GetListOfViews(navigationClearcaseViews).Except(allViews);

            if (!string.IsNullOrEmpty(RequestedRemoveView))
            {
                // Add non-username view via Union to prevent duplicates. 
                viewsToRemove = viewsToRemove.Union(new List<string> { RequestedRemoveView }).ToList();
                RequestedRemoveView = string.Empty;   // Clear request.
            }

            // Remove views to the navigation menu
            int removeIndex = GetIndexAfterSubheader("VERSION CONTROL");
            foreach (ClearcaseManagerViewItem clearcaseItem in navigationClearcaseViews)
            {
                // Remove views
                string key = "ClearcaseViewTabControlViewModel." + clearcaseItem.ViewName;
                if (viewsToRemove.Contains(clearcaseItem.ViewName))
                {
                    // Remove from navigation
                    NavigationItems.RemoveAt(removeIndex);

                    // Remove from ViewModelMap
                    RemoveViewModelFromMap(key);
                }
                else
                {
                    // Only increment if current iteration did not remove item, to prevent index mismatch from shifting
                    removeIndex++;
                }
            }
            // Set current views to latest views (post-removals)
            navigationClearcaseViews.RemoveAll(x => viewsToRemove.ToList().Contains(x.ViewName));
        }

        public void UpdateNavigationAdditions()
        {
            string myUsername = "dickson"; // TODO: NEED to build user settings profile
            List<string> currentNavigationViews = GetListOfViews(navigationClearcaseViews);
            List<string> requestedAdds = GetListOfViews(ClearcaseViewDictionary.Values.Where(d => d.ViewName.StartsWith(myUsername)).ToList()); // Username
            if (!string.IsNullOrEmpty(RequestedAddView))
            {
                // Add non-username view via Union to prevent duplicates. 
                requestedAdds = requestedAdds.Union(new List<string> { RequestedAddView }).ToList();
                RequestedAddView = string.Empty;   // Clear request.
            }
            requestedAdds = requestedAdds.Union(currentNavigationViews).ToList();
            requestedAdds.Sort();
            IEnumerable<string> viewsToAdd = requestedAdds.Except(currentNavigationViews);

            // Generate updated list of navigation views
            List<ClearcaseManagerViewItem> updatedClearcaseViews = new List<ClearcaseManagerViewItem>();
            foreach (string view in requestedAdds)
            {
                ClearcaseManagerViewItem value;
                if (ClearcaseViewDictionary.TryGetValue(view, out value))
                {
                    updatedClearcaseViews.Add(value);
                }
                else
                {
                    // Unable to access value, must be unused. Remove associated viewModel to free memory.
                    RemoveViewModelFromMap("ClearcaseViewTabControlViewModel." + view);
                }
            }

            // Add views to the navigation menu
            int addIndex = GetIndexAfterSubheader("VERSION CONTROL");
            foreach (ClearcaseManagerViewItem clearcaseItem in updatedClearcaseViews)
            {
                string key = "ClearcaseViewTabControlViewModel." + clearcaseItem.ViewName;

                if (viewsToAdd.Contains(clearcaseItem.ViewName))
                {
                    // Add to navigation
                    GetViewModelFromMap(key, clearcaseItem.ViewPath); // Ensure viewModel is instantiated
                    INavigationItem navigationItem = new FirstLevelNavigationItem() { Label = clearcaseItem.ViewName, Icon = clearcaseItem.Icon, NavigationItemSelectedCallback = item => GetViewModelFromMap(key, clearcaseItem.ViewPath) };
                    NavigationItems.Insert(addIndex, navigationItem);
                }
                // Increment index regardless of insertion
                addIndex++;
            }
            // Set current views to latest views (post-additions)
            navigationClearcaseViews = updatedClearcaseViews;
        }

        public async Task RefreshClearcaseViewsPeriodically(TimeSpan interval, CancellationToken cancellationToken)
        {
            string directoryPath = m_supportedNetworkModeHelper.CurrentNetworkMode.NetworkSpecificPath;

            while (true)
            {
                RequestLock();
                try
                {
                    // Ensure connection is not lost.  Only perform update if directory is accessible.
                    if (!Directory.Exists(directoryPath))
                    {
                        continue;
                    }

                    GetLatestAvailableViews(directoryPath);
                    UpdateNavigationRemovals();
                    UpdateNavigationAdditions();

                    await Task.Delay(interval, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    // Throw immediately to be responsive. The alternative is to do one more item of work, and throw on next iteration 
                    // because IsCancellationRequested will be true.
                    Console.WriteLine("Navigation Menu Auto-Refresh Canceled.");
                    throw;
                }
                finally
                {
                    ReleaseLock();
                }
            };
        }
    }
}