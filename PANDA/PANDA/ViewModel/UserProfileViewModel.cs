namespace PANDA.ViewModel
{
    public class UserProfileViewModel : ViewModel
    {
        // Helpers
        public UserSettingsHelper UserSettingsHelper;

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
        }
    }

    public class UserProfileItem : ViewModel
    {
        public string Username { get; set; }
        public string NetworkName { get; set; }
    }

}