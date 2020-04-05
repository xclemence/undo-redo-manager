using Xce.UndoRedo.Models.Base;

namespace Xce.UndoRedo.Models.PropertySave
{
    public class PropertyTrackingManagerProvider : BaseTrackingManagerProvider
    {
        public static PropertyTrackingManagerProvider Instance { get; } = new PropertyTrackingManagerProvider();
    }
}
