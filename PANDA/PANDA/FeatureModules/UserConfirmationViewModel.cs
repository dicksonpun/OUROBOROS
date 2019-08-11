using MaterialDesignThemes.Wpf;
using PANDA.Command;

namespace PANDA.ViewModel
{
    // Referenced Bachup Application
    class UserConfirmationViewModel : ViewModel
    {
        public UserConfirmationViewModel(string message)
        {
            SetupCommands();
            Message = message;
        }

        public UserConfirmationViewModel(string message, string submessage)
        {
            SetupCommands();
            Message = message;
            SubMessage = submessage;
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        private string _subMessage;
        public string SubMessage
        {
            get { return _subMessage; }
            set
            {
                _subMessage = value; 
                OnPropertyChanged(nameof(SubMessage));
            }
        }

        // Relay Commands
        public RelayCommand NoCommand { get; private set; }
        public RelayCommand YesCommand { get; private set; }

        #region Events

        private void No(object o)
        {
            DialogHost.CloseDialogCommand.Execute(false, null);
        }

        private void Yes(object o)
        {
            DialogHost.CloseDialogCommand.Execute(true, null);
        }

        #endregion

        #region Methods

        private void SetupCommands()
        {
            NoCommand = new RelayCommand(No);
            YesCommand = new RelayCommand(Yes);
        }

        #endregion

    }
}
