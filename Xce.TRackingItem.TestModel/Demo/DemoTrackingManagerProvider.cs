using Xce.TrackingItem.TestModel.Base;

namespace Xce.TrackingItem.TestModel.Demo
{
    public class DemoTrackingManagerProvider : TrackingManagerProvider
    {
        public static DemoTrackingManagerProvider Instance { get; } = new DemoTrackingManagerProvider();
    }
}
