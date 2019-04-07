using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PANDA.ViewModel
{
    public class ClearcaseManagerViewModel : ViewModel
    {
        private object m_selectedItem;

        // Getters and Setters
        public IAutocompleteSource ClearcaseManagerAutocompleteSource { get; }

        public object SelectedItem
        {
            get { return m_selectedItem; }

            set
            {
                m_selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        // Constructor
        public ClearcaseManagerViewModel() : base()
        {
            ClearcaseManagerAutocompleteSource = new ClearcaseManagerAutocompleteSource();
            m_selectedItem = null;
        }
    }

    public class ClearcaseManagerViewItem
    {
        public PackIconKind Icon       { get; set; }
        public string DirectoryName    { get; set; }
        public string DirectoryPath    { get; set; }
        public ClearcaseManagerViewItem() { }
    }

    public class ClearcaseManagerAutocompleteSource : IAutocompleteSource
    {
        private List<ClearcaseManagerViewItem> m_clearcaseManagerViewItems;

        public ClearcaseManagerAutocompleteSource()
        {
            m_clearcaseManagerViewItems = GetClearcaseManagerViewItems();
        }

        private static List<ClearcaseManagerViewItem> GetClearcaseManagerViewItems()
        {
            List<ClearcaseManagerViewItem> temp = new List<ClearcaseManagerViewItem>()
            {
                    new ClearcaseManagerViewItem() { Icon = PackIconKind.Android, DirectoryName = "Android Gingerbread" },
                    new ClearcaseManagerViewItem() { Icon = PackIconKind.Android, DirectoryName = "Android Icecream Sandwich" },
                    new ClearcaseManagerViewItem() { Icon = PackIconKind.Android, DirectoryName = "Android Jellybean" },
                    new ClearcaseManagerViewItem() { Icon = PackIconKind.Android, DirectoryName = "Android Lollipop" },
                    new ClearcaseManagerViewItem() { Icon = PackIconKind.Android, DirectoryName = "Android Nougat" },
                    new ClearcaseManagerViewItem() { Icon = PackIconKind.Linux, DirectoryName = "Debian" },
                    new ClearcaseManagerViewItem() { Icon = PackIconKind.DesktopMac, DirectoryName = "Mac OSX" },
                    new ClearcaseManagerViewItem() { Icon = PackIconKind.Raspberrypi, DirectoryName = "Raspbian" },
                    new ClearcaseManagerViewItem() { Icon = PackIconKind.Ubuntu, DirectoryName = "Ubuntu Wily Werewolf" },
                    new ClearcaseManagerViewItem() { Icon = PackIconKind.Ubuntu, DirectoryName = "Ubuntu Xenial Xerus" },
                    new ClearcaseManagerViewItem() { Icon = PackIconKind.Ubuntu, DirectoryName = "Ubuntu Yakkety Yak" },
                    new ClearcaseManagerViewItem() { Icon = PackIconKind.Ubuntu, DirectoryName = "Ubuntu Zesty Zapus" },
                    new ClearcaseManagerViewItem() { Icon = PackIconKind.Windows, DirectoryName = "Windows 7" },
                    new ClearcaseManagerViewItem() { Icon = PackIconKind.Windows, DirectoryName = "Windows 8" },
                    new ClearcaseManagerViewItem() { Icon = PackIconKind.Windows, DirectoryName = "Windows 8.1" },
                    new ClearcaseManagerViewItem() { Icon = PackIconKind.Windows, DirectoryName = "Windows 10" },
                    new ClearcaseManagerViewItem() { Icon = PackIconKind.Windows, DirectoryName = "Windows Vista" },
                    new ClearcaseManagerViewItem() { Icon = PackIconKind.Windows, DirectoryName = "Windows XP" }
            };
            return temp;
        }

        public IEnumerable Search(string searchTerm)
        {
            searchTerm = searchTerm ?? string.Empty;
            searchTerm = searchTerm.ToLower();

            return m_clearcaseManagerViewItems.Where(item => item.DirectoryName.ToLower().Contains(searchTerm));
        }

        /*
        public async Task UpdateViewListPeriodically(TimeSpan interval, CancellationToken cancellationToken)
        {
            while (true)
            {
                //await FooAsync();
                m_clearcaseManagerViewItems.Add(temp[1]);
                await Task.Delay(interval, cancellationToken);
            };
        }
        */
    }
}
