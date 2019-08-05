using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PANDA.ViewModel
{
    public class ConfigSpecEditorViewModel : ViewModel
    {
        public ConfigSpecEditorViewModel()
        {

        }

        private ObservableCollection<string> m_currentConfigSpec;
        public ObservableCollection<string> CurrentConfigSpec
        {
            get { return m_currentConfigSpec; }
            set
            {
                m_currentConfigSpec = value;
                OnPropertyChanged(nameof(CurrentConfigSpec));
            }
        }

        private ObservableCollection<string> m_tentativeConfigSpec;
        public ObservableCollection<string> TentativeConfigSpec
        {
            get { return m_tentativeConfigSpec; }
            set
            {
                m_tentativeConfigSpec = value;
                OnPropertyChanged(nameof(TentativeConfigSpec));
            }
        }
    }
}
