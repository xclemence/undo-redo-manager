﻿using Xce.TrackingItem;
using Xce.TrackingItem.Interfaces;
using Xce.TrackingItem.TrackingAction;
using Xce.UndoRedo.Models.Base;

namespace Xce.UndoRedo.Models.ItemSave
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

        public void SetItem(CarItem item)
        {
            using (var scope = new StopTrackingScope(trackingManager))
                this.SetCar(item);
        }

        protected override void OnAfterSetProperty<TObject, TValue>(TObject item, TValue field, TValue value, string callerName)
        {
            trackingManager.AddAction(() =>
            {
                using (var scope = new StopTrackingScope(trackingManager))
                    return this.GetTrackingItemUpdate();
            });
        }
    }
}