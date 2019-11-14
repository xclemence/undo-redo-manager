using System.Collections.Generic;
using Xce.TrackingItem;
using Xce.TRackingItem.TestModel.Base;

namespace Xce.TRackingItem.TestModel.PropertySave
{
    public class PropertyTrackingManagerProvider : TrackingManagerProvider
    {
        internal TrackingManager Manager { get; } = new TrackingManager();

        public static PropertyTrackingManagerProvider Instance { get; } = new PropertyTrackingManagerProvider();

        public override IEnumerable<TrackingManager> GetTrackingManagers()
        {
            yield return Manager;
        }
    }
}
