using Xce.TrackingItem.Interfaces;
using Xce.TrackingItem.TrackingAction;
using Xce.TrackingItem.TestModel.Base;

namespace Xce.TrackingItem.TestModel.ItemSave
{
    public class DriverItem : Driver<CarItem, AddressItem>, ICopiable<DriverItem>, ISettable<DriverItem>
    {
        private readonly TrackingManager trackingManager = ItemTrackingManagerProvider.Instance.Manager;

        public DriverItem() : base()
        {
            if (!trackingManager.IsAction)
                TrackingItemCache.Instance.SetCacheObject(this);
        }

        public override void Initialize() => TrackingItemCache.Instance.SetCacheObject(this);

        public DriverItem DeepCopy()
        {
            using var scope = new StopTrackingScope(trackingManager);
            return this.DeepCopyDriver<DriverItem, CarItem, AddressItem>();
        }

        public void Set(DriverItem item)
        {
            using var scope = new StopTrackingScope(trackingManager);
            this.SetDriver<DriverItem, CarItem, AddressItem>(item);
        }

        protected override void OnAfterSetProperty<TObject, TValue>(TObject item, TValue field, TValue value, string callerName)
        {
            if (trackingManager.IsAction)
                return;

            trackingManager.AddAction(() =>
            {
                using var scope = new StopTrackingScope(trackingManager);
                return this.GetTrackingItemUpdate();
            });
        }
    }
}