using System;
using System.Collections;
using System.Collections.Generic;

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

    public class TrackingItemCache
    {
        public static TrackingItemCache Instance { get; } = new TrackingItemCache();

        private IDictionary<object, object> cache = new Dictionary<object, object>();

        public T GetCacheObject<T>(T referenceObject) where T: class
        {
            if (cache.TryGetValue(referenceObject, out var item))
                return item as T;

            return null;
        }

        public T SetCacheObject<T>(T referenceObject) where T : class, ICopiable<T>
        {
            var item = referenceObject.DeepCopy();
            cache[referenceObject] = item;

            return item;
        }

        public T SetCacheObject<T>(T referenceObject, T item) where T : class, ICopiable<T>
        {
            cache[referenceObject] = item;
            return item;
        }

        public void Clear() => cache.Clear();
    }

    public class TrackingItemUpdate<TObject> : ITrackingAction
        where TObject : ICopiable<TObject>, ISettable<TObject>
    {
        public TrackingItemUpdate(TObject referenceItem, TObject newItem, TObject oldItem)
        {
            OldItem = oldItem;
            ItemReference = referenceItem;
            NewItem = newItem;
        }

        public TObject ItemReference { get; }
        public TObject OldItem { get; }
        public TObject NewItem { get; private set; }

        public void Apply() => ItemReference.Set(NewItem);

        public void Revert() => ItemReference.Set(OldItem);
    }
}
