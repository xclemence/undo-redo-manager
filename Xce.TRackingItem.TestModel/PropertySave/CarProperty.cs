using Xce.TrackingItem;
using Xce.TRackingItem.TestModel.Base;

namespace Xce.TRackingItem.TestModel.PropertySave
{
    public class CarProperty : Car
    {
        private readonly TrackingManager trackingManager = PropertyTrackingManagerProvider.Instance.Manager;

        protected override void OnAfterSetProperty<TObject, TValue>(TObject item, TValue field, TValue value, string callerName)
        {
            if (!trackingManager.IsAction)
                trackingManager.AddAction(() => item.GetTrackingPropertyUpdate(field, value, callerName));
        }
    }
}