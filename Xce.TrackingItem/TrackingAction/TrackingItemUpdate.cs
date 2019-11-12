using System;

namespace Xce.TrackingItem.TrackingAction
{
    public interface ICopiable<out T> where T : ICopiable<T>
    {
        T DeepCopy();
    }

    public interface ISettable<in T> where T : ISettable<T>
    {
        void Set(T item);
    }

    public class TrackingItemUpdate<TObject> : ITrackingAction
        where TObject : ICopiable<TObject>, ISettable<TObject>
    {
        public TrackingItemUpdate(TObject oldItem, TObject newItem)
        {
            OldItem = oldItem;
            NewItem = newItem.DeepCopy();

            ItemReference = newItem;
        }

        public TObject ItemReference { get; }
        public TObject OldItem { get; }
        public TObject NewItem { get; }

        public void Apply() => ItemReference.Set(NewItem);

        public void Revert() => ItemReference.Set(OldItem);
    }
}
