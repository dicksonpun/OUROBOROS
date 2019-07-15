using PANDA.ViewModel;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace PANDA
{
    public class UserSettingsHelper
    {
        // Class members
        private readonly MainWindow m_mainWindow;
        public UserProfileItem UserProfile { get; private set; }

        // Constructor
        public UserSettingsHelper(MainWindow mainWindow)
        {
            m_mainWindow  = mainWindow; 
            UserProfile   = new UserProfileItem(); 
            InitializeUserSettings();
        }

        // Public Helper Functions
        public bool DetermineUserOrNonuserView(string inputView)
        {
           return inputView.Split('\\').Last().Split('-').First().Equals(UserProfile.Username.ToString());
        }

        // Private Helper Functions
        private void InitializeUserSettings()
        {
            // Get Username
            GetUsernameCommand getUsernameCommand = new GetUsernameCommand("echo %username%");
            getUsernameCommand.RunCommand();
            UserProfile.Username = getUsernameCommand.Username;

            m_mainWindow.MainSnackbar.MessageQueue.Enqueue("Welcome back " + UserProfile.Username  );
        }

        // Define helper classes
        class GetUsernameCommand : CMDHelper
        {
            public string Username;
            public GetUsernameCommand(string arguments) : base(arguments) {}

            public override void ProcessOutput(string output)
            {
                // The string generated from output contains return carriage and newline
                Username = output.ToString().TrimEnd();
            }
        }

    }
}
