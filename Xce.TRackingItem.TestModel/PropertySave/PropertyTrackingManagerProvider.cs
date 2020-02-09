using Xce.TrackingItem.TestModel.Base;

namespace Xce.TrackingItem.TestModel.PropertySave
{
    public class PropertyTrackingManagerProvider : BaseTrackingManagerProvider
    {
        public static PropertyTrackingManagerProvider Instance { get; } = new PropertyTrackingManagerProvider();
    }
}
