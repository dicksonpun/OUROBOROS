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
            m_unfilteredSearchSource = new ObservableCollection<ClearcaseManagerItem>() { };
            m_unfilteredSearchSourceResults = new ObservableCollection<ClearcaseManagerItem>() { };

            // Initialize dummy views
            m_unfilteredSearchSource.Add(new ClearcaseManagerItem() { ViewName = "Dickson-shortview1" });
            m_unfilteredSearchSource.Add(new ClearcaseManagerItem() { ViewName = "Dickson-shortview2" });
            m_unfilteredSearchSource.Add(new ClearcaseManagerItem() { ViewName = "dickson-shortview3" });
            m_unfilteredSearchSource.Add(new ClearcaseManagerItem() { ViewName = "dickson-shortview4" });
            m_unfilteredSearchSource.Add(new ClearcaseManagerItem() { ViewName = "randomusername-shortview5" });
            m_unfilteredSearchSource.Add(new ClearcaseManagerItem() { ViewName = "randomusername-shortview6" });
            m_unfilteredSearchSource.Add(new ClearcaseManagerItem() { ViewName = "randomusername-shortview7" });
            m_unfilteredSearchSource.Add(new ClearcaseManagerItem() { ViewName = "Dickson-veryverylongview1" });
            m_unfilteredSearchSource.Add(new ClearcaseManagerItem() { ViewName = "Dickson-veryverylongview2" });
            m_unfilteredSearchSource.Add(new ClearcaseManagerItem() { ViewName = "randomusername-veryverylongview3" });
            m_unfilteredSearchSource.Add(new ClearcaseManagerItem() { ViewName = "randomusername-veryverylongview4" });
            m_unfilteredSearchSource.Add(new ClearcaseManagerItem() { ViewName = "randomusername-veryverylongview5" });
            m_unfilteredSearchSource.Add(new ClearcaseManagerItem() { ViewName = "randomusername-veryverylongview6" });
            m_unfilteredSearchSource.Add(new ClearcaseManagerItem() { ViewName = "randomusername-veryverylongview7" });
            m_unfilteredSearchSource.Add(new ClearcaseManagerItem() { ViewName = "randomusername-veryverylongview8" });
            m_unfilteredSearchSource.Add(new ClearcaseManagerItem() { ViewName = "_" });
            m_unfilteredSearchSource.Add(new ClearcaseManagerItem() { ViewName = "admin" });
            m_unfilteredSearchSource.Add(new ClearcaseManagerItem() { ViewName = "nonsenseViewName-asfggdds23" });

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
        private ObservableCollection<ClearcaseManagerItem> m_unfilteredSearchSource;
        public ObservableCollection<ClearcaseManagerItem> UnfilteredSearchSource
        {
            get { return m_unfilteredSearchSource; }
            set
            {
                m_unfilteredSearchSource = value;
                OnPropertyChanged(nameof(UnfilteredSearchSource));
            }
        }
        private ObservableCollection<ClearcaseManagerItem> m_unfilteredSearchSourceResults;
        public ObservableCollection<ClearcaseManagerItem> UnfilteredSearchSourceResults
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
                    UnfilteredSearchSourceResults = new ObservableCollection<ClearcaseManagerItem>(results.ToList());

                    // Default top result as selection
                    UnfilteredSearchSourceResults.First().IsSelected = true;
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
            return m_unfilteredSearchSource.Where(item => item.ViewName.ToLower().Contains(searchTerm.ToLower()))
                                           .OrderBy(x => x.ViewName);
        }
        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseManagerViewModel
        // Method      : SelectedItems
        // Description : Returns a subset of items which are currently selected.
        // ----------------------------------------------------------------------------------------
        public IEnumerable<ClearcaseManagerItem> SelectedItems()
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
