using Xce.TrackingItem.TestModel.Base;

namespace Xce.TrackingItem.TestModel.ItemSave
{
    public class ItemTrackingManagerProvider : BaseTrackingManagerProvider
    {
        public static ItemTrackingManagerProvider Instance { get; } = new ItemTrackingManagerProvider();
    }
}
