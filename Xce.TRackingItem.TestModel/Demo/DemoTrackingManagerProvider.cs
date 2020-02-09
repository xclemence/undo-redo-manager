using Xce.TrackingItem.TestModel.Base;

namespace Xce.TrackingItem.TestModel.Demo
{
    public class DemoTrackingManagerProvider : BaseTrackingManagerProvider
    {
        public static DemoTrackingManagerProvider Instance { get; } = new DemoTrackingManagerProvider();
    }
}
