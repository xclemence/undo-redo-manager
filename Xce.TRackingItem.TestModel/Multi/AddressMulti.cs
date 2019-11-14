﻿using System.Collections.Generic;
using Xce.TrackingItem;
using Xce.TRackingItem.TestModel.Base;
using Xce.TRackingItem.TestModel.ItemSave;
using Xce.TRackingItem.TestModel.PropertySave;

namespace Xce.TRackingItem.TestModel.Multi
{
    public class AddressMulti : Address
    {
        private readonly IList<AbsctractModel> testItems = new List<AbsctractModel>
        {
            new AddressProperty(),
            new AddressItem()
        };

        private readonly TrackingManager trackingManager = MultiTrackingManagerProvider.Instance.Manager;

        protected override void OnBeforeSetProperty<TObject, TValue>(TObject item, TValue field, TValue value, string callerName)
        {
            if (!trackingManager.IsAction)
            {
                trackingManager.AddAction(item.GetTrackingPropertyUpdate(field, value, callerName));

                foreach (var test in testItems)
                {
                    var property = test.GetType().GetProperty(callerName);
                    property.SetValue(test, value, new object[] { });
                }
            }
        }
    }
}
