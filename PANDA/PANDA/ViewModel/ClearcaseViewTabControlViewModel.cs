using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Linq;

namespace PANDA.ViewModel
{
    public class ClearcaseViewTabControlViewModel : ViewModel
    {
        // Members
        private string m_viewPath;
        private string m_username;
        private bool m_isUserView;

        // Databinding to store last selected index
        public int SelectedIndex { get; set; }

        // Databinding for TabControl UI elements
        private ObservableCollection<ClearcaseViewTabItem> m_clearcaseViewTabItems;
        public ObservableCollection<ClearcaseViewTabItem> ClearcaseViewTabItems
        {
            get { return m_clearcaseViewTabItems; }
            set { m_clearcaseViewTabItems = value; }
        }

        // Constructor
        public ClearcaseViewTabControlViewModel(string viewPath, string username) : base()
        {
            SelectedIndex = 0;
            m_viewPath = viewPath;
            m_username = username;

            DetermineUserOrNonuserView(); // NOTE: m_viewpath should be set prior to call

            m_clearcaseViewTabItems = new ObservableCollection<ClearcaseViewTabItem>()
            {
                new ClearcaseViewTabItem() { Header = "Project Source"  , Kind = PackIconKind.CodeTags , TabChildViewModel = new ClearcaseViewTabCodeViewModel(viewPath, m_isUserView, m_username)},
                new ClearcaseViewTabItem() { Header = "Automation"      , Kind = PackIconKind.Robot    , TabChildViewModel = new LicenseLogViewModel()},
                new ClearcaseViewTabItem() { Header = "Insights"        , Kind = PackIconKind.ChartBar , TabChildViewModel = new LicenseLogViewModel()}
            };
        }

        // Helpers
        private void DetermineUserOrNonuserView()
        {
            m_isUserView = m_viewPath.Split('\\').Last().Split('-').First().Equals(m_username);
        }
    }

    public class ClearcaseViewTabItem : ViewModel
    {
        public string Header { get; set; }
        public PackIconKind Kind { get; set; }
        public ViewModel TabChildViewModel { get; set; }
        public ClearcaseViewTabItem() { }
    }
}
