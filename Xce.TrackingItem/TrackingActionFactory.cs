﻿using System;
using Xce.TrackingItem.TrackingAction;

namespace Xce.TrackingItem
{
    public static class TrackingActionFactory
    {
        static public ITrackingAction GetTrackingPropertyUpdate<TObject, TValue>(this TObject item, TValue field, TValue value, string propertyName)
        {
            var setterMethod = (Action<TObject, TValue>)Delegate.CreateDelegate(typeof(Action<TObject, TValue>), null, typeof(TObject).GetProperty(propertyName).GetSetMethod());
            return new TrackingPropertyUpdate<TObject, TValue>(field, value, item, setterMethod);
        }
    }
}
