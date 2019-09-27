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
using static OUROBOROS.Model.ProjectHelperModel;

namespace OUROBOROS.ViewModel
{
    public class ProjectHelperViewModel : ViewModel
    {
        #region Members

        public ProjectHelperModel ProjectHelperModel;
        private CancellationTokenSource cancellationTokenSource;

        private static readonly SemaphoreSlim m_SearchResultsMutex = new SemaphoreSlim(1, 1);
        public async void RequestLock() { await m_SearchResultsMutex.WaitAsync(); }
        public void ReleaseLock() { m_SearchResultsMutex.Release(); }

        #endregion


        #region Constructor
 
        public ProjectHelperViewModel()
        {
            ProjectHelperModel = new ProjectHelperModel();

            // Project Source Search
            m_searchSource = new ObservableCollection<ProjectHelperSourceItem>() { };
            m_searchResults = new ObservableCollection<ProjectHelperSourceItem>() { };
            SearchTerm = string.Empty; // NOTE: Set after initializing SearchSource(s) to avoid null reference.

            // VOB
            m_selectedVOB = null;
            m_selectedVOBs = new ObservableCollection<VOBItem>();
            m_VOBAutocompleteSource = new VOBAutocompleteSource();

            // View
            m_currentView = string.Empty;
            m_selectedView = null;
            m_ViewAutocompleteSource = new ViewAutocompleteSource();

            // Register FileSystemWatcher and auto-refresh processing
            StartAutoRefresh();
        }

        #endregion


        #region Databinding

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

        private ObservableCollection<ProjectHelperSourceItem> m_searchSource;
        public ObservableCollection<ProjectHelperSourceItem> SearchSource
        {
            get { return m_searchSource; }
            set
            {
                m_searchSource = value;
                OnPropertyChanged(nameof(SearchSource));
            }
        }

        private ObservableCollection<ProjectHelperSourceItem> m_searchResults;
        public ObservableCollection<ProjectHelperSourceItem> SearchResults
        {
            get { return m_searchResults; }
            set
            {
                m_searchResults = value;
                OnPropertyChanged(nameof(SearchResults));
            }
        }
 
        private IAutocompleteSource m_VOBAutocompleteSource;
        public IAutocompleteSource VOBAutocompleteSource
        {
            get { return m_VOBAutocompleteSource; }
            set
            {
                if (value != null)
                {
                    m_VOBAutocompleteSource = value;
                    OnPropertyChanged(nameof(VOBAutocompleteSource));
                }
            }
        }

        private VOBItem m_selectedVOB;
        public VOBItem SelectedVOB
        {
            get { return m_selectedVOB; }

            set
            {
                m_selectedVOB = value;
                OnPropertyChanged(nameof(SelectedVOB));

                AddToSelectedVOBs(value);
            }
        }

        private ObservableCollection<VOBItem> m_selectedVOBs;
        public ObservableCollection<VOBItem> SelectedVOBs
        {
            get { return m_selectedVOBs; }
            set
            {
                m_selectedVOBs = value;
                OnPropertyChanged(nameof(SelectedVOBs));
                UpdateSearchSourceResults(); // Update results accordingly
            }
        }

        private string m_currentView;
        public string CurrentView
        {
            get { return m_currentView; }
            set
            {
                m_currentView = value;
                OnPropertyChanged(nameof(CurrentView)); 
            }
        }
        
        private DirectoryInfo m_selectedView;
        public DirectoryInfo SelectedView
        {
            get { return m_selectedView; }
            set
            {
                m_selectedView = value;
                OnPropertyChanged(nameof(SelectedView));
                SetupNewView(SelectedView);
            }
        }

        private IAutocompleteSource m_ViewAutocompleteSource;
        public IAutocompleteSource ViewAutocompleteSource
        {
            get { return m_ViewAutocompleteSource; }
            set
            {
                if (value != null)
                {
                    m_ViewAutocompleteSource = value;
                    OnPropertyChanged(nameof(ViewAutocompleteSource));
                }
            }
        }

        #endregion


        #region Methods

        public void SetupNewView(DirectoryInfo item)
        {
            RequestLock();
            CurrentView = item.Name;
            SearchSource = new ObservableCollection<ProjectHelperSourceItem>() { };
            SearchResults = new ObservableCollection<ProjectHelperSourceItem>() { };
            ProjectHelperModel = new ProjectHelperModel(item.FullName);
            ReleaseLock();
        }

        public void AddToSelectedVOBs(VOBItem item)
        {
            // Add to SelectedVOBs if the value is non-null and not already on the list
            if (!SelectedVOBs.Contains(item) && item != null)
            {
                SelectedVOBs.Add(item);
                OnPropertyChanged(nameof(SelectedVOBs));
                UpdateSearchSourceResults(); // Update results accordingly
            }
        }
        public void RemoveFromSelectedVOBs(VOBItem item)
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
        // Class       : ProjectHelperViewModel
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
                    SearchResults = new ObservableCollection<ProjectHelperSourceItem>(results.ToList());

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
        public IEnumerable<ProjectHelperSourceItem> Search(string searchTerm)
        {
            string viewRootPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + CurrentView + @"\";

            return m_searchSource.Where(item => item.DirInfo.Name.ToLower().Contains(searchTerm.ToLower())
                                        && SelectedVOBs.Any(vob => item.DirInfo.FullName.ToLower()
                                                                  .Contains(viewRootPath.ToLower() + vob.Name.ToLower())))
                                 .OrderBy(x => x.DirInfo.Name);
        }
        // ----------------------------------------------------------------------------------------
        // Method      : SelectedItems
        // Description : Returns a subset of items which are currently selected.
        // ----------------------------------------------------------------------------------------
        public IEnumerable<ProjectHelperSourceItem> SelectedItems()
        {
            return m_searchSource.Where(item => item.IsSelected);
        }
        // ----------------------------------------------------------------------------------------
        // Method      : DeselectAllSelectedItems
        // Description : Deselects all items which are currently selected.
        // ----------------------------------------------------------------------------------------
        public void DeselectAllSelectedItems()
        {
            foreach (ProjectHelperSourceItem selectedItem in SelectedItems())
            {
                selectedItem.IsSelected = false;
            }
        }
        // ----------------------------------------------------------------------------------------
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
            bool processedUpdates = false;
            while (ProjectHelperModel.UpdateQueue.Any())
            {
                // Consume next queue item
                UpdateQueueItem item = ProjectHelperModel.UpdateQueue.Dequeue();

                switch (item.WatcherChangeType)
                {
                    case (WatcherChangeTypes.All): ProcessInitializedItem(item.ProjectSourceItem); break; // INITIALIZATION CASE
                    case (WatcherChangeTypes.Created): ProcessCreatedItem(item.ProjectSourceItem); break; // CREATED CASE
                    case (WatcherChangeTypes.Deleted): ProcessDeletedItem(item.ProjectSourceItem); break; // DELETED CASE
                    case (WatcherChangeTypes.Changed): ProcessChangedItem(item.ProjectSourceItem); break; // CHANGED CASE
                }
                processedUpdates = true;
            }

            // Only refresh the UI if updated
            if (processedUpdates)
            {
                UpdateSearchSourceResults(); // Update results accordingly
            }
        }
        public void ProcessInitializedItem(ProjectHelperSourceItem item)
        {
            SearchSource.Add(item);
        }
        public void ProcessCreatedItem(ProjectHelperSourceItem item)
        {
            SearchSource.Add(item);
        }
        public void ProcessDeletedItem(ProjectHelperSourceItem item)
        {
            // Reference : https://stackoverflow.com/questions/20403162/remove-one-item-in-observablecollection
            var result = SearchSource.Remove(SearchSource.FirstOrDefault(i => i.DirInfo.Name.Equals(item.DirInfo.Name)));
        }
        public void ProcessChangedItem(ProjectHelperSourceItem item)
        {
            // TODO - figure out what to do here
        }

        #endregion


        #region Relay Commands

        private RelayCommand _openCommand;
        public RelayCommand OpenCommand => _openCommand ?? (_openCommand = new RelayCommand(param => OpenCommandProcessing()));

        private void OpenCommandProcessing()
        {
            foreach (ProjectHelperSourceItem item in SelectedItems())
            {
                try
                {
                    // When you perform Process.Start(filename), it actually looks for the associated application for the extension.
                    // If it does not find the association, open it in notepad++.
                    Process.Start(item.DirInfo.FullName);
                }
                catch
                {
                    // Default to notepad++ if an error is thrown (assumes you have at least notepad++ installed)
                    // "Exception thrown: 'System.ComponentModel.Win32Exception' in System.dll" is expected.
                    // Don't bother logging this error.
                    Process.Start("notepad++.exe", item.DirInfo.FullName);
                }
            }
        }

        #endregion

    }
}
