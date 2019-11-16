﻿using Xce.TrackingItem.TestModel.Base;

namespace Xce.TrackingItem.TestModel.Demo
{
    public class CarDemo : Car
    {
        private readonly TrackingManager trackingManager = DemoTrackingManagerProvider.Instance.Manager;

        protected override void OnBeforeSetProperty<TObject, TValue>(TObject item, TValue field, TValue value, string callerName)
        {
            if (!trackingManager.IsAction)
                trackingManager.AddAction(() => item.GetTrackingPropertyUpdate(field, value, callerName));
        }
    }
}