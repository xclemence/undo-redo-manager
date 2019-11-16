using Xce.TrackingItem.TestModel.Base;

namespace Xce.TrackingItem.TestModel.PropertySave
{
    public class PropertyTrackingManagerProvider : TrackingManagerProvider
    {
        public static PropertyTrackingManagerProvider Instance { get; } = new PropertyTrackingManagerProvider();
    }
}
