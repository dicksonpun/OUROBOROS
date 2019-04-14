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
using TinyMessenger;

namespace PANDA.ViewModel
{
    public class ClearcaseManagerViewModel : ViewModel
    {
        private MessageHubHelper m_messageHubHelper;

        // REQUIRED FOR DATABINDING
        private bool m_autocompleteTextBoxEnabled;
        public bool AutocompleteTextBoxEnabled
        {
            get { return m_autocompleteTextBoxEnabled; }
            set
            {
                m_autocompleteTextBoxEnabled = value;
                OnPropertyChanged(nameof(AutocompleteTextBoxEnabled));
            }
        }

        // REQUIRED FOR DATABINDING
        private PackIconKind m_autocompleteTextBoxIcon;
        public PackIconKind AutocompleteTextBoxIcon
        {
            get { return m_autocompleteTextBoxIcon; }
            set
            {
                m_autocompleteTextBoxIcon = value;
                OnPropertyChanged(nameof(AutocompleteTextBoxIcon));
            }
        }

        // REQUIRED FOR DATABINDING
        private string m_viewSearchTextBoxValue;
        public string ViewSearchTextBoxValue
        {
            get { return m_viewSearchTextBoxValue; }
            set
            {
                m_viewSearchTextBoxValue = value;
                OnPropertyChanged(nameof(ViewSearchTextBoxValue));
            }
        }

        // REQUIRED FOR DATABINDING
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

        // REQUIRED FOR DATABINDING
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
                case (nameof(SelectedItem)):
                    // NOTE: NULL case occurs when de-selection occurs
                    if (SelectedItem != null)
                    {
                        // Set up the message
                        ClearcaseManagerMessage msg = new ClearcaseManagerMessage(this, new ClearcaseManagerMessage_Content()
                        {
                            MessageCommand = ClearcaseManagerMessageCommand.REQUEST_VIEW_ADD,
                            ClearcaseManagerViewItem = (ClearcaseManagerViewItem)m_selectedItem
                        });

                        // Set up callback logic
                        AsyncCallback printToConsole = new AsyncCallback(PrintToConsole); //temp callback
                        void PrintToConsole(IAsyncResult result)
                        {
                            Console.WriteLine("Print To Console Placeholder");
                        }

                        // Send the message
                        m_messageHubHelper.MessageHub.PublishAsync(msg, printToConsole);
                    }
                    break;
                case (nameof(ClearcaseManagerAutocompleteSource)):
                    if (CurrentClearcaseManagerViewItemList.Count is 0)
                    {
                        ViewSearchTextBoxValue = "Establishing connection...";
                        AutocompleteTextBoxIcon = PackIconKind.SyncWarning;
                        //AutocompleteTextBoxEnabled = false;
                        //SelectedItem = null;
                    }
                    else
                    {
                        ViewSearchTextBoxValue = "Search available views...";
                        AutocompleteTextBoxIcon = PackIconKind.Magnify;
                        AutocompleteTextBoxEnabled = true;
                    }
                    break;
            }
        }

        public List<ClearcaseManagerViewItem> CurrentClearcaseManagerViewItemList { get; set; }

        // Constructor
        public ClearcaseManagerViewModel(MessageHubHelper messageHubHelper) : base()
        {
            m_messageHubHelper = messageHubHelper;
            ViewSearchTextBoxValue = "Establishing connection...";
            AutocompleteTextBoxEnabled = false;

            CurrentClearcaseManagerViewItemList = getEmptyClearcaseViewsList();
            ClearcaseManagerAutocompleteSource = new ClearcaseManagerAutocompleteSource(CurrentClearcaseManagerViewItemList);
            m_selectedItem = null;

            // Register to the PropertyChanged event in the class Constructor
            this.PropertyChanged += ClearcaseManagerViewModel_PropertyChanged;

            InitializePeriodicUpdates();
        }

        // Periodic Update Processing
        public CancellationToken PeriodicUpdateCancellationToken { get; set; }
        public async void InitializePeriodicUpdates()
        {
            PeriodicUpdateCancellationToken = new CancellationToken();
            await UpdateViewListPeriodically(TimeSpan.FromSeconds(1.0), PeriodicUpdateCancellationToken);
        }

        public async Task UpdateViewListPeriodically(TimeSpan interval, CancellationToken cancellationToken)
        {
            string directoryPath = m_messageHubHelper.m_supportedNetworkModeHelper.CurrentNetworkMode.NetworkSpecificPath;

            while (true)
            {
                List<ClearcaseManagerViewItem> newClearcaseViewsList = getEmptyClearcaseViewsList();
                if (Directory.Exists(directoryPath))
                {
                    foreach (var currentDirectory in Directory.GetDirectories(directoryPath))
                    {
                        var dir = new DirectoryInfo(currentDirectory);
                        newClearcaseViewsList.Add(new ClearcaseManagerViewItem() { Icon = PackIconKind.SourceBranch, DirectoryName = dir.Name, DirectoryPath = dir.FullName });
                    }
                }

                ClearcaseManagerAutocompleteSource = new ClearcaseManagerAutocompleteSource(newClearcaseViewsList);
                CurrentClearcaseManagerViewItemList = newClearcaseViewsList;

                await Task.Delay(interval, cancellationToken);
            };
        }

        public List<ClearcaseManagerViewItem> getEmptyClearcaseViewsList()
        {
            return new List<ClearcaseManagerViewItem>() { };
        }
    }

    public class ClearcaseManagerViewItem
    {
        public PackIconKind Icon { get; set; }
        public string DirectoryName { get; set; }
        public string DirectoryPath { get; set; }
        public ClearcaseManagerViewItem() { }
    }

    //IAutocompleteSource
    public class ClearcaseManagerAutocompleteSource : AutocompleteSourceChangingItems<ClearcaseManagerViewItem>
    {
        public List<ClearcaseManagerViewItem> ClearcaseManagerViewItems { get; set; }
        public ClearcaseManagerAutocompleteSource(List<ClearcaseManagerViewItem> newList)
        {
            ClearcaseManagerViewItems = newList;
            OnAutocompleteSourceItemsChanged();
        }
        public override IEnumerable<ClearcaseManagerViewItem> Search(string searchTerm)
        {
            searchTerm = searchTerm ?? string.Empty;
            searchTerm = searchTerm.ToLower();

            return ClearcaseManagerViewItems.Where(item => item.DirectoryName.ToLower().Contains(searchTerm));
        }
    }
}
