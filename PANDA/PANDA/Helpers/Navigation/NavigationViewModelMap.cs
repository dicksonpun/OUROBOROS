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
            // Only add if the key doesn't already exist
            if (!ViewModelMap.ContainsKey(key))
            {
                if (key.StartsWith("ClearcaseManagerViewModel"))
                {
                    ViewModelMap.Add(key, new ClearcaseManagerViewModel());
                }
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

        // ----------------------------------------------------------------------------------------
        // Class       : NavigationHelper
        // Method      : RemoveViewModelFromMap
        // Description : Removes the requested ViewModel From ViewModelMap, if it exists.
        // Parameters  :
        // - key (string)  : Key to ViewModelMap
        // ----------------------------------------------------------------------------------------
        public void RemoveViewModelFromMap(string key)
        {
            // Note: The out parameter is discarded since it is not used/needed.
            if (ViewModelMap.TryGetValue(key, out _))
            {
                // Only remove the entry if it currently exists.
                ViewModelMap.Remove(key);
            }
        }
    }
}