using Xce.UndoRedo.Models.Base;

namespace Xce.UndoRedo.Models.Demo
{
    public class DemoTrackingManagerProvider : BaseTrackingManagerProvider
    {
        public static DemoTrackingManagerProvider Instance { get; } = new DemoTrackingManagerProvider();
    }
}
