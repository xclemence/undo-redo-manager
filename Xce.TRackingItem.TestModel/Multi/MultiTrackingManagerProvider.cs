using System.Collections.Generic;
using Xce.TrackingItem;
using Xce.TRackingItem.TestModel.Base;
using Xce.TRackingItem.TestModel.ItemSave;
using Xce.TRackingItem.TestModel.PropertySave;

namespace Xce.TRackingItem.TestModel.Multi
{
    public class MultiTrackingManagerProvider : TrackingManagerProvider
    {
        internal TrackingManager Manager { get; } = new TrackingManager();

        public static MultiTrackingManagerProvider Instance { get; } = new MultiTrackingManagerProvider();

        public override IEnumerable<TrackingManager> GetTrackingManagers()
        {
            yield return Manager;
            yield return PropertyTrackingManagerProvider.Instance.Manager;
            yield return ItemTrackingManagerProvider.Instance.Manager;
        }
    }
}
