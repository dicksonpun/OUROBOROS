using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;
using PANDA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PANDA.ViewModel
{
    public class ClearcaseManagerViewModel : ViewModel
    {
        private PackIconKind m_autocompleteTextBoxIcon;
        public PackIconKind AutocompleteTextBoxIcon
        {
            get { return m_autocompleteTextBoxIcon; }
            set
            {
                // Only update if it changes
                if (m_autocompleteTextBoxIcon != value)
                {
                    m_autocompleteTextBoxIcon = value;
                    OnPropertyChanged(nameof(AutocompleteTextBoxIcon));
                }
            }
        }

        private string m_viewSearchTextBoxValue;
        public string ViewSearchTextBoxValue
        {
            get { return m_viewSearchTextBoxValue; }
            set
            {
                // Only update if it changes
                if (m_viewSearchTextBoxValue != value)
                {
                    m_viewSearchTextBoxValue = value;
                    OnPropertyChanged(nameof(ViewSearchTextBoxValue));
                }
            }
        }

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

        private object m_selectedItem;
        public object SelectedItem
        {
            get { return m_selectedItem; }

            set
            {
                m_selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        // PropertyChanged event handler
        private void ClearcaseManagerViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine("A property has changed: " + e.PropertyName);

            switch (e.PropertyName)
            {
                case (nameof(ClearcaseManagerAutocompleteSource)):
                    if (ConnectionAvailable)
                    {
                        ViewSearchTextBoxValue = "Search available views...";
                        AutocompleteTextBoxIcon = PackIconKind.Magnify;
                    }
                    else
                    {
                        ViewSearchTextBoxValue = "Establishing connection...";
                        AutocompleteTextBoxIcon = PackIconKind.SyncWarning;
                    }
                    break;
            }
        }

        public bool ConnectionAvailable { get; set; }

        public ClearcaseManagerViewModel() : base()
        {
            ConnectionAvailable = false;
            ClearcaseManagerAutocompleteSource = new ClearcaseManagerAutocompleteSource(new List<ClearcaseManagerViewItem>());
            m_selectedItem = null;

            // Register to the PropertyChanged event in the class Constructor
            this.PropertyChanged += ClearcaseManagerViewModel_PropertyChanged;
        }
    }

    public class ClearcaseManagerViewItem
    {
        public PackIconKind Icon { get; set; }
        public string ViewName { get; set; }
        public string ViewPath { get; set; }
        public ClearcaseManagerViewItem() { }
    }

    public class ClearcaseManagerAutocompleteSource : AutocompleteSourceChangingItems<ClearcaseManagerViewItem>
    {
        private List<ClearcaseManagerViewItem> m_clearcaseManagerViewItems { get; set; }
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