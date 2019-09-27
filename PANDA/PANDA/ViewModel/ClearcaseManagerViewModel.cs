using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace OUROBOROS.ViewModel
{
    public class ClearcaseViewHelperViewModel : ViewModel
    {
        // Autocomplete Databinding
        private AutocompleteSourceChangingItems<ClearcaseManagerViewItem> m_clearcaseManagerAutocompleteSource;
        public AutocompleteSourceChangingItems<ClearcaseManagerViewItem> ClearcaseManagerAutocompleteSource
        {
            get { return m_clearcaseManagerAutocompleteSource; }
            set
            {
                m_clearcaseManagerAutocompleteSource = value;
                OnPropertyChanged(nameof(ClearcaseManagerAutocompleteSource));
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

        // Background Refresh Settings Databinding
        private ObservableCollection<ClearcaseManagerViewItem> m_clearcaseManagerBackgroundRefreshSource;
        public ObservableCollection<ClearcaseManagerViewItem> ClearcaseManagerBackgroundRefreshSource
        {
            get { return m_clearcaseManagerBackgroundRefreshSource; }
            set
            {
                m_clearcaseManagerBackgroundRefreshSource = value;
                OnPropertyChanged(nameof(ClearcaseManagerBackgroundRefreshSource));
            }
        }

        // SelectedAction Databinding (Experimental for now)
        public string SelectedAction
        {
            get { return AddLocalProjectFileSystemDialog.SelectedAction; }
            set
            {
                AddLocalProjectFileSystemDialog.SelectedAction = value;
                OnPropertyChanged(nameof(SelectedAction));
            }
        }

        // Members
        public NavigationHelper AccessNavigationHelper;
        public FileSystemDialogViewModel AddLocalProjectFileSystemDialog;

        // Constructor
        public ClearcaseViewHelperViewModel(NavigationHelper navigationHelper) : base()
        {
            // Initialize Helpers
            AccessNavigationHelper = navigationHelper;
            AddLocalProjectFileSystemDialog = new FileSystemDialogViewModel();

            // Initialize Members
            ClearcaseManagerAutocompleteSource = new ClearcaseManagerAutocompleteSource(new List<ClearcaseManagerViewItem>());
            ClearcaseManagerBackgroundRefreshSource = new ObservableCollection<ClearcaseManagerViewItem>();
            m_selectedItem = null;


            // Register to the PropertyChanged event in the class Constructor
            this.PropertyChanged += ClearcaseManagerViewModel_PropertyChanged;
        }

        // PropertyChanged event handler
        private void ClearcaseManagerViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Console.WriteLine("A property has changed: " + e.PropertyName);
            switch (e.PropertyName)
            {
                case (nameof(SelectedItem)):
                    ClearcaseManagerViewItem viewItem = (ClearcaseManagerViewItem)SelectedItem;
                    AccessNavigationHelper.AddSelectedView(viewItem.ViewName);
                    break;
            }
        }
    }

    public class ClearcaseManagerViewItem
    {
        public PackIconKind Icon { get; set; }
        public string ViewName { get; set; }
        public string ViewPath { get; set; }
        public bool IsUserView { get; set; }
        public ClearcaseManagerViewItem() { }
    }

    public class ClearcaseManagerAutocompleteSource : AutocompleteSourceChangingItems<ClearcaseManagerViewItem>
    {
        private readonly List<ClearcaseManagerViewItem> m_clearcaseManagerViewItems;
        public ClearcaseManagerAutocompleteSource(List<ClearcaseManagerViewItem> newList)
        {
            m_clearcaseManagerViewItems = newList;
            OnAutocompleteSourceItemsChanged();
        }
        public override IEnumerable<ClearcaseManagerViewItem> Search(string searchTerm)
        {
            searchTerm = searchTerm ?? string.Empty;
            searchTerm = searchTerm.ToLower();

            return m_clearcaseManagerViewItems.Where(item => item.ViewName.ToLower().Contains(searchTerm));
        }
    }
}