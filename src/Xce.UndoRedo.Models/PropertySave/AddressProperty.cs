using Xce.TrackingItem;
using Xce.UndoRedo.Models.Base;

namespace Xce.UndoRedo.Models.PropertySave
{
    public class AddressProperty : Address
    {
        private readonly TrackingManager trackingManager = PropertyTrackingManagerProvider.Instance.Manager;

        protected override void OnBeforeSetProperty<TObject, TValue>(TObject item, TValue field, TValue value, string callerName) =>
            trackingManager.AddAction(() =>  item.GetTrackingPropertyUpdateV2(field, value, callerName));
    }
}
