using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TinyMessenger;

namespace PANDA
{
    public partial class MainWindow : Window
    {
        //====================================//
        // Main Window Message Hub Processing //
        //====================================//
        public TinyMessengerHub MessageHub { get; private set; }

        private async void InitializeMessageHub()
        {
            // Simple hub creation
            MessageHub = new TinyMessengerHub();

            //var resolvedHub = container.Resolve<ITinyMessengerHub>();

            // Publish Messages Periodically And Asynchronously
            await PeriodicAsync_ClearcaseManagerUpdate(TimeSpan.FromSeconds(1.0));
        }

        public async Task PeriodicAsync_ClearcaseManagerUpdate(TimeSpan interval)
        {
            CancellationToken cancellationToken;
            while (true)
            {
                string directoryPath = CurrentApplicationMode.NetworkSpecificPath;

                if (Directory.Exists(directoryPath))
                {
                    //Console.WriteLine("Directory Path: Found");
                }
                else
                {
                    //Console.WriteLine("Directory Path: Not Found");
                }

                MessageHub.PublishAsync(new ClearcaseManager_UpdateMessage(this, new ClearcaseManager_UpdateMessage_Content() { Path = directoryPath }), printToConsole);
                await Task.Delay(interval, cancellationToken);
            };
        }

        AsyncCallback printToConsole = new AsyncCallback(PrintToConsole);
        static void PrintToConsole(IAsyncResult result)
        {
            Console.WriteLine("Published Message to ClearcaseManager");
        }

        public class ClearcaseManager_UpdateMessage : GenericTinyMessage<ClearcaseManager_UpdateMessage_Content>
        {
            public ClearcaseManager_UpdateMessage(object sender, ClearcaseManager_UpdateMessage_Content content) : base(sender, content) {}
        }

        public class ClearcaseManager_UpdateMessage_Content
        {
            public string Path { get; set; }
        }
    }
}
