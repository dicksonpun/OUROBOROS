using PANDA.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using TinyMessenger;

namespace PANDA
{
    public partial class MessageHubHelper
    {
        public TinyMessengerHub MessageHub { get; private set; }
        public SupportedNetworkModeHelper SupportedNetworkModeHelper;

        public MessageHubHelper(SupportedNetworkModeHelper supportedNetworkModeHelper)
        {
            MessageHub = new TinyMessengerHub();
            SupportedNetworkModeHelper = supportedNetworkModeHelper;
        }

        public List<string> GetListOfViews(List<ClearcaseManagerViewItem> viewList)
        {
            List<string> result = new List<string>();

            foreach (var view in viewList)
                result.Add(view.ViewName);

            return result;
        }
    }
}
