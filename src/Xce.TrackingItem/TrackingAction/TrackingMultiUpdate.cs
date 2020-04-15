using System.Linq;
using System.Collections.Generic;

namespace Xce.TrackingItem.TrackingAction
{
    public class TrackingMultiUpdate : ITrackingAction
    {
        public TrackingMultiUpdate(IList<ITrackingAction> actions)
        {
            Actions = actions.ToList();
        }

        public IList<ITrackingAction> Actions { get; }
        
        public void Apply()
        {
            var revertActions = Actions.Reverse().ToList();

            foreach (var item in revertActions)
                item.Apply();
        }

        public void Revert()
        {
            foreach (var item in Actions)
                item.Revert();
        }
    }
}
