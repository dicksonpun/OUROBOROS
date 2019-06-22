using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDA.ViewModel
{
    public class ClearcaseViewTabControlViewModel : ViewModel
    {
        // Members
        private string m_viewPath;
        private string m_username = "dickson"; // TODO: NEED to build user settings profile
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
        public ClearcaseViewTabControlViewModel(string viewPath) : base()
        {
            SelectedIndex = 0;
            m_viewPath = viewPath;

            DetermineUserOrNonuserView(); // NOTE: m_viewpath should be set prior to call

            m_clearcaseViewTabItems = new ObservableCollection<ClearcaseViewTabItem>()
            {
                new ClearcaseViewTabItem() { Header = "Code"    , Kind = PackIconKind.CodeTags       , TabChildViewModel = new ClearcaseViewTabCodeViewModel(viewPath, m_isUserView)},
                new ClearcaseViewTabItem() { Header = "Memo"    , Kind = PackIconKind.BookOpenOutline, TabChildViewModel = new LicenseLogViewModel()},
                new ClearcaseViewTabItem() { Header = "Tools"   , Kind = PackIconKind.ServiceToolbox , TabChildViewModel = new LicenseLogViewModel()},
                new ClearcaseViewTabItem() { Header = "Insights", Kind = PackIconKind.ChartBar       , TabChildViewModel = new LicenseLogViewModel()},
                new ClearcaseViewTabItem() { Header = "Settings", Kind = PackIconKind.Gear           , TabChildViewModel = new LicenseLogViewModel()}
            };
        }

        // Helpers
        private void DetermineUserOrNonuserView()
        {
            m_isUserView = m_viewPath.Split('\\').Last().StartsWith(m_username);
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
