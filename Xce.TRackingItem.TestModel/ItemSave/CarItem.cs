using Xce.TrackingItem.Interfaces;
using Xce.TrackingItem.TrackingAction;
using Xce.TrackingItem.TestModel.Base;

namespace Xce.TrackingItem.TestModel.ItemSave
{
    public class CarItem : Car, ICopiable<CarItem>, ISettable<CarItem>
    {
        private readonly TrackingManager trackingManager = ItemTrackingManagerProvider.Instance.Manager;

        public CarItem()
        {
            if (!trackingManager.IsAction)
                TrackingItemCache.Instance.SetCacheObject(this);
        }

        public override void Initialize() => TrackingItemCache.Instance.SetCacheObject(this);

        public CarItem DeepCopy()
        {
            using (var scope = new StopTrackingScope(trackingManager))
                return this.DeepCopyCar();
        }

        public void Set(CarItem item)
        {
            using (var scope = new StopTrackingScope(trackingManager))
                this.SetCar(item);
        }

        protected override void OnAfterSetProperty<TObject, TValue>(TObject item, TValue field, TValue value, string callerName)
        {
            if (trackingManager.IsAction)
                return;

            trackingManager.AddAction(() =>
            {
                using (var scope = new StopTrackingScope(trackingManager))
                    return this.GetTrackingItemUpdate();
            });
        }
    }
}