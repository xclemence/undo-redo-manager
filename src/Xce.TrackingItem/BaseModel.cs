using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xce.TrackingItem.TrackingAction;

namespace Xce.TrackingItem
{
    public class ModelBase : INotifyPropertyChanged
    {
        private readonly TrackingManager trackingManager;

        public event PropertyChangedEventHandler PropertyChanged;

        public ModelBase(TrackingManager trackingManager)
        {
            this.trackingManager = trackingManager;
        }

        protected bool SetProperty<TObject, TValue>(TObject item,  ref TValue field, TValue value, [CallerMemberName] string callerName = null)
            where TObject : ModelBase
        {
            
            if (field == null && value == null || (field?.Equals(value) ?? false))
                return false;

            if(!trackingManager.IsAction)
                trackingManager.AddAction(item.GetTrackingPropertyUpdate(field, value, callerName));

            field = value;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(callerName));

            return true;
        }

        protected void OnAddItem<TCollection, TValue>(TCollection collection, IList<TValue> items, int position)
          where TCollection : IList<TValue>
        {
            if (!trackingManager.IsAction)
                trackingManager.AddAction(new CollectionTrackingAction<TCollection, TValue>(collection, items, position, TrackingCollectionUdpateMode.Add));
        }

        protected void OnRemoveItem<TCollection, TValue>(TCollection collection, IList<TValue> items, int position)
        where TCollection : IList<TValue>
        {
            if (!trackingManager.IsAction)
                trackingManager.AddAction(new CollectionTrackingAction<TCollection, TValue>(collection, items, position, TrackingCollectionUdpateMode.Remove));
        }
    }
}
