using Xce.TrackingItem.Interfaces;
using Xce.TrackingItem.TrackingAction;
using Xce.TrackingItem.TestModel.Base;

namespace Xce.TrackingItem.TestModel.ItemSave
{
    public class AddressItem : Address, ICopiable<AddressItem>, ISettable<AddressItem>
    {
        private readonly TrackingManager trackingManager = ItemTrackingManagerProvider.Instance.Manager;

        public AddressItem(bool tracking = false) : base()
        {
            if (!trackingManager.IsAction || tracking)
                TrackingItemCache.Instance.SetCacheObject(this);
        }

        public AddressItem DeepCopy()
        {
            using var scope = new StopTrackingScope(trackingManager);
            return this.DeepCopyAddress();
        }

        public void Set(AddressItem item)
        {
            using var scope = new StopTrackingScope(trackingManager);
            this.SetAddress(item);
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
