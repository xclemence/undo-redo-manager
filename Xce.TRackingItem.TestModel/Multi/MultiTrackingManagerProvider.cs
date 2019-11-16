using System.Collections.Generic;
using Xce.TrackingItem.TestModel.Base;
using Xce.TrackingItem.TestModel.DataSet;
using Xce.TrackingItem.TestModel.ItemSave;
using Xce.TrackingItem.TestModel.PropertySave;

namespace Xce.TrackingItem.TestModel.Multi
{
    public class MultiTrackingManagerProvider : TrackingManagerProvider
    {
        public static MultiTrackingManagerProvider Instance { get; } = new MultiTrackingManagerProvider();

        public override IEnumerable<TrackingManager> GetTrackingManagers()
        {
            yield return Manager;
            yield return PropertyTrackingManagerProvider.Instance.Manager;
            yield return ItemTrackingManagerProvider.Instance.Manager;
            yield return DataSetTrackingManagerProvider.Instance.Manager;
        }
    }
}
