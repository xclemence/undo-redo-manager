using System;
using Xce.TrackingItem.Interfaces;
using Xce.TrackingItem.TestModel.Base;
using Xce.TrackingItem.TrackingAction;

namespace Xce.TrackingItem.TestModel.DataSet
{
    public class AddressDataSet : Address, ICopiable<AddressDataSet>, ISettable<AddressDataSet>, IIdentifiable
    {
        private readonly TrackingManager trackingManager = DataSetTrackingManagerProvider.Instance.Manager;

        public AddressDataSet()
        {
            if (!trackingManager.IsAction)
            {
                DataSetTrackingManagerProvider.Instance.DataSet.Addresses.Add(this);
                TrackingDataSetCache.Instance.SetDataSet(DataSetTrackingManagerProvider.Instance.DataSet);
            }
        }

        public Guid Id { get; private set; } = Guid.NewGuid();
        
        public override void Initialize() => DataSetTrackingManagerProvider.Instance.DataSet.Addresses.Add(this);

        public AddressDataSet DeepCopy()
        {
            using (var scope = new StopTrackingScope(trackingManager))
            {

                var copy = this.DeepCopyAddress();
                copy.Id = this.Id;

                return copy;
            }
        }

        public void Set(AddressDataSet item)
        {
            using (var scope = new StopTrackingScope(trackingManager))
            {
                this.SetAddress(item);
                this.Id = item.Id;
            }
        }
        
        protected override void OnAfterSetProperty<TObject, TValue>(TObject item, TValue field, TValue value, string callerName)
        {
            trackingManager.AddAction(() =>
            {
                using (var scope = new StopTrackingScope(trackingManager))
                    return DataSetTrackingManagerProvider.Instance.DataSet.GetTrackingDatSetUpdate();
            });
        }
    }
}
