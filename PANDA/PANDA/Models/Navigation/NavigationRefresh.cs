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
        // Members
        private CancellationTokenSource cancellationTokenSource;
        public Dictionary<string, ClearcaseManagerViewItem> ClearcaseViewDictionary = new Dictionary<string, ClearcaseManagerViewItem>();
        public bool UpdateDetectedChanges;      // Flag for detecting additions or removals from navigation menu for Clearcase views
        public bool UpdateDetectedYMountChange; // Flag for detecting mounted Y-Drive changed
        private string PreviousYDriveMountedView = string.Empty;

        // Instantiate a Singleton of the Semaphore with a value of 1. This means that only 1 thread can be granted access at a time.
        private static readonly SemaphoreSlim m_mutex = new SemaphoreSlim(1, 1);

        // ----------------------------------------------------------------------------------------
        // Class       : NavigationHelper
        // Method      : RequestLock
        // Description : Asynchronously wait to enter the Semaphore. 
        //               If no-one has been granted access to the Semaphore, code execution will proceed.
        //               Otherwise, this thread waits here until the m_mutex is released.
        // ----------------------------------------------------------------------------------------
        public async void RequestLock()
        {
            await m_mutex.WaitAsync();
        }

        // ----------------------------------------------------------------------------------------
        // Class       : NavigationHelper
        // Method      : ReleaseLock
        // Description : TLDR: Make sure you ALWAYS call this function from the 'finally' clause of a try-catch-finally statement.
        //               When the task is finished, release the m_mutex.
        //               It is vital to ALWAYS release the m_mutex when ready or else the Semaphore is deadlocked.
        //               This is why it is important to do the Release within a try...finally clause; program execution may crash or 
        //               take a different path, this way you are guaranteed execution of release.
        // ----------------------------------------------------------------------------------------
        public void ReleaseLock()
        {
            m_mutex.Release();
        }

        // ----------------------------------------------------------------------------------------
        // Class       : NavigationHelper
        // Method      : AddSelectedView
        // Description : Sets the RequestedAddView parameter to the requested view under lock.
        // ----------------------------------------------------------------------------------------
        public void AddSelectedView(string view)
        {
            RequestLock();
            RequestedAddView = view;
            ReleaseLock();
        }

        // ----------------------------------------------------------------------------------------
        // Class       : NavigationHelper
        // Method      : RemoveSelectedView
        // Description : Sets the RequestedRemoveView parameter to the requested view under lock.
        // ----------------------------------------------------------------------------------------
        public void RemoveSelectedView(string view)
        {
            RequestLock();
            RequestedRemoveView = view;
            ReleaseLock();
        }

        // ----------------------------------------------------------------------------------------
        // Class       : NavigationHelper
        // Method      : StartAutoRefresh
        // Description : Starts an auto-refresh task asynchronously. Upon a cancellation request,
        //               the task will throw a OperationCanceledException and the catch will release
        //               off all resources used by the current instance of CancellationTokenSource.
        // ----------------------------------------------------------------------------------------
        public async void StartAutoRefresh()
        {
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken PeriodicUpdateCancellationToken = cancellationTokenSource.Token;
            try
            {
                await RefreshClearcaseViewsPeriodically(TimeSpan.FromSeconds(1), PeriodicUpdateCancellationToken);
            }
            catch
            {
                cancellationTokenSource.Dispose();
            }
        }

        // ----------------------------------------------------------------------------------------
        // Class       : NavigationHelper
        // Method      : StopAutoRefresh
        // Description : Requests a cancellation to be processed on the next asynchronous action.
        // ----------------------------------------------------------------------------------------
        public void StopAutoRefresh()
        {
            cancellationTokenSource.Cancel();
        }

        // ----------------------------------------------------------------------------------------
        // Class       : NavigationHelper
        // Method      : GetListOfViews
        // Description : Returns a list of strings extracted in the order of the provided list.
        // Parameters  :
        // - viewList (List<ClearcaseManagerViewItem>) : List of ClearcaseManagerViewItem objects
        // ----------------------------------------------------------------------------------------
        public List<string> GetListOfViews(List<ClearcaseManagerViewItem> viewList)
        {
            List<string> result = new List<string>();

            foreach (var view in viewList)
                result.Add(view.ViewName);

            return result;
        }

        // ----------------------------------------------------------------------------------------
        // Class       : NavigationHelper
        // Method      : IsViewMountedToYDrive
        // Description : Returns boolean:
        //               TRUE  - Provided view name is currently mapped to the Y-Drive.
        //               FALSE - Provided view name is not mapped to the Y-Drive.
        // Parameters  :
        // - viewname (string) : name of view
        // ----------------------------------------------------------------------------------------
        public bool IsViewMountedToYDrive(string viewname)
        {
            bool result = false;

            string yDrivePath = AccessYDriveMounter.volumeFunctions.DriveIsMappedTo("Y:");
            string mountedView = yDrivePath.Split('\\').Last();
            // Check if view is mounted to Y-Drive
            if (mountedView.Equals(viewname))
            {
                result = true;
            }
            return result;
        }

        // ----------------------------------------------------------------------------------------
        // Class       : NavigationHelper
        // Method      : UpdateViews
        // Description : Gets all of the latest available views.
        //               The Dictionary mapping view to ClearcaseManagerViewItem is cleared to account for removals.
        //               The Dictionary is then re-populated with all the latest views.
        //               Y-Drive is determined and the icon for the Navigation item is updated accordingly.
        // ----------------------------------------------------------------------------------------
        public void UpdateViews(string directoryPath)
        {
            // Clear all entries to account for removals.
            ClearcaseViewDictionary.Clear();

            string currentYDriveMountedView = string.Empty;

            // The GetDirectories method throws a variety of exceptions, catch them all and write to log
            try
            {
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
                            currentYDriveMountedView = dir.Name;
                        }

                        ClearcaseManagerViewItem viewItem = new ClearcaseManagerViewItem
                        {
                            Icon = icon,
                            ViewName = dir.Name,
                            ViewPath = dir.FullName,
                            IsUserView = AccessUserSettingsHelper.DetermineUserOrNonuserView(dir.FullName)
                        };

                        ClearcaseViewDictionary.Add(dir.Name, viewItem);
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine("An element with Key = " + dir.Name + " already exists."); // Should never happen.
                    }
                }

                // Check if mounted Y: Drive changed
                if (!currentYDriveMountedView.Equals(PreviousYDriveMountedView))
                {
                    // Loop through all navigation items 
                    foreach (INavigationItem navItem in NavigationItems)
                    {
                        if (navItem.GetType().IsEquivalentTo(new FirstLevelNavigationItem().GetType()))
                        {
                            FirstLevelNavigationItem viewNavItem = (FirstLevelNavigationItem)navItem;

                            // Update icons if currently in navigation menu
                            if (viewNavItem.Label.Equals(PreviousYDriveMountedView) ||
                                viewNavItem.Label.Equals(currentYDriveMountedView))
                            {
                                if (ClearcaseViewDictionary.TryGetValue(viewNavItem.Label, out ClearcaseManagerViewItem buffer)) // Returns true.
                                {
                                    viewNavItem.Icon = buffer.Icon;
                                }
                            }
                        }
                    }

                    // Update mounted view and flag
                    PreviousYDriveMountedView = currentYDriveMountedView; 
                    UpdateDetectedYMountChange = true;
                }
            }
            // TODO: Implement console log
            catch (UnauthorizedAccessException) { }
            catch (ArgumentException)           { }
            catch (PathTooLongException)        { }
            catch (IOException)                 { }
        }

        // ----------------------------------------------------------------------------------------
        // Class       : NavigationHelper
        // Method      : GetStartingNavigationIndexForClearcaseViews
        // Description : Returns the next index following the requested subheader in the navigation menu.
        // Parameters  :
        // - subheader (string) : Subheader value
        // ----------------------------------------------------------------------------------------
        public int GetStartingNavigationIndexForClearcaseViews()
        {
            // Loop through all navigation items 
            foreach (INavigationItem navItem in NavigationItems)
            {
                if (navItem.GetType().IsEquivalentTo(new FirstLevelNavigationItem().GetType()))
                {
                    FirstLevelNavigationItem viewNavItem = (FirstLevelNavigationItem)navItem;
                    if (viewNavItem.Label.Equals("Clearcase Manager"))
                    {
                        // Set index to entry after found entry
                        return (NavigationItems.IndexOf(viewNavItem) + 1);
                    }
                }
            }

            return 0; // This should never happen.
        }

        // ----------------------------------------------------------------------------------------
        // Class       : NavigationHelper
        // Method      : UpdateNavigationRemovals
        // Description : Determines what views to remove from the ViewModelMap and the navigation menu.
        //               Get the difference between the list of current views and list of latest available views.
        //               The difference is the views on the navigation menu that can be removed.
        //               Additionally, add the union of the requested view removal from an external source.
        //               The external request typically pertains to non-username views (not main user).
        //               The navigation menu and ViewModelMap is updated.
        // ----------------------------------------------------------------------------------------
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
            int removeIndex = GetStartingNavigationIndexForClearcaseViews();
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

                    // Update change detected flag
                    UpdateDetectedChanges = true;
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

        // ----------------------------------------------------------------------------------------
        // Class       : NavigationHelper
        // Method      : UpdateNavigationAdditions
        // Description : Determines what views to add to the ViewModelMap and the navigation menu.
        //               Get the difference between the list of current views and list of latest available views 
        //               that include the username.  The difference is what can be added to the navigation menu.
        //               Additionally, add the union of the requested view addition from an external source.
        //               The external request typically pertains to non-username views (not main user).
        //               The navigation menu and ViewModelMap is updated.
        //               NOTE: In the event of a race condition between a view being added in the app and 
        //                     the same view being removed, the view in question will not be added to the
        //                     navigation menu and it will be removed from the ViewModelMap, if it exists.
        // ----------------------------------------------------------------------------------------
        public void UpdateNavigationAdditions()
        {
            // Get username
            string username = AccessUserSettingsHelper.UserProfile.Username.ToString();

            List<string> currentNavigationViews = GetListOfViews(navigationClearcaseViews);
            List<string> requestedAdds = GetListOfViews(ClearcaseViewDictionary.Values
                                        .Where(d => d.ViewName.Split('-').First().Equals(username)).ToList()); // Username views

            if (!string.IsNullOrEmpty(RequestedAddView))
            {
                // Add non-username view via Union to prevent duplicates. 
                requestedAdds = requestedAdds.Union(new List<string> { RequestedAddView }).ToList();
                RequestedAddView = string.Empty;   // Clear request.
            }

            requestedAdds = requestedAdds.Union(currentNavigationViews).ToList();
            requestedAdds.Sort(); // Sort before populating list so the list on navigation menu is sorted
            IEnumerable<string> viewsToAdd = requestedAdds.Except(currentNavigationViews);

            // Generate updated list of navigation views
            List<ClearcaseManagerViewItem> updatedClearcaseViews = new List<ClearcaseManagerViewItem>();
            foreach (string view in requestedAdds)
            {
                if (ClearcaseViewDictionary.TryGetValue(view, out ClearcaseManagerViewItem value))
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
            int addIndex = GetStartingNavigationIndexForClearcaseViews();
            foreach (ClearcaseManagerViewItem clearcaseItem in updatedClearcaseViews)
            {
                string key = "ClearcaseViewTabControlViewModel." + clearcaseItem.ViewName; 

                if (viewsToAdd.Contains(clearcaseItem.ViewName))
                {
                    // Add to navigation
                    GetViewModelFromMap(key, clearcaseItem.ViewPath, username); // Ensure viewModel is instantiated
                    INavigationItem navigationItem = new FirstLevelNavigationItem()
                    {
                        Label                          = clearcaseItem.ViewName,
                        Icon                           = clearcaseItem.Icon,
                        NavigationItemSelectedCallback = item => GetViewModelFromMap(key, clearcaseItem.ViewPath, username)
                    };
                    NavigationItems.Insert(addIndex, navigationItem);

                    // Update change detected flag
                    UpdateDetectedChanges = true;
                }
                // Increment index regardless of insertion
                addIndex++;
            }
            // Set current views to latest views (post-additions)
            navigationClearcaseViews = updatedClearcaseViews;
        }

        // ----------------------------------------------------------------------------------------
        // Class       : NavigationHelper
        // Method      : UpdateClearcaseManagerAutocompleteSource
        // Description : Sets the ClearcaseManagerAutocompleteSource to the latest views update.
        // ----------------------------------------------------------------------------------------
        public void UpdateClearcaseManagerAutocompleteSource()
        {
            ClearcaseManagerViewModel viewModel = (ClearcaseManagerViewModel)GetViewModelFromMap("ClearcaseManagerViewModel");
            viewModel.ClearcaseManagerAutocompleteSource = new ClearcaseManagerAutocompleteSource(ClearcaseViewDictionary.Values.ToList());
            viewModel.ClearcaseManagerBackgroundRefreshSource = new ObservableCollection<ClearcaseManagerViewItem>(navigationClearcaseViews.ToList());
        }

        // ----------------------------------------------------------------------------------------
        // Class       : NavigationHelper
        // Method      : RefreshClearcaseViewsPeriodically
        // Description : This asynchronous Task refreshes the available views periodically.
        //               The update processing handles both internal and external updates.
        // ----------------------------------------------------------------------------------------
        public async Task RefreshClearcaseViewsPeriodically(TimeSpan interval, CancellationToken cancellationToken)
        {
            string directoryPath = AccessSupportedNetworkModeHelper.CurrentNetworkMode.NetworkSpecificPath;

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

                    // Reset flag for detected change in current update
                    UpdateDetectedChanges      = false;
                    UpdateDetectedYMountChange = false;

                    // Internal Updates
                    UpdateViews(directoryPath);
                    UpdateNavigationRemovals();
                    UpdateNavigationAdditions();

                    // Only update external sources if changes detected
                    if (UpdateDetectedChanges || UpdateDetectedYMountChange)
                    {
                        // External Updates
                        UpdateClearcaseManagerAutocompleteSource();
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
                    ReleaseLock();
                }
            };
        }
    }
}