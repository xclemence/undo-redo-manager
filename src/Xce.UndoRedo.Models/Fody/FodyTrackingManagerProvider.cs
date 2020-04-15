using Xce.TrackingItem;
using Xce.UndoRedo.Models.Base;

namespace Xce.UndoRedo.Models.Fody
{
    public class FodyTrackingManagerProvider : BaseTrackingManagerProvider
    {
        public static FodyTrackingManagerProvider Instance { get; } = new FodyTrackingManagerProvider();

        FodyTrackingManagerProvider()
        {
            Manager = TrackingManagerProvider.GetDefault();
        }

    }
}
