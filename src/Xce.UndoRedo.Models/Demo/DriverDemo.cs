using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Xce.TrackingItem;
using Xce.UndoRedo.Models.Base;

namespace Xce.UndoRedo.Models.Demo
{
    public class DriverDemo : Driver<CarDemo, AddressDemo>
    {
        private readonly TrackingManager trackingManager = DemoTrackingManagerProvider.Instance.Manager;

        public DriverDemo()
        {

            Cars.CollectionChanged += OnItemsCollectionChanged<CarDemo>;
            Addresses.CollectionChanged += OnItemsCollectionChanged<AddressDemo>;
        }

        ~DriverDemo()
        {
            Cars.CollectionChanged -= OnItemsCollectionChanged<CarDemo>;
            Addresses.CollectionChanged -= OnItemsCollectionChanged<AddressDemo>;
        }

        private void OnItemsCollectionChanged<TItem>(object sender, NotifyCollectionChangedEventArgs e) =>
            trackingManager.AddActions((sender as IList<TItem>)?.GetCollectionChangedTrackingAction(e).ToList());

        protected override void OnBeforeSetProperty<TObject, TValue>(TObject item, TValue field, TValue value, string callerName) =>
            trackingManager.AddAction(() => item.GetTrackingPropertyUpdateV2(field, value, callerName));
    }
}