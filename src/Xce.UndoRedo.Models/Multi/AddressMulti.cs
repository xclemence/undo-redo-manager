using System.Collections.Generic;
using Xce.TrackingItem;
using Xce.UndoRedo.Models.Base;
using Xce.UndoRedo.Models.DataSet;
using Xce.UndoRedo.Models.Fody;
using Xce.UndoRedo.Models.ItemSave;
using Xce.UndoRedo.Models.PropertySave;

namespace Xce.UndoRedo.Models.Multi
{
    public class AddressMulti : Address
    {
        private readonly IList<AbsctractModel> testItems = new List<AbsctractModel>
        {
            new AddressProperty(),
            new AddressItem(),
            new AddressDataSet(),
            new AddressFody()
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
