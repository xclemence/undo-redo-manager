using Xce.TrackingItem.TestModel.Base;

namespace Xce.TrackingItem.TestModel.ItemSave
{
    public class ItemTrackingManagerProvider : TrackingManagerProvider
    {
        public static ItemTrackingManagerProvider Instance { get; } = new ItemTrackingManagerProvider();
    }
}
