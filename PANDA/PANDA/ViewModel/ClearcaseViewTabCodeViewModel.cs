using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;
using PANDA;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDA.ViewModel
{
    public class ClearcaseViewTabCodeViewModel : ViewModel
    {
        // Members
        private ProjectSourceHelper m_projectSourceHelper;
        private string m_username = "dickson"; // TODO: NEED to build user settings profile
        private readonly string m_viewPath;
        private bool m_isUserView;

        // Autocomplete Databinding
        private AutocompleteSourceChangingItems<ProjectSourceInfoItem> m_clearcaseCodeAutocompleteSource;
        public AutocompleteSourceChangingItems<ProjectSourceInfoItem> ClearcaseCodeAutocompleteSource
        {
            get { return m_clearcaseCodeAutocompleteSource; }
            set
            {
                m_clearcaseCodeAutocompleteSource = value;
                OnPropertyChanged(nameof(ClearcaseCodeAutocompleteSource));
            }
        }

        // Selected Item Databinding
        private object m_selectedItem;
        public object SelectedItem
        {
            get { return m_selectedItem; }

            set
            {
                m_selectedItem = value;
                if (m_selectedItem != null)
                {
                    // Only invoke property change if valid item was selected
                    OnPropertyChanged(nameof(SelectedItem));
                }

            }
        }

        // Constructor
        public ClearcaseViewTabCodeViewModel(string viewPath, bool isUserView) : base()
        {
            m_selectedItem = null;
            m_viewPath = viewPath;
            m_isUserView = isUserView;

            ClearcaseCodeAutocompleteSource = new ClearcaseCodeAutocompleteSource(new List<ProjectSourceInfoItem>());
            m_projectSourceHelper = new ProjectSourceHelper(this, viewPath);
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
        public DirectoryInfo DirInfo { get; set; }
        public string VOBFullPath { get; set; }
        public string VOBParentPath { get; set; }
        public PackIconKind Icon { get; set; }

        // Placeholder items
        public SOURCE_CONTROLLED_STATUS SourceControlledStatus { get; set; }
        public int SourceVersion { get; set; }
        public ProjectSourceInfoItem() { }
    }

    public class ClearcaseCodeAutocompleteSource : AutocompleteSourceChangingItems<ProjectSourceInfoItem>
    {
        private readonly List<ProjectSourceInfoItem> m_clearcaseViewTabCodeItems;
        public ClearcaseCodeAutocompleteSource(List<ProjectSourceInfoItem> newList)
        {
            m_clearcaseViewTabCodeItems = newList;
            OnAutocompleteSourceItemsChanged();
        }
        public override IEnumerable<ProjectSourceInfoItem> Search(string searchTerm)
        {
            searchTerm = searchTerm ?? string.Empty;
            searchTerm = searchTerm.ToLower();

            return m_clearcaseViewTabCodeItems.Where(item => item.DirInfo.Name.ToLower().Contains(searchTerm));
        }
    }
}
