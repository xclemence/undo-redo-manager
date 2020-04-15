using Xce.TrackingItem;
using Xce.TrackingItem.Interfaces;
using Xce.TrackingItem.TrackingAction;
using Xce.UndoRedo.Models.Base;

namespace Xce.UndoRedo.Models.ItemSave
{
    public class AddressItem : Address, ICopiable<AddressItem>, ISettable<AddressItem>
    {
        private readonly TrackingManager trackingManager = ItemTrackingManagerProvider.Instance.Manager;

        public AddressItem()
        {
            if (!trackingManager.IsAction)
                TrackingItemCache.Instance.SetCacheObject(this);
        }

        public override void Initialize() => TrackingItemCache.Instance.SetCacheObject(this);

        public AddressItem DeepCopy()
        {
            using (var scope = new StopTrackingScope(trackingManager))
                return this.DeepCopyAddress();
        }

        public void SetItem(AddressItem item)
        {
            using (var scope = new StopTrackingScope(trackingManager))
                this.SetAddress(item);
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
