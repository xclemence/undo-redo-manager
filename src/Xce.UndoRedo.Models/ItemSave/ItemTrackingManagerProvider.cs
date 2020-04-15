using Xce.UndoRedo.Models.Base;

namespace Xce.UndoRedo.Models.ItemSave
{
    public class ItemTrackingManagerProvider : BaseTrackingManagerProvider
    {
        public static ItemTrackingManagerProvider Instance { get; } = new ItemTrackingManagerProvider();
    }
}
