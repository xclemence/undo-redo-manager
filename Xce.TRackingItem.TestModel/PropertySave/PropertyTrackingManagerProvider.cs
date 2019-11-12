using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xce.TrackingItem;
using Xce.TRackingItem.TestModel.Base;

namespace Xce.TRackingItem.TestModel.PropertySave
{
    public class PropertyTrackingManagerProvider : ITrackingManagerProvider
    {
        internal TrackingManager Manager { get; } = new TrackingManager();

        public static PropertyTrackingManagerProvider Instance { get; } = new PropertyTrackingManagerProvider();

        public IEnumerable<TrackingManager> GetTrackingManagers()
        {
            yield return Manager;
        }
    }
}
