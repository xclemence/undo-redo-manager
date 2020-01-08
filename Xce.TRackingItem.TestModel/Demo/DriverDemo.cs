using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Xce.TrackingItem.TestModel.Base;
using Xce.TrackingItem.TrackingAction;

namespace Xce.TrackingItem.TestModel.Demo
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

        private void OnItemsCollectionChanged<TItem>(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                OnAddItem(sender as ObservableCollection<TItem>, e.NewItems.Cast<TItem>().ToList(), e.NewStartingIndex);

            if (e.OldItems != null)
                OnRemoveItem(sender as ObservableCollection<TItem>, e.OldItems.Cast<TItem>().ToList(), e.OldStartingIndex);
        }

        protected void OnAddItem<TCollection, TValue>(TCollection collection, IList<TValue> items, int position)
         where TCollection : IList<TValue>
        {
            if (!trackingManager.IsAction)
                trackingManager.AddAction(() => new TrackingCollectionUdpate<TCollection, TValue>(collection, items, position, TrackingCollectionUdpateMode.Add));
        }

        protected void OnRemoveItem<TCollection, TValue>(TCollection collection, IList<TValue> items, int position)
        where TCollection : IList<TValue>
        {
            if (!trackingManager.IsAction)
                trackingManager.AddAction(() =>new TrackingCollectionUdpate<TCollection, TValue>(collection, items, position, TrackingCollectionUdpateMode.Remove));
        }

        protected override void OnBeforeSetProperty<TObject, TValue>(TObject item, TValue field, TValue value, string callerName)
        {
            if (!trackingManager.IsAction)
                trackingManager.AddAction(() => item.GetTrackingPropertyUpdateV2(field, value, callerName));
        }
    }
}