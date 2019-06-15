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
        private ObservableCollection<ClearcaseViewTabItem> m_items;
        private readonly string m_viewPath;

        public int SelectedIndex { get; set; }

        public ObservableCollection<ClearcaseViewTabItem> ClearcaseViewTabItems
        {
            get { return m_items; }
            set { m_items = value; }
        }

        public ClearcaseViewTabControlViewModel(string viewPath) : base()
        {
            SelectedIndex = 0;
            m_viewPath = viewPath;

            m_items = new ObservableCollection<ClearcaseViewTabItem>()
            {
                new ClearcaseViewTabItem() { Header = "Code"    , Kind = PackIconKind.CodeTags},
                new ClearcaseViewTabItem() { Header = "Memo"    , Kind = PackIconKind.BookOpenOutline},
                new ClearcaseViewTabItem() { Header = "Tools"   , Kind = PackIconKind.ServiceToolbox},
                new ClearcaseViewTabItem() { Header = "Insights", Kind = PackIconKind.ChartBar},
                new ClearcaseViewTabItem() { Header = "Settings", Kind = PackIconKind.Gear}
            };
        }
    }

    public class ClearcaseViewTabItem : ViewModel
    {
        public string Header { get; set; }
        public PackIconKind Kind { get; set; }
        public ClearcaseViewTabItem() { }
    }
}
