using MaterialDesignExtensions.Model;
using OUROBOROS.Command;
using OUROBOROS.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static OUROBOROS.Model.ClearcaseProjectSourceModel;

namespace OUROBOROS.ViewModel
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
            m_searchSource = new ObservableCollection<ClearcaseProjectSourceItem>() { };
            m_searchResults = new ObservableCollection<ClearcaseProjectSourceItem>() { };
            SearchTerm = string.Empty; // NOTE: Set after initializing SearchSource(s) to avoid null reference.
            ClearcaseProjectSourceModel = new ClearcaseProjectSourceModel(viewName);

            // VOB Filter
            m_selectedVOB = null;
            m_VOBAutocompleteSource = new ClearcaseVOBAutocompleteSource();
            m_selectedVOBs = new ObservableCollection<ClearcaseVOBItem>();

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

        private ObservableCollection<ClearcaseProjectSourceItem> m_searchSource;
        public ObservableCollection<ClearcaseProjectSourceItem> SearchSource
        {
            get { return m_searchSource; }
            set
            {
                m_searchSource = value;
                OnPropertyChanged(nameof(SearchSource));
            }
        }

        private ObservableCollection<ClearcaseProjectSourceItem> m_searchResults;
        public ObservableCollection<ClearcaseProjectSourceItem> SearchResults
        {
            get { return m_searchResults; }
            set
            {
                m_searchResults = value;
                OnPropertyChanged(nameof(SearchResults));
            }
        }

        // VOB-specific Databinding
        private IAutocompleteSource m_VOBAutocompleteSource;
        public IAutocompleteSource VOBAutocompleteSource
        {
            get { return m_VOBAutocompleteSource; }
        }

        private ClearcaseVOBItem m_selectedVOB;
        public ClearcaseVOBItem SelectedVOB
        {
            get { return m_selectedVOB; }

            set
            {
                m_selectedVOB = value;
                OnPropertyChanged(nameof(SelectedVOB));

                AddToSelectedVOBs(value);
            }
        }

        private ObservableCollection<ClearcaseVOBItem> m_selectedVOBs;
        public ObservableCollection<ClearcaseVOBItem> SelectedVOBs
        {
            get { return m_selectedVOBs; }
        }

        // VOB Helper functions
        public void AddToSelectedVOBs(ClearcaseVOBItem item)
        {
            // Add to SelectedVOBs if the value is non-null and not already on the list
            if (!SelectedVOBs.Contains(item) && item != null)
            {
                SelectedVOBs.Add(item);
                OnPropertyChanged(nameof(SelectedVOBs));
                UpdateSearchSourceResults(); // Update results accordingly
            }
        }
        public void RemoveFromSelectedVOBs(ClearcaseVOBItem item)
        {
            // Remove From SelectedVOBs if the value is on the list
            if (SelectedVOBs.Contains(item))
            {
                SelectedVOBs.Remove(item);
                OnPropertyChanged(nameof(SelectedVOBs));
                UpdateSearchSourceResults(); // Update results accordingly
            }
        }

        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseProjectSourceViewModel
        // Method      : UpdateSearchSourceResults
        // Description : Updates the SearchResults ObservableCollection with the latest
        //               detected search term and enabled filters.
        // ----------------------------------------------------------------------------------------
        public void UpdateSearchSourceResults()
        {
            // Perform the update under lock, in case other processing is also using search results
            RequestLock();

            // Deselect currently selected item(s) before processing new search
            DeselectAllSelectedItems();

            // Clear current search returns before processing new search
            SearchResults.Clear();

            // Get new search results based on new search term (if any)
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var results = Search(m_searchTerm);

                if (results.Any())
                {
                    SearchResults = new ObservableCollection<ClearcaseProjectSourceItem>(results.ToList());

                    // Default top result as selection
                    SearchResults.First().IsSelected = true;
                }
            }

            ReleaseLock();
        }
        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseProjectSourceViewModel
        // Method      : Search
        // Description : Returns a subset of items matching the search term.
        // Parameters  :
        // - searchTerm (string) : Input search term
        // ----------------------------------------------------------------------------------------
        public IEnumerable<ClearcaseProjectSourceItem> Search(string searchTerm)
        {
            return m_searchSource.Where(item => item.DirInfo.Name.ToLower().Contains(searchTerm.ToLower())
                                        && SelectedVOBs.Any(vob => item.DirInfo.FullName.ToLower().Contains(vob.Name.ToLower())))
                                 .OrderBy(x => x.DirInfo.Name);
        }
        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseProjectSourceViewModel
        // Method      : SelectedItems
        // Description : Returns a subset of items which are currently selected.
        // ----------------------------------------------------------------------------------------
        public IEnumerable<ClearcaseProjectSourceItem> SelectedItems()
        {
            return m_searchSource.Where(item => item.IsSelected);
        }
        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseProjectSourceViewModel
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
        // Class       : ClearcaseProjectSourceViewModel
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
            catch (OperationCanceledException)
            {
                cancellationTokenSource.Dispose();
            }
        }
        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseProjectSourceViewModel
        // Method      : StopAutoRefresh
        // Description : Requests a cancellation to be processed on the next asynchronous action.
        // ----------------------------------------------------------------------------------------
        public void StopAutoRefresh()
        {
            cancellationTokenSource.Cancel();
        }
        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseProjectSourceViewModel
        // Method      : ProcessUpdateQueuePeriodically
        // Description : This asynchronous Task processes the UpdateQueue periodically. 
        // ----------------------------------------------------------------------------------------
        public async Task ProcessUpdateQueuePeriodically(TimeSpan interval, CancellationToken cancellationToken)
        {
            while (true)
            {
                ProcessUpdateQueue();
                await Task.Delay(interval, cancellationToken);
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
                    case (WatcherChangeTypes.All): SearchSource.Add(item.ProjectSourceItem); break; // INITIALIZATION CASE
                    case (WatcherChangeTypes.Created): ProcessCreatedItem(item.ProjectSourceItem); break; // CREATED CASE
                    case (WatcherChangeTypes.Deleted): ProcessDeletedItem(item.ProjectSourceItem); break; // DELETED CASE
                    case (WatcherChangeTypes.Changed): ProcessChangedItem(item.ProjectSourceItem); break; // CHANGED CASE
                }
            }
        }
        public void ProcessCreatedItem(ClearcaseProjectSourceItem item)
        {
            SearchSource.Add(item);
            UpdateSearchSourceResults(); // Update results accordingly
        }
        public void ProcessDeletedItem(ClearcaseProjectSourceItem item)
        {
            // Reference : https://stackoverflow.com/questions/20403162/remove-one-item-in-observablecollection 
            var result = SearchSource.Remove(SearchSource.FirstOrDefault(i => i.DirInfo.Name.Equals(item.DirInfo.Name)));
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
                new ClearcaseVOBItem() { Name = "calculator-master" },
                new ClearcaseVOBItem() { Name = "evt-master" },
                new ClearcaseVOBItem() { Name = "GTS-GamesTaskScheduler-master" },
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
