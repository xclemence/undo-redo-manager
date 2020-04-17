using System.Collections.Generic;

namespace Xce.TrackingItem.TrackingAction
{
    public enum TrackingCollectionUdpateMode
    {
        Add,
        Remove,
    }

    public class CollectionTrackingAction<TCollection, TValue> : ITrackingAction
       where TCollection : IList<TValue>
    {
        public CollectionTrackingAction(TCollection collection, IList<TValue> items, int position, TrackingCollectionUdpateMode mode)
        {
            Items = items;
            Position = position;
            Collection = collection;
            Mode = mode;
        }

        public IList<TValue> Items { get; }
        public int Position { get; }

        public TCollection Collection { get; }

        public TrackingCollectionUdpateMode Mode { get; }

        protected void AddItems()
        {
            var currentPostion = Position;

            foreach (var item in Items)
            {
                Collection.Insert(currentPostion, item);
                ++currentPostion;
            }
        }

        protected void RemoveItems()
        {
            foreach (var item in Items)
            {
                Collection.Remove(item);
            }
        }

        public void Apply() 
        {
            if (Mode == TrackingCollectionUdpateMode.Remove)
                RemoveItems();
            else
                AddItems();
        }

        public void Revert()
        {
            if (Mode == TrackingCollectionUdpateMode.Remove)
                AddItems();
            else
                RemoveItems();
        }

        public override string ToString() => $"Collection: {Collection.GetType().Name}, {Mode}";
    }
}
