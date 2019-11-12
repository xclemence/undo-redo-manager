using System;
using System.Collections.Generic;
using System.Text;
using Xce.TrackingItem;

namespace Xce.TRackingItem.TestModel.Base
{
    public interface ITrackingManagerProvider
    {
        IEnumerable<TrackingManager> GetTrackingManagers();
    }
}
