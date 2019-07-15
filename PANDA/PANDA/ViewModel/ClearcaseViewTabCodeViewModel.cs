using MaterialDesignThemes.Wpf;
using PANDA.Command;
using PANDA.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PANDA.ViewModel
{
    public class ClearcaseViewTabCodeViewModel : ViewModel
    {
        // SearchSourceUnfiltered Databinding
        private ObservableCollection<ProjectSourceInfoItem> m_searchSourceUnfiltered;
        public ObservableCollection<ProjectSourceInfoItem> SearchSourceUnfiltered
        {
            get { return m_searchSourceUnfiltered; }
            set
            {
                m_searchSourceUnfiltered = value;
                // Raises PropertyChanged events on the properties
                OnPropertyChanged(nameof(SearchSourceUnfiltered));
            }
        }

        // SearchSourceFiltered Databinding
        private ObservableCollection<ProjectSourceInfoItem> m_searchSourceFiltered;
        public ObservableCollection<ProjectSourceInfoItem> SearchSourceFiltered
        {
            get { return m_searchSourceFiltered; }
            set
            {
                m_searchSourceFiltered = value;
                // Raises PropertyChanged events on the properties
                OnPropertyChanged(nameof(SearchSourceFiltered));
            }
        }

        // SearchTerm Databinding
        private string m_searchTerm;
        public string SearchTerm
        {
            get { return m_searchTerm; }

            set
            {
                m_searchTerm = value;
                OnPropertyChanged(nameof(SearchTerm));

                // Update source accordingly
                UpdateSearchSourceFiltered();
            }
        }

        // Members
        private ProjectSourceHelper m_projectSourceHelper;
        private string m_username;
        private readonly string m_viewPath;
        private bool m_isUserView;

        // Constructor
        public ClearcaseViewTabCodeViewModel(string viewPath, bool isUserView, string username) : base()
        {
            m_viewPath   = viewPath;
            m_isUserView = isUserView;
            m_username   = username;

            SearchSourceUnfiltered = new ObservableCollection<ProjectSourceInfoItem>();
            SearchSourceFiltered   = new ObservableCollection<ProjectSourceInfoItem>();
            m_projectSourceHelper  = new ProjectSourceHelper(this, viewPath);
        }

        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseViewTabCodeViewModel
        // Method      : UpdateSearchSourceFiltered
        // Description : Updates the SearchSourceFiltered ObservableCollection with the latest
        //               detected search term and enabled filters.
        // ----------------------------------------------------------------------------------------
        public void UpdateSearchSourceFiltered()
        {
            // Clear currently selected item(s) before processing new search
            UnselectAllSelectedItems();

            // Clear current search returns before processing new search
            SearchSourceFiltered.Clear();

            // Get new search results based on new search term (if any)
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var results = Search(m_searchTerm);

                if (results.Any())
                {
                    SearchSourceFiltered = new ObservableCollection<ProjectSourceInfoItem>(results.ToList());

                    // Default top result as selection
                    SearchSourceFiltered.First().IsSelected = true;
                }
            }
        }

        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseViewTabCodeViewModel
        // Method      : Search
        // Description : Returns a subset of items matching the search term.
        // Parameters  :
        // - searchTerm (string) : Input search term
        // ----------------------------------------------------------------------------------------
        public IEnumerable<ProjectSourceInfoItem> Search(string searchTerm)
        {
            return m_searchSourceUnfiltered.Where(item => item.DirInfo.Name.ToLower().Contains(searchTerm.ToLower()))
                                           .OrderBy(x => x.DirInfo.Name);
        }

        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseViewTabCodeViewModel
        // Method      : SelectedItems
        // Description : Returns a subset of items which are currently selected.
        // ----------------------------------------------------------------------------------------
        public IEnumerable<ProjectSourceInfoItem> SelectedItems()
        {
            return m_searchSourceFiltered.Where(item => item.IsSelected);
        }

        // ----------------------------------------------------------------------------------------
        // Class       : ClearcaseViewTabCodeViewModel
        // Method      : UnselectAllSelectedItems
        // Description : Unselects all items which are currently selected.
        // ----------------------------------------------------------------------------------------
        public void UnselectAllSelectedItems()
        {
            foreach (ProjectSourceInfoItem selectedItem in SelectedItems())
            {
                selectedItem.IsSelected = false;
            }
        }

        // Commands
        private RelayCommand _editCommand;
        public RelayCommand EditCommand => _editCommand ?? (_editCommand = new RelayCommand(param => EditCommandProcessing()));

        private void EditCommandProcessing()
        {
            foreach (ProjectSourceInfoItem item in SelectedItems())
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
}

    public enum SOURCE_CONTROLLED_STATUS
    {
        SOURCE_CONTROLLED,
        NOT_SOURCE_CONTROLLED,
        CHECKED_IN,
        CHECKED_OUT_RESERVED,
        CHECKED_OUT_UNRESERVED,
    }

    public class ProjectSourceInfoItem : ViewModel
    {
        public bool IsSelected { get; set; }
        public DirectoryInfo DirInfo { get; set; }
        public string VOBFullPath { get; set; }
        public string VOBParentPath { get; set; }
        public PackIconKind Icon { get; set; }

        // Placeholder items
        public SOURCE_CONTROLLED_STATUS SourceControlledStatus { get; set; }
        public int SourceVersion { get; set; }
        public ProjectSourceInfoItem() { }
    }
