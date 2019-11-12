using Xce.TrackingItem;
using Xce.TrackingItem.TrackingAction;
using Xce.TRackingItem.TestModel.Base;

namespace Xce.TRackingItem.TestModel.ItemSave
{
    public class DriverItem : Driver<CarItem, AddressItem>, ICopiable<DriverItem>, ISettable<DriverItem>
    {
        private readonly TrackingManager trackingManager = ItemTrackingManagerProvider.Instance.Manager;

        public DriverItem DeepCopy()
        {
            trackingManager.IsAction = true;
            var copy = new DriverItem
            {
                FirstName = FirstName,
                LastName = LastName,
                PhoneNumber = PhoneNumber,
            };
            trackingManager.IsAction = false;

            return copy;
        }

        public void Set(DriverItem item)
        {
            trackingManager.IsAction = true;
            
            FirstName = item.FirstName;
            LastName = item.LastName;
            PhoneNumber = item.PhoneNumber;
            
            trackingManager.IsAction = false;
        }

        private DriverItem itemTmp;

        protected override void OnBeforeSetProperty<TObject, TValue>(TObject item, TValue field, TValue value, string callerName) => itemTmp = DeepCopy();

        protected override void OnAfterSetProperty<TObject, TValue>(TObject item, TValue field, TValue value, string callerName)
        {
            if (trackingManager.IsAction)
                return;
            var oldItem = itemTmp;
            itemTmp = null;
            
            trackingManager.AddAction(() => this.GetTrackingItemUpdate(oldItem));
        }
    }
}