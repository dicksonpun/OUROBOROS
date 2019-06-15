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
        // Instantiate a Singleton of the Semaphore with a value of 1. This means that only 1 thread can be granted access at a time.
        private static SemaphoreSlim m_semaphore = new SemaphoreSlim(1, 1);

        // REQUIRED FOR DATABINDING
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

        // REQUIRED FOR DATABINDING
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

        // REQUIRED FOR DATABINDING
        private List<ClearcaseManagerViewItem> m_currentActiveViews;
        public List<ClearcaseManagerViewItem> CurrentActiveViews
        {
            get { return m_currentActiveViews; }

            set
            {
                m_currentActiveViews = value;
                OnPropertyChanged(nameof(CurrentActiveViews));
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
                        m_semaphore.WaitAsync();
                        try
                        {
                            CurrentActiveViews.Add((ClearcaseManagerViewItem)m_selectedItem);
                        }
                        finally
                        {
                            m_semaphore.Release();
                        }

                    }
                    break;
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

        // Constructor
        public ClearcaseManagerViewModel() : base()
        {
            ConnectionAvailable = false;
            CurrentActiveViews = GetEmptyClearcaseManagerViewsList();
            ClearcaseManagerAutocompleteSource = new ClearcaseManagerAutocompleteSource(GetEmptyClearcaseManagerViewsList());
            m_selectedItem = null;

            // Register to the PropertyChanged event in the class Constructor
            this.PropertyChanged += ClearcaseManagerViewModel_PropertyChanged;

            //InitializePeriodicUpdates();
        }


        /*
        // Periodic Update Processing
        public CancellationToken PeriodicUpdateCancellationToken { get; set; }
        public async void InitializePeriodicUpdates()
        {
            PeriodicUpdateCancellationToken = new CancellationToken();
            await UpdateViewListPeriodically(TimeSpan.FromSeconds(3), PeriodicUpdateCancellationToken);
        }


        public async Task UpdateViewListPeriodically(TimeSpan interval, CancellationToken cancellationToken)
        {
            string directoryPath = SupportedNetworkModeHelper.CurrentNetworkMode.NetworkSpecificPath;

            while (true)
            {
                // Asynchronously wait to enter the Semaphore. If no-one has been granted access to the Semaphore, code execution will proceed, otherwise this thread waits here until the navigationUpdate_Mutex is released 
                await navigationUpdate_Mutex.WaitAsync();
                try
                {
                    List<ClearcaseManagerViewItem> tempClearcaseManagerViewsList = GetEmptyClearcaseManagerViewsList(); // Used to populate the autocomplete list
                    List<ClearcaseManagerViewItem> tempEligibleUserViewsList = GetEmptyClearcaseManagerViewsList(); // Used to populate the active views
                    List<string> tempCurrentActive = GetListOfViews(navigationClearcaseViews);

                    if (Directory.Exists(directoryPath))
                    {
                        string tempUserName = "dickson"; // TODO: NEED to build user settings profile

                        foreach (var currentDirectory in Directory.GetDirectories(directoryPath))
                        {
                            var dir = new DirectoryInfo(currentDirectory);
                            tempClearcaseManagerViewsList.Add(new ClearcaseManagerViewItem() { Icon = PackIconKind.SourceBranch, ViewName = dir.Name, ViewPath = dir.FullName });

                            // Build list of eligible user views
                            if (dir.Name.StartsWith(tempUserName) || tempCurrentActive.Contains(dir.Name))
                            {
                                tempEligibleUserViewsList.Add(new ClearcaseManagerViewItem() { Icon = PackIconKind.SourceBranch, ViewName = dir.Name, ViewPath = dir.FullName });
                            }
                        }

                        navigationClearcaseViews = new List<ClearcaseManagerViewItem>(tempEligibleUserViewsList);

                        ConnectionAvailable = true; // NOTE: Must be set prior to ClearcaseManagerAutocompleteSource property change
                    }
                    else
                    {
                        // NOTE: Do not bother sending empty list to Main Window since the connection is not available and the results are invalid.
                        ConnectionAvailable = false; // NOTE: Must be set prior to ClearcaseManagerAutocompleteSource property change
                    }

                    ClearcaseManagerAutocompleteSource = new ClearcaseManagerAutocompleteSource(tempClearcaseManagerViewsList);

                    await Task.Delay(interval, cancellationToken);
                }
                finally
                {
                    // When the task is ready, release the navigationUpdate_Mutex. It is vital to ALWAYS release the navigationUpdate_Mutex when we are ready, or else we will end up with a Semaphore that is forever locked.
                    // This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                    navigationUpdate_Mutex.Release();
                }
            };
        }
        */
        public List<ClearcaseManagerViewItem> GetEmptyClearcaseManagerViewsList()
        {
            return new List<ClearcaseManagerViewItem>() { };
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

            // Also update upon new search term
            OnAutocompleteSourceItemsChanged();

            return m_clearcaseManagerViewItems.Where(item => item.ViewName.ToLower().Contains(searchTerm));
        }
    }
}