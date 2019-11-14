using System;
using System.Collections;
using System.Collections.Generic;
using Xce.TrackingItem.TrackingAction;

namespace Xce.TrackingItem
{
    public static class TrackingActionFactory
    {
        private static IDictionary<Type, IDictionary<string, object>> setteMethodCache = new Dictionary<Type, IDictionary<string, object>>();

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
            //var setterMethod = (Action<TObject, TValue>)Delegate.CreateDelegate(typeof(Action<TObject, TValue>), null, typeof(TObject).GetProperty(propertyName).GetSetMethod());
            var setterMethod = GetSetter<TObject, TValue>(propertyName);
            return new TrackingPropertyUpdate<TObject, TValue>(field, value, item, setterMethod);
        }

        public static ITrackingAction GetTrackingItemUpdate<TObject>(this TObject referenceItem, TObject newItem, TObject oldItem)
            where TObject : ICopiable<TObject>, ISettable<TObject>
        {
            return new TrackingItemUpdate<TObject>(referenceItem, newItem, oldItem);
        }
    }
}
