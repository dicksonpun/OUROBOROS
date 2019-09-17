using System;
using System.Threading;
using System.Threading.Tasks;

namespace PANDA.ViewModel
{
    public class ConnectionMonitorViewModel : ViewModel
    {
        #region Members

        private CancellationTokenSource cancellationTokenSource;

        #endregion


        #region Databinding

        private bool m_isConnected;
        public bool IsConnected
        {
            get { return m_isConnected; }
            set
            {
                m_isConnected = value;
                OnPropertyChanged(nameof(IsConnected));
            }
        }

        private string m_username;
        public string Username
        {
            get { return m_username; }
            set
            {
                m_username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string m_userID;
        public string UserID
        {
            get { return m_userID; }
            set
            {
                m_userID = value;
                OnPropertyChanged(nameof(UserID));
            }
        }

        #endregion


        #region Constructor

        public ConnectionMonitorViewModel()
        {
            m_isConnected = false;
            m_username = string.Empty;
            m_userID = string.Empty;

            StartAutoRefresh();
        }

        #endregion


        #region Methods

        public async void StartAutoRefresh()
        {
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken PeriodicUpdateCancellationToken = cancellationTokenSource.Token;
            try
            {
                await TestConnectionPeriodically(TimeSpan.FromSeconds(10), PeriodicUpdateCancellationToken);
            }
            catch
            {
                cancellationTokenSource.Dispose();
            }
        }


        public async Task TestConnectionPeriodically(TimeSpan interval, CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    await Task.Run(() =>
                    {
                        // Periodic toggle to test states
                        IsConnected = !IsConnected;

                        if (IsConnected)
                        {
                            Username = "FIRSTNAME/LASTNAME";
                            UserID = "A1/#ABC123";
                        }
                        else
                        {
                            Username = string.Empty;
                            UserID = "A1/#ABC123";
                        }
                    });
                    
                    await Task.Delay(interval, cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    throw;
                }
            }
        }

        #endregion
    }
}
