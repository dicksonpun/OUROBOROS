using OUROBOROS.ViewModel;
using System.Collections.Generic;

namespace OUROBOROS
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
        // - arg1 (object) : OPTIONAL parameter used to help instantiate ViewModels (MUST BE PROPERLY CASTED BEFORE USE).
        // - arg2 (object) : OPTIONAL parameter used to help instantiate ViewModels (MUST BE PROPERLY CASTED BEFORE USE).
        // ----------------------------------------------------------------------------------------
        public ViewModel.ViewModel GetViewModelFromMap(string key, object arg1 = null, object arg2 = null)
        {
            // Only add if the key doesn't already exist
            if (!ViewModelMap.ContainsKey(key))
            {
                // ==================================================================================
                // PROTOTYPE FEATURE MODULES 
                // ==================================================================================
                if (key.StartsWith("ClearcaseManagerViewModel"))
                {
                    ViewModelMap.Add(key, new ClearcaseManagerViewModel());
                }
                else if (key.StartsWith("ClearcaseTabControlViewModel"))
                {
                    string viewPath = (string)arg1;
                    ViewModelMap.Add(key, new ClearcaseTabControlViewModel(viewPath));
                }
                else if (key.StartsWith("ProjectHelperViewModel"))
                {
                    ViewModelMap.Add(key, new ProjectHelperViewModel());
                }
                else if (key.StartsWith("ProjectHelperSettingsViewModel"))
                {
                    ViewModelMap.Add(key, new ProjectHelperSettingsViewModel());
                }
                // ==================================================================================
                // EXPERIMENTAL / LEARNING MODULES 
                // ==================================================================================
                else if (key.StartsWith("UserProfileViewModel"))
                {
                    UserSettingsHelper helper = (UserSettingsHelper)arg1;
                    ViewModelMap.Add(key, new UserProfileViewModel(helper));
                }
                else if (key.StartsWith("ColorToolViewModel"))
                {
                    ViewModelMap.Add(key, new ColorToolViewModel());
                }
                else if (key.StartsWith("ClearcaseViewHelperViewModel"))
                {
                    NavigationHelper helper = (NavigationHelper)arg1;
                    ViewModelMap.Add(key, new ClearcaseViewHelperViewModel(helper));
                }
                else if (key.StartsWith("ClearcaseViewTabControlViewModel"))
                {
                    string viewPath = (string)arg1;
                    string username = (string)arg2;
                    ViewModelMap.Add(key, new ClearcaseViewTabControlViewModel(viewPath, username));
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