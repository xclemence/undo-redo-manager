using Xce.TrackingItem;
using Xce.TrackingItem.TrackingAction;
using Xce.TRackingItem.TestModel.Base;

namespace Xce.TRackingItem.TestModel.ItemSave
{
    public class CarItem : Car, ICopiable<CarItem>, ISettable<CarItem>
    {
        private readonly TrackingManager trackingManager = ItemTrackingManagerProvider.Instance.Manager;

        public CarItem() : base()
        {
            if (!trackingManager.IsAction)
                TrackingItemCache.Instance.SetCacheObject(this);
        }

        public CarItem DeepCopy()
        {
            trackingManager.IsAction = true;
            var copy =  new CarItem
            {
                Fuel = Fuel,
                Manufacturer = Manufacturer,
                Model = Model,
                Type = Type,
                Vin = Vin,
            };

            trackingManager.IsAction = false;

            return copy;
        }

        public void Set(CarItem item)
        {
            trackingManager.IsAction = true;

            Fuel = item.Fuel;
            Manufacturer = item.Manufacturer;
            Model = item.Model;
            Type = item.Type;
            Vin = item.Vin;

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