using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PANDA.ViewModel
{
    public class ClearcaseManagerViewModel : ViewModel
    {
        // Constructor
        public ClearcaseManagerViewModel()
        {
            m_searchSource = new ObservableCollection<ClearcaseManagerItem>() { };
            m_searchResults = new ObservableCollection<ClearcaseManagerItem>() { };

            // Initialize dummy views
            m_searchSource.Add(new ClearcaseManagerItem() { ViewName = "Dickson-shortview1" });
            m_searchSource.Add(new ClearcaseManagerItem() { ViewName = "Dickson-shortview2" });
            m_searchSource.Add(new ClearcaseManagerItem() { ViewName = "dickson-shortview3" });
            m_searchSource.Add(new ClearcaseManagerItem() { ViewName = "dickson-shortview4" });
            m_searchSource.Add(new ClearcaseManagerItem() { ViewName = "randomusername-shortview5" });
            m_searchSource.Add(new ClearcaseManagerItem() { ViewName = "randomusername-shortview6" });
            m_searchSource.Add(new ClearcaseManagerItem() { ViewName = "randomusername-shortview7" });
            m_searchSource.Add(new ClearcaseManagerItem() { ViewName = "Dickson-veryverylongview1" });
            m_searchSource.Add(new ClearcaseManagerItem() { ViewName = "Dickson-veryverylongview2" });
            m_searchSource.Add(new ClearcaseManagerItem() { ViewName = "randomusername-veryverylongview3" });
            m_searchSource.Add(new ClearcaseManagerItem() { ViewName = "randomusername-veryverylongview4" });
            m_searchSource.Add(new ClearcaseManagerItem() { ViewName = "randomusername-veryverylongview5" });
            m_searchSource.Add(new ClearcaseManagerItem() { ViewName = "randomusername-veryverylongview6" });
            m_searchSource.Add(new ClearcaseManagerItem() { ViewName = "randomusername-veryverylongview7" });
            m_searchSource.Add(new ClearcaseManagerItem() { ViewName = "randomusername-veryverylongview8" });
            m_searchSource.Add(new ClearcaseManagerItem() { ViewName = "_abc" });
            m_searchSource.Add(new ClearcaseManagerItem() { ViewName = "admin222" });
            m_searchSource.Add(new ClearcaseManagerItem() { ViewName = "nonsenseViewName-asfggdds23" });

            // Default search term to username
            SearchTerm = "dickson";
        }

        // Databinding
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
        private ObservableCollection<ClearcaseManagerItem> m_searchSource;
        public ObservableCollection<ClearcaseManagerItem> SearchSource
        {
            get { return m_searchSource; }
            set
            {
                m_searchSource = value;
                OnPropertyChanged(nameof(SearchSource));
            }
        }
        private ObservableCollection<ClearcaseManagerItem> m_searchResults;
        public ObservableCollection<ClearcaseManagerItem> SearchResults
        {
            get { return m_searchResults; }
            set
            {
                m_searchResults = value;
                OnPropertyChanged(nameof(SearchResults));
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
                    SearchResults = new ObservableCollection<ClearcaseManagerItem>(results.ToList());

                    // Default top result as selection
                    SearchResults.First().IsSelected = true;
                }
            }
        }

        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseManagerViewModel
        // Method      : Search
        // Description : Returns a subset of items matching the search term.
        // Parameters  :
        // - searchTerm (string) : Input search term
        // ----------------------------------------------------------------------------------------
        public IEnumerable<ClearcaseManagerItem> Search(string searchTerm)
        {
            return m_searchSource.Where(item => item.ViewName.ToLower().Contains(searchTerm.ToLower()))
                                           .OrderBy(x => x.ViewName);
        }
        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseManagerViewModel
        // Method      : SelectedItems
        // Description : Returns a subset of items which are currently selected.
        // ----------------------------------------------------------------------------------------
        public IEnumerable<ClearcaseManagerItem> SelectedItems()
        {
            return m_searchSource.Where(item => item.IsSelected);
        }
        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseManagerViewModel
        // Method      : DeselectAllSelectedItems
        // Description : Deselects all items which are currently selected.
        // ----------------------------------------------------------------------------------------
        public void DeselectAllSelectedItems()
        {
            foreach (ClearcaseManagerItem selectedItem in SelectedItems())
            {
                selectedItem.IsSelected = false;
            }
        }
    }

    public class ClearcaseManagerItem : ViewModel
    {
        public string ViewName { get; set; }
        public List<string> DefaultVOBs { get; set; }
        public bool IsSelected { get; set; }
        public bool IsActivated { get; set; }
        public bool IsMountedToYDrive { get; set; }

        public ClearcaseManagerItem()
        {
            DefaultVOBs = new List<string>();
            IsSelected = false;
            IsActivated = false;
            IsMountedToYDrive = false;
        }
    }
}
