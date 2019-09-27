namespace OUROBOROS.ViewModel
{
    public class UserProfileViewModel : ViewModel
    {
        // Helpers
        public UserSettingsHelper UserSettingsHelper;
        public ConnectionMonitorViewModel ConnectionMonitor { get; private set; }

        // Databinding
        public UserProfileItem UserProfileItem
        {
            get
            {
                return UserSettingsHelper.UserProfile;
            }
        }

        // Constructor
        public UserProfileViewModel(UserSettingsHelper userSettingsHelper) : base()
        {
            UserSettingsHelper = userSettingsHelper;
            ConnectionMonitor = new ConnectionMonitorViewModel();
        }
    }

    public class UserProfileItem : ViewModel
    {
        public string Username { get; set; }
        public string NetworkName { get; set; }
    }

}