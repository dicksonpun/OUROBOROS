using PANDA.ViewModel;
using System.Collections.Generic;

namespace PANDA
{
    public partial class NavigationHelper
    {
        public Dictionary<string, ViewModel.ViewModel> ViewModelMap = new Dictionary<string, ViewModel.ViewModel>();

        // ----------------------------------------------------------------------------------------
        // Class       : NavigationHelper
        // Method      : GetViewModelFromMap
        // Description : Returns the requested ViewModel From ViewModelMap.
        //               If the requested ViewModel instance (key) does not exist, create it and then return it.
        // Parameters  :
        // - key (string)  : Key to ViewModelMap. 
        //                   The expected key pattern is <viewModel_Type>.<unique_Identifier> 
        //                   The .<unique_Identifier> is optional but required if multiple instances of the same viewModel are created
        // - arg1 (string) : OPTIONAL parameter used as a buffer to help instantiate ViewModels.
        // ----------------------------------------------------------------------------------------
        public ViewModel.ViewModel GetViewModelFromMap(string key, string arg1 = null)
        {
            if (!ViewModelExist(key))
            {
                if (key.StartsWith("ClearcaseViewTabControlViewModel"))
                {
                    string viewPath = arg1;
                    ViewModelMap.Add(key, new ClearcaseViewTabControlViewModel(viewPath));
                }
                else if (key.StartsWith("VersionLogViewModel"))
                {
                    ViewModelMap.Add(key, new VersionLogViewModel());
                }
                else if (key.StartsWith("LicenseLogViewModel"))
                {
                    ViewModelMap.Add(key, new LicenseLogViewModel());
                }
            }
            return ViewModelMap[key]; ;
        }
        public bool ViewModelExist(string key)
        {
            return ViewModelMap.ContainsKey(key);
        }

        public void RemoveViewModelFromMap(string key)
        {
            ViewModelMap.Remove(key);
        }

        public void AddToViewModelMap(string viewToAdd)
        {
            GetViewModelFromMap(viewToAdd);
        }
    }
}