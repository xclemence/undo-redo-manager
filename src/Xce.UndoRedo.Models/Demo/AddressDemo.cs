﻿using Xce.TrackingItem;
using Xce.UndoRedo.Models.Base;

namespace Xce.UndoRedo.Models.Demo
{
    public class AddressDemo : Address
    {
        private readonly TrackingManager trackingManager = DemoTrackingManagerProvider.Instance.Manager;

        protected override void OnBeforeSetProperty<TObject, TValue>(TObject item, TValue field, TValue value, string callerName) =>
           trackingManager.AddAction(() => item.GetTrackingPropertyUpdateV2(field, value, callerName));
    }
}
