using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;

namespace PANDA.ViewModel
{
    public class ClearcaseTabControlViewModel : ViewModel
    {        
        // Constructor
        public ClearcaseTabControlViewModel(string viewName) : base()
        {
            SelectedIndex = 0;

            m_clearcaseViewTabItems = new ObservableCollection<ClearcaseTabControlItem>()
            {
                new ClearcaseTabControlItem() { Header = "PROJECT SOURCE"  , Kind = PackIconKind.CodeTags , TabChildViewModel = new ClearcaseProjectSourceViewModel(viewName)},
                new ClearcaseTabControlItem() { Header = "AUTOMATION"      , Kind = PackIconKind.Robot    , TabChildViewModel = new LicenseLogViewModel()},
                new ClearcaseTabControlItem() { Header = "INSIGHTS"        , Kind = PackIconKind.ChartBar , TabChildViewModel = new LicenseLogViewModel()}
            };
        }

        // Databinding to store last selected index
        public int SelectedIndex { get; set; }

        // Databinding for TabControl UI elements
        private ObservableCollection<ClearcaseTabControlItem> m_clearcaseViewTabItems;
        public ObservableCollection<ClearcaseTabControlItem> ClearcaseViewTabItems
        {
            get { return m_clearcaseViewTabItems; }
            set { m_clearcaseViewTabItems = value; }
        }
    }

    public class ClearcaseTabControlItem : ViewModel
    {
        public string Header { get; set; }
        public PackIconKind Kind { get; set; }
        public ViewModel TabChildViewModel { get; set; }
        public ClearcaseTabControlItem() { }
    }
}

