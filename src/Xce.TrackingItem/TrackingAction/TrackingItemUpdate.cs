using System.Collections.Generic;
using Xce.TrackingItem.Interfaces;

namespace Xce.TrackingItem.TrackingAction
{
    public class TrackingItemCache
    {
        public static TrackingItemCache Instance { get; } = new TrackingItemCache();

        private readonly IDictionary<object, object> cache = new Dictionary<object, object>();

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
        where TObject : class, ICopiable<TObject>, ISettable<TObject>
    {
        public TrackingItemUpdate(TObject referenceItem)
        {
            ReferenceItem = referenceItem;
            OldItem = TrackingItemCache.Instance.GetCacheObject(referenceItem);
            NewItem = TrackingItemCache.Instance.SetCacheObject(referenceItem);
        }

        public TObject ReferenceItem { get; }
        public TObject OldItem { get; }
        public TObject NewItem { get; private set; }

        public void Apply()
        {
            ReferenceItem.Set(NewItem);
            TrackingItemCache.Instance.SetCacheObject(ReferenceItem, NewItem);
        }

        public void Revert()
        {
            ReferenceItem.Set(OldItem);
            TrackingItemCache.Instance.SetCacheObject(ReferenceItem, OldItem);
        }
    }
}
