using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;
using PANDA.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PANDA
{
    public partial class NavigationHelper
    {
        // Instantiate a Singleton of the Semaphore with a value of 1. This means that only 1 thread can be granted access at a time.
        private static readonly SemaphoreSlim navigationUpdate_Mutex = new SemaphoreSlim(1, 1);

        private CancellationTokenSource cancellationTokenSource;

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

        public async Task RefreshClearcaseViewsPeriodically(TimeSpan interval, CancellationToken cancellationToken)
        {
            string directoryPath = m_supportedNetworkModeHelper.CurrentNetworkMode.NetworkSpecificPath;
            string myUsername = "dickson"; // TODO: NEED to build user settings profile

            while (true)
            {
                // Asynchronously wait to enter the Semaphore. If no-one has been granted access to the Semaphore, code execution will proceed, otherwise this thread waits here until the navigationUpdate_Mutex is released 
                await navigationUpdate_Mutex.WaitAsync();
                try
                {
                    // Ensure connection is not lost.  Only perform update if directory is accessible.
                    if (!Directory.Exists(directoryPath))
                    {
                        continue;
                    }

                    // Build list of eligible user views (User views + User previously selected views)
                    List<ClearcaseManagerViewItem> updatedClearcaseViews = new List<ClearcaseManagerViewItem>();
                    List<string> navigationViews = GetListOfViews(navigationClearcaseViews);
                    foreach (var currentDirectory in Directory.GetDirectories(directoryPath))
                    {
                        var dir = new DirectoryInfo(currentDirectory);
                        if (dir.Name.StartsWith(myUsername) || navigationViews.Contains(dir.Name))
                        {
                            updatedClearcaseViews.Add(new ClearcaseManagerViewItem() { Icon = PackIconKind.Git, ViewName = dir.Name, ViewPath = dir.FullName });
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
                                // Check if mounted to Y-Drive
                                string yDrivePath = m_YDriveMounter.volumeFunctions.DriveIsMappedTo("Y:");
                                string mountedView = yDrivePath.Split('\\').Last();

                                // Set index to entry after "VERSION CONTROL"
                                int index = NavigationItems.IndexOf(temp) + 1;

                                // Remove views to the navigation menu
                                foreach (ClearcaseManagerViewItem clearcaseItem in navigationClearcaseViews)
                                {
                                    // Set icon for Y-Drive accordingly
                                    // NOTE: Only need to update in removal loop from this update.
                                    //       The "additions" from this update will get processed on the next update's removal loop.
                                    INavigationItem navClearcaseItem = NavigationItems[index];
                                    FirstLevelNavigationItem tempClearcaseItem = (FirstLevelNavigationItem)navClearcaseItem;

                                    // Set default icon
                                    PackIconKind viewIcon = PackIconKind.Git;
                                    if (clearcaseItem.Icon == PackIconKind.Git && mountedView.Equals(clearcaseItem.ViewName))
                                    {
                                        // Upgrade to Y-Drive icon
                                        viewIcon = PackIconKind.LetterYBox;
                                    }
                                    tempClearcaseItem.Icon = viewIcon;

                                    // Remove views
                                    string key = "ClearcaseViewTabControlViewModel." + clearcaseItem.ViewName;
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
                                    string key = "ClearcaseViewTabControlViewModel." + clearcaseItem.ViewName;

                                    if (itemsToAdd.Contains(clearcaseItem.ViewName))
                                    {
                                        // Add to navigation
                                        INavigationItem navigationItem = new FirstLevelNavigationItem() { Label = clearcaseItem.ViewName, Icon = clearcaseItem.Icon, NavigationItemSelectedCallback = item => GetViewModelFromMap(key, clearcaseItem.ViewPath) };
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
                    // When the task is ready, release the navigationUpdate_Mutex. It is vital to ALWAYS release the 
                    // navigationUpdate_Mutex when we are ready, or else we will end up with a Semaphore that is forever locked.
                    // This is why it is important to do the Release within a try...finally clause; program execution may crash or 
                    // take a different path, this way you are guaranteed execution.
                    navigationUpdate_Mutex.Release();
                }
            };
        }
    }
}