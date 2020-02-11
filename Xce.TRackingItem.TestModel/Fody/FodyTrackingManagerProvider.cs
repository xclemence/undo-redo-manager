using Xce.TrackingItem.TestModel.Base;

namespace Xce.TrackingItem.TestModel.Fody
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
