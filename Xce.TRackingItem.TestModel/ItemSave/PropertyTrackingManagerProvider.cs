using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xce.TrackingItem;
using Xce.TRackingItem.TestModel.Base;

namespace Xce.TRackingItem.TestModel.ItemSave
{
    public class ItemTrackingManagerProvider : ITrackingManagerProvider
    {
        internal TrackingManager Manager { get; } = new TrackingManager();

        public static ItemTrackingManagerProvider Instance { get; } = new ItemTrackingManagerProvider();

        public IEnumerable<TrackingManager> GetTrackingManagers()
        {
            yield return Manager;
        }
    }
}
