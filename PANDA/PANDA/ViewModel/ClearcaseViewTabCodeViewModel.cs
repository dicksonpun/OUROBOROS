using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;
using PANDA;
using PANDA.Command;
using PANDA.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace PANDA.ViewModel
{
    public class ClearcaseViewTabCodeViewModel : ViewModel
    {
        // Members
        private ProjectSourceHelper m_projectSourceHelper;
        private string m_username = "dickson"; // TODO: NEED to build user settings profile
        private readonly string m_viewPath;
        private bool m_isUserView;

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

        // SearchResultsVisibility Databinding
        private object m_searchResultsVisibility;
        public object SearchResultsVisibility
        {
            get { return m_searchResultsVisibility; }

            set
            {
                m_searchResultsVisibility = value;
                OnPropertyChanged(nameof(SearchResultsVisibility));
            }
        }

        // Constructor
        public ClearcaseViewTabCodeViewModel(string viewPath, bool isUserView) : base()
        {
            m_searchResultsVisibility = null;
            m_viewPath = viewPath;
            m_isUserView = isUserView;

            SearchSourceUnfiltered = new ObservableCollection<ProjectSourceInfoItem>();
            SearchSourceFiltered   = new ObservableCollection<ProjectSourceInfoItem>();
            m_projectSourceHelper  = new ProjectSourceHelper(this, viewPath);
        }

        public void UpdateSearchSourceFiltered()
        {
            SearchResultsVisibility = null;
            SearchSourceFiltered.Clear();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                // NOTE: The more results that are displayed the more memory usage goes up. Only care about the top search results anyway.
                int resultsLimit = 10;
                var results = Search(m_searchTerm).Take(resultsLimit);

                if (results.Any())
                {
                    SearchSourceFiltered = new ObservableCollection<ProjectSourceInfoItem>(results.ToList());
                    SearchResultsVisibility = true;
                }
            }
        }

        public IEnumerable<ProjectSourceInfoItem> Search(string searchTerm)
        {
            return m_searchSourceUnfiltered.Where(item => item.DirInfo.Name.ToLower().StartsWith(searchTerm.ToLower()))
                                           .OrderBy(x => x.DirInfo.Name);
        }
        public IEnumerable<ProjectSourceInfoItem> SelectedItems()
        {
            return m_searchSourceFiltered.Where(item => item.IsSelected);
        }

        // Commands
        private RelayCommand _editCommand;
        public RelayCommand EditCommand => _editCommand ?? (_editCommand = new RelayCommand(param => EditCommandProcessing()));

        private void EditCommandProcessing()
        {
            foreach (ProjectSourceInfoItem item in SelectedItems())
            {
                Process.Start(item.DirInfo.FullName);
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
