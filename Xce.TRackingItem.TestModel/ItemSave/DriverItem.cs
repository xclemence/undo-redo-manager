using Xce.TrackingItem;
using Xce.TrackingItem.TrackingAction;
using Xce.TRackingItem.TestModel.Base;

namespace Xce.TRackingItem.TestModel.ItemSave
{
    public class DriverItem : Driver<CarItem, AddressItem>, ICopiable<DriverItem>, ISettable<DriverItem>
    {
        private readonly TrackingManager trackingManager = ItemTrackingManagerProvider.Instance.Manager;

        public DriverItem() : base()
        {
            if (!trackingManager.IsAction)
                TrackingItemCache.Instance.SetCacheObject(this);
        }

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

            TrackingItemCache.Instance.SetCacheObject(this, item);

            trackingManager.IsAction = false;
        }

        protected override void OnAfterSetProperty<TObject, TValue>(TObject item, TValue field, TValue value, string callerName)
        {
            if (trackingManager.IsAction)
                return;

            trackingManager.AddAction(() =>
            {
                trackingManager.IsAction = true;

                var oldItem = TrackingItemCache.Instance.GetCacheObject(this);
                var newItem = TrackingItemCache.Instance.SetCacheObject(this);

                var item = this.GetTrackingItemUpdate(newItem, oldItem);

                trackingManager.IsAction = false;

                return item;
            });
        }
    }
}