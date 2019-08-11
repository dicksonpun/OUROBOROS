using MaterialDesignExtensions.Model;
using PANDA.Command;
using PANDA.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static PANDA.Model.ClearcaseProjectSourceModel;

namespace PANDA.ViewModel
{
    public class ClearcaseProjectSourceViewModel : ViewModel
    {
        // ----------------------------------------------------------------------------------------
        // Members
        // ----------------------------------------------------------------------------------------
        public ClearcaseProjectSourceModel ClearcaseProjectSourceModel;
        private CancellationTokenSource cancellationTokenSource;

        private static readonly SemaphoreSlim m_SearchResultsMutex = new SemaphoreSlim(1, 1);
        public async void RequestLock() { await m_SearchResultsMutex.WaitAsync(); }
        public void ReleaseLock() { m_SearchResultsMutex.Release(); }
        // ----------------------------------------------------------------------------------------
        // Constructor
        // ----------------------------------------------------------------------------------------
        public ClearcaseProjectSourceViewModel(string viewName)
        {
            // Project Source Search
            m_unfilteredSearchSource = new ObservableCollection<ClearcaseProjectSourceItem>() { };
            m_unfilteredSearchSourceResults = new ObservableCollection<ClearcaseProjectSourceItem>() { };
            SearchTerm = string.Empty; // NOTE: Set after initializing SearchSource(s) to avoid null reference.
            ClearcaseProjectSourceModel = new ClearcaseProjectSourceModel(viewName);

            // VOB Filter
            m_selectedVOBItem = null;
            m_VOBAutocompleteSource = new ClearcaseVOBAutocompleteSource();
            m_selectedVOBFilters = new ObservableCollection<ClearcaseVOBItem>();

            // Register FileSystemWatcher and auto-refresh processing
            StartAutoRefresh();
        }
        // ----------------------------------------------------------------------------------------
        // Databinding
        // ----------------------------------------------------------------------------------------
        private string m_searchTerm;
        public string SearchTerm
        {
            get { return m_searchTerm; }

            set
            {
                m_searchTerm = value;
                OnPropertyChanged(nameof(SearchTerm));
                UpdateSearchSourceResults(); // Update results accordingly
            }
        }

        private ObservableCollection<ClearcaseProjectSourceItem> m_unfilteredSearchSource;
        public ObservableCollection<ClearcaseProjectSourceItem> UnfilteredSearchSource
        {
            get { return m_unfilteredSearchSource; }
            set
            {
                m_unfilteredSearchSource = value;
                OnPropertyChanged(nameof(UnfilteredSearchSource));
            }
        }

        private ObservableCollection<ClearcaseProjectSourceItem> m_unfilteredSearchSourceResults;
        public ObservableCollection<ClearcaseProjectSourceItem> UnfilteredSearchSourceResults
        {
            get { return m_unfilteredSearchSourceResults; }
            set
            {
                m_unfilteredSearchSourceResults = value;
                OnPropertyChanged(nameof(UnfilteredSearchSourceResults));
            }
        }

        private IAutocompleteSource m_VOBAutocompleteSource;
        public IAutocompleteSource VOBAutocompleteSource
        {
            get { return m_VOBAutocompleteSource; }
        }

        private ClearcaseVOBItem m_selectedVOBItem;
        public ClearcaseVOBItem SelectedVOBItem
        {
            get { return m_selectedVOBItem; }

            set
            {
                m_selectedVOBItem = value;
                OnPropertyChanged(nameof(SelectedVOBItem));

                AddToSelectedVOBFilters(value);
            }
        }
        // Helper functions
        public void AddToSelectedVOBFilters(ClearcaseVOBItem item)
        {
            // Add to Selected Filters if the value is non-null and not already on the list
            if (!SelectedVOBFilters.Contains(item) && item != null)
            {
                SelectedVOBFilters.Add(item);
                OnPropertyChanged(nameof(SelectedVOBFilters));
            }
        }
        public void RemoveFromSelectedVOBFilters(ClearcaseVOBItem item)
        {
            // Remove From Selected Filters if the value is on the list
            if (SelectedVOBFilters.Contains(item))
            {
                SelectedVOBFilters.Remove(item);
                OnPropertyChanged(nameof(SelectedVOBFilters));
            }
        }

        private ObservableCollection<ClearcaseVOBItem> m_selectedVOBFilters;
        public ObservableCollection<ClearcaseVOBItem> SelectedVOBFilters
        {
            get { return m_selectedVOBFilters; }

            set
            {
                m_selectedVOBFilters = value;
                OnPropertyChanged(nameof(SelectedVOBFilters));
            }
        }
        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseViewTabCodeViewModel
        // Method      : UpdateSearchSourceResults
        // Description : Updates the SearchSourceFiltered ObservableCollection with the latest
        //               detected search term and enabled filters.
        // ----------------------------------------------------------------------------------------
        public void UpdateSearchSourceResults()
        {
            // Perform the update under lock, in case other processing is also using search results
            RequestLock();

            // Deselect currently selected item(s) before processing new search
            DeselectAllSelectedItems();

            // Clear current search returns before processing new search
            UnfilteredSearchSourceResults.Clear();

            // Get new search results based on new search term (if any)
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var results = Search(m_searchTerm);

                if (results.Any())
                {
                    UnfilteredSearchSourceResults = new ObservableCollection<ClearcaseProjectSourceItem>(results.ToList());

                    // Default top result as selection
                    UnfilteredSearchSourceResults.First().IsSelected = true;
                }
            }

            ReleaseLock();
        }
        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseManagerViewModel
        // Method      : Search
        // Description : Returns a subset of items matching the search term.
        // Parameters  :
        // - searchTerm (string) : Input search term
        // ----------------------------------------------------------------------------------------
        public IEnumerable<ClearcaseProjectSourceItem> Search(string searchTerm)
        {
            return m_unfilteredSearchSource.Where(item => item.DirInfo.Name.ToLower().Contains(searchTerm.ToLower()))
                                           .OrderBy(x => x.DirInfo.Name);
        }
        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseManagerViewModel
        // Method      : SelectedItems
        // Description : Returns a subset of items which are currently selected.
        // ----------------------------------------------------------------------------------------
        public IEnumerable<ClearcaseProjectSourceItem> SelectedItems()
        {
            return m_unfilteredSearchSource.Where(item => item.IsSelected);
        }
        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseManagerViewModel
        // Method      : DeselectAllSelectedItems
        // Description : Deselects all items which are currently selected.
        // ----------------------------------------------------------------------------------------
        public void DeselectAllSelectedItems()
        {
            foreach (ClearcaseProjectSourceItem selectedItem in SelectedItems())
            {
                selectedItem.IsSelected = false;
            }
        }
        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseViewTabCodeViewModel
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
                await ProcessUpdateQueuePeriodically(TimeSpan.FromSeconds(1), PeriodicUpdateCancellationToken);
            }
            catch
            {
                cancellationTokenSource.Dispose();
            }
        }
        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseViewTabCodeViewModel
        // Method      : StopAutoRefresh
        // Description : Requests a cancellation to be processed on the next asynchronous action.
        // ----------------------------------------------------------------------------------------
        public void StopAutoRefresh()
        {
            cancellationTokenSource.Cancel();
        }
        // ----------------------------------------------------------------------------------------
        // Class       : ProcessUpdateQueuePeriodically
        // Method      : RefreshClearcaseViewsPeriodically
        // Description : This asynchronous Task processes the UpdateQueue periodically. 
        // ----------------------------------------------------------------------------------------
        public async Task ProcessUpdateQueuePeriodically(TimeSpan interval, CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    ProcessUpdateQueue();
                    await Task.Delay(interval, cancellationToken);
                }
                catch
                {
                    throw;
                }
            }
        }
        public void ProcessUpdateQueue()
        {
            while (ClearcaseProjectSourceModel.GetUpdateQueueCountUnderLock() > 0)
            {
                // Consume next queue item
                UpdateQueueItem item = ClearcaseProjectSourceModel.DequeueUnderLock();

                switch (item.WatcherChangeType)
                {
                    case (WatcherChangeTypes.All)     : UnfilteredSearchSource.Add(item.ProjectSourceItem); break; // INITIALIZATION CASE
                    case (WatcherChangeTypes.Created) : ProcessCreatedItem(item.ProjectSourceItem);         break; // CREATED CASE
                    case (WatcherChangeTypes.Deleted) : ProcessDeletedItem(item.ProjectSourceItem);         break; // DELETED CASE
                    case (WatcherChangeTypes.Changed) : ProcessChangedItem(item.ProjectSourceItem);         break; // CHANGED CASE
                }
            }
        }
        public void ProcessCreatedItem(ClearcaseProjectSourceItem item)
        {
            UnfilteredSearchSource.Add(item);
            UpdateSearchSourceResults(); // Update results accordingly
        }
        public void ProcessDeletedItem(ClearcaseProjectSourceItem item)
        {
            // Reference : https://stackoverflow.com/questions/20403162/remove-one-item-in-observablecollection
            var result = UnfilteredSearchSource.Remove(UnfilteredSearchSource.Where(i => i.DirInfo.Name.Equals(item.DirInfo.Name)).Single());
            UpdateSearchSourceResults(); // Update results accordingly
        }
        public void ProcessChangedItem(ClearcaseProjectSourceItem item)
        {
            // TODO - figure out what to do here
            UpdateSearchSourceResults(); // Update results accordingly
        }
        // ----------------------------------------------------------------------------------------
        // Commands
        // ----------------------------------------------------------------------------------------
        private RelayCommand _editCommand;
        public RelayCommand EditCommand => _editCommand ?? (_editCommand = new RelayCommand(param => EditCommandProcessing()));

        private void EditCommandProcessing()
        {
            foreach (ClearcaseProjectSourceItem item in SelectedItems())
            {
                try
                {
                    Process.Start(item.DirInfo.FullName);
                }
                catch
                {
                    // Default to notepad++ if an error is thrown (assumes you have at least notepad++ installed)
                    Process.Start("notepad++.exe", item.DirInfo.FullName);
                }
            }
        }
    }

    public class ClearcaseProjectSourceItem 
    {
        public bool IsSelected { get; set; }
        public DirectoryInfo DirInfo { get; set; }
        public SOURCE_CONTROL_STATUS SourceControlStatus { get; set; }
        public int SourceVersion { get; set; }

        public ClearcaseProjectSourceItem()
        {
            IsSelected = false;
            SourceVersion = 0;
        }
    }

    public enum SOURCE_CONTROL_STATUS
    {
        NOT_IN_SOURCE_CONTROL,
        IN_SOURCE_CONTROL,
        CHECKED_OUT_RESERVED,
        CHECKED_OUT_UNRESERVED,
    }
    public class ClearcaseVOBItem
    {
        public string Name { get; set; }
        public ClearcaseVOBItem() { }
    }

    public class ClearcaseVOBAutocompleteSource : IAutocompleteSource
    {
        private readonly List<ClearcaseVOBItem> m_clearcaseVOBItems;
        public ClearcaseVOBAutocompleteSource()
        {
            m_clearcaseVOBItems = new List<ClearcaseVOBItem>()
            {
                new ClearcaseVOBItem() { Name = "Android Gingerbread" },
                new ClearcaseVOBItem() { Name = "Android Icecream Sandwich" },
                new ClearcaseVOBItem() { Name = "Android Jellybean" },
                new ClearcaseVOBItem() { Name = "Android Lollipop" },
                new ClearcaseVOBItem() { Name = "Android Nougat" },
                new ClearcaseVOBItem() { Name = "Debian" },
                new ClearcaseVOBItem() { Name = "Mac OSX" },
                new ClearcaseVOBItem() { Name = "Raspbian" },
                new ClearcaseVOBItem() { Name = "Ubuntu Wily Werewolf" },
                new ClearcaseVOBItem() { Name = "Ubuntu Xenial Xerus" },
                new ClearcaseVOBItem() { Name = "Ubuntu Yakkety Yak" },
                new ClearcaseVOBItem() { Name = "Ubuntu Zesty Zapus" },
                new ClearcaseVOBItem() { Name = "Windows 7" },
                new ClearcaseVOBItem() { Name = "Windows 8" },
                new ClearcaseVOBItem() { Name = "Windows 10" },
                new ClearcaseVOBItem() { Name = "Windows Vista" },
                new ClearcaseVOBItem() { Name = "Windows XP" }
            };
        }

        public IEnumerable Search(string searchTerm)
        {
            searchTerm = searchTerm ?? string.Empty;
            searchTerm = searchTerm.ToLower();

            return m_clearcaseVOBItems.Where(item => item.Name.ToLower().Contains(searchTerm));
        }
    }
}
