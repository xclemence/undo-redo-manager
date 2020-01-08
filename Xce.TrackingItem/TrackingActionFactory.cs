using System;
using System.Collections.Generic;
using Xce.TrackingItem.Interfaces;
using Xce.TrackingItem.TrackingAction;

namespace Xce.TrackingItem
{
    public static class TrackingActionFactory
    {
        private static readonly IDictionary<Type, IDictionary<string, object>> setteMethodCache = new Dictionary<Type, IDictionary<string, object>>();

        public static void ClearCache() => setteMethodCache.Clear();

        private static Action<TObject, TValue> GetSetter<TObject, TValue>(string propertyName)
        {
            var key = typeof(TObject);

            if (setteMethodCache.TryGetValue(key, out var properties))
            {
                if (properties.TryGetValue(propertyName, out var setter))
                    return setter as Action<TObject, TValue>;
            }
            else
            {
                setteMethodCache[key] = new Dictionary<string, object>();
            }
                
            var setterMethod = (Action<TObject, TValue>)Delegate.CreateDelegate(typeof(Action<TObject, TValue>), null, typeof(TObject).GetProperty(propertyName).GetSetMethod());
            setteMethodCache[key][propertyName] = setterMethod;

            return setterMethod;
        }

        public static ITrackingAction GetTrackingPropertyUpdate<TObject, TValue>(this TObject item, TValue field, TValue value, string propertyName)
        {
            var setterMethod = GetSetter<TObject, TValue>(propertyName);
            return new TrackingPropertyUpdate<TObject, TValue>(field, value, item, setterMethod);
        }
        public static ITrackingAction GetTrackingPropertyUpdateV2<TObject, TValue>(this TObject item, TValue field, TValue value, string propertyName)
        {
            return new TrackingPropertyUpdate<TObject, TValue>(field, value, item, (x, y) => GetSetter<TObject, TValue>(propertyName)(x, y));
        }

        public static ITrackingAction GetTrackingItemUpdate<TObject>(this TObject referenceItem)
            where TObject : class, ICopiable<TObject>, ISettable<TObject>
        {
            return new TrackingItemUpdate<TObject>(referenceItem);
        }

        public static ITrackingAction GetTrackingDatSetUpdate<TDataSet>(this TDataSet currentDataSet)
            where TDataSet : class, ICopiable<TDataSet>, ISettable<TDataSet>
        {
            return new TrackingDataSetUpdate<TDataSet>(currentDataSet);
        }
    }
}
