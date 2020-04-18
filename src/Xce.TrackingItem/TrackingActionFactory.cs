using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Xce.TrackingItem.Interfaces;
using Xce.TrackingItem.TrackingAction;

namespace Xce.TrackingItem
{
    public static class TrackingActionFactory
    {
        private static readonly IDictionary<Type, IDictionary<string, object>> setterMethodCache = new Dictionary<Type, IDictionary<string, object>>();

        public static void ClearCache() => setterMethodCache.Clear();

        private static Action<TObject, TValue> GetSetter<TObject, TValue>(string propertyName)
        {
            var key = typeof(TObject);

            if (setterMethodCache.TryGetValue(key, out var properties))
            {
                if (properties.TryGetValue(propertyName, out var setter))
                    return setter as Action<TObject, TValue>;
            }
            else
            {
                setterMethodCache[key] = new Dictionary<string, object>();
            }

            var setterMethod = (Action<TObject, TValue>)Delegate.CreateDelegate(typeof(Action<TObject, TValue>), null, typeof(TObject).GetProperty(propertyName).GetSetMethod());
            setterMethodCache[key][propertyName] = setterMethod;

            return setterMethod;
        }

        public static ITrackingAction GetTrackingPropertyUpdate<TObject, TValue>(this TObject item, TValue field, TValue value, string propertyName)
        {
            var setterMethod = GetSetter<TObject, TValue>(propertyName);
            return new PropertyTrackingAction<TObject, TValue>(field, value, item, setterMethod);
        }

        public static ITrackingAction GetTrackingPropertyUpdate<TObject, TValue>(this TObject item, TValue field, TValue value, Action<TObject, TValue> setter) =>
            new PropertyTrackingAction<TObject, TValue>(field, value, item, setter);

        public static Func<ITrackingAction> GetTrackingPropertyUpdateFunc<TObject, TValue>(this TObject item, TValue field, TValue value, Action<TObject, TValue> setter) =>
            () => new PropertyTrackingAction<TObject, TValue>(field, value, item, setter);

        public static ITrackingAction GetTrackingPropertyUpdateV2<TObject, TValue>(this TObject item, TValue field, TValue value, string propertyName) =>
            new PropertyTrackingAction<TObject, TValue>(field, value, item, (x, y) => GetSetter<TObject, TValue>(propertyName)(x, y));

        public static ITrackingAction GetTrackingItemUpdate<TObject>(this TObject referenceItem)
            where TObject : class, ICopiable<TObject>, ISettable<TObject> => new ItemTrackingAction<TObject>(referenceItem);

        public static ITrackingAction GetTrackingDatSetUpdate<TDataSet>(this TDataSet currentDataSet)
            where TDataSet : class, ICopiable<TDataSet>, ISettable<TDataSet> => new DataSetTrackingAction<TDataSet>(currentDataSet);

        public static IEnumerable<ITrackingAction> GetCollectionChangedTrackingAction<TValue>(this IList<TValue> collection, NotifyCollectionChangedEventArgs e)
        {

            if (e.NewItems != null)
            {
                var items = e.NewItems.Cast<TValue>().ToList();
                yield return new CollectionTrackingAction<IList<TValue>, TValue>(collection, items, e.NewStartingIndex, TrackingCollectionUpdateMode.Add);
            }

            if (e.OldItems != null)
            {
                var items = e.OldItems.Cast<TValue>().ToList();
                yield return new CollectionTrackingAction<IList<TValue>, TValue>(collection, items, e.OldStartingIndex, TrackingCollectionUpdateMode.Remove);
            }
        }

        public static IList<ITrackingAction> GetCollectionChangedTrackingActionLIst<TValue>(this IList<TValue> collection, NotifyCollectionChangedEventArgs e) =>
            GetCollectionChangedTrackingAction(collection, e).ToList();
    }
}
