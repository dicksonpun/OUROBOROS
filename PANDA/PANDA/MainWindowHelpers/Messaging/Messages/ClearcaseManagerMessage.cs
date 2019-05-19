using PANDA.ViewModel;
using System.Collections.Generic;
using TinyMessenger;

namespace PANDA
{
    public class ClearcaseManagerMessage : GenericTinyMessage<ClearcaseManagerMessage_Content>
    {
        public ClearcaseManagerMessage(object sender, ClearcaseManagerMessage_Content content) : base(sender, content) { }
    }

    public class ClearcaseManagerMessage_Content
    {
        public List<ClearcaseManagerViewItem> ClearcaseManagerViewItemsList { get; set; }
    }
}
