﻿using System.Collections.Generic;
using Xce.TrackingItem.TestModel.Base;
using Xce.TrackingItem.TestModel.DataSet;
using Xce.TrackingItem.TestModel.ItemSave;
using Xce.TrackingItem.TestModel.PropertySave;

namespace Xce.TrackingItem.TestModel.Multi
{
    public class AddressMulti : Address
    {
        private readonly IList<AbsctractModel> testItems = new List<AbsctractModel>
        {
            new AddressProperty(),
            new AddressItem(),
            new AddressDataSet(),
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
