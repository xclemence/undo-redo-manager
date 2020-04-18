using System.Collections.Generic;
using Xce.TrackingItem;
using Xce.UndoRedo.Models.Base;
using Xce.UndoRedo.Models.DataSet;
using Xce.UndoRedo.Models.ItemSave;
using Xce.UndoRedo.Models.PropertySave;

namespace Xce.UndoRedo.Models.Multi
{
    public class MultiTrackingManagerProvider : BaseTrackingManagerProvider
    {
        public static MultiTrackingManagerProvider Instance { get; } = new MultiTrackingManagerProvider();

        public override IEnumerable<TrackingManager> TrackingManagers
        {
            get
            {
                yield return Manager;
                yield return PropertyTrackingManagerProvider.Instance.Manager;
                yield return ItemTrackingManagerProvider.Instance.Manager;
                yield return DataSetTrackingManagerProvider.Instance.Manager;
            }
        }
    }
}
