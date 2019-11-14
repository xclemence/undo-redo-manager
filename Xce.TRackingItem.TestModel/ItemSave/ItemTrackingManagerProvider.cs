using System.Collections.Generic;
using Xce.TrackingItem;
using Xce.TRackingItem.TestModel.Base;

namespace Xce.TRackingItem.TestModel.ItemSave
{
    public class ItemTrackingManagerProvider : TrackingManagerProvider
    {
        internal TrackingManager Manager { get; } = new TrackingManager();

        public static ItemTrackingManagerProvider Instance { get; } = new ItemTrackingManagerProvider();

        public override IEnumerable<TrackingManager> GetTrackingManagers()
        {
            yield return Manager;
        }
    }
}
