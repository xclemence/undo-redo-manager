using Xce.TrackingItem.TestModel.Base;

namespace Xce.TrackingItem.TestModel.PropertySave
{
    public class AddressProperty : Address
    {
        private readonly TrackingManager trackingManager = PropertyTrackingManagerProvider.Instance.Manager;

        protected override void OnBeforeSetProperty<TObject, TValue>(TObject item, TValue field, TValue value, string callerName)
        {
            if (!trackingManager.IsAction)
                trackingManager.AddAction(() =>  item.GetTrackingPropertyUpdateV2(field, value, callerName));
        }
    }
}
