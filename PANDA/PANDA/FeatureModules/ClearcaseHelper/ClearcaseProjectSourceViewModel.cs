using PANDA.Command;
using PANDA.Model;
using System;
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
            m_unfilteredSearchSource = new ObservableCollection<ClearcaseProjectSourceItem>() { };
            m_unfilteredSearchSourceResults = new ObservableCollection<ClearcaseProjectSourceItem>() { };

            // Initialize dummy files 

            // Default search term to empty
            SearchTerm = string.Empty;

            ClearcaseProjectSourceModel = new ClearcaseProjectSourceModel(viewName);
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
                UpdateQueueItem item = ClearcaseProjectSourceModel.DequeueUpdateQueueUnderLock();

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

    public class ClearcaseProjectSourceItem : ViewModel
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
 
}
