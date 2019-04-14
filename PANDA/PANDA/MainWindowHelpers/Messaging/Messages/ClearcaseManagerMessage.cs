using PANDA.ViewModel;
using TinyMessenger;

namespace PANDA
{
    public enum ClearcaseManagerMessageCommand
    {
        REQUEST_VIEW_ADD,
        REQUEST_VIEW_REMOVE
    };

    public class ClearcaseManagerMessage : GenericTinyMessage<ClearcaseManagerMessage_Content>
    {
        public ClearcaseManagerMessage(object sender, ClearcaseManagerMessage_Content content) : base(sender, content) { }
    }

    public class ClearcaseManagerMessage_Content
    {
        public ClearcaseManagerMessageCommand MessageCommand { get; set; }
        public ClearcaseManagerViewItem ClearcaseManagerViewItem { get; set; }
    }
}
