using System;
using Xce.TrackingItem.Interfaces;
using Xce.TrackingItem.TestModel.Base;
using Xce.TrackingItem.TrackingAction;

namespace Xce.TrackingItem.TestModel.DataSet
{
    public class DriverDataSet : Driver<CarDataSet, AddressDataSet>, ICopiable<DriverDataSet>, ISettable<DriverDataSet>, IIdentifiable
    {
        private readonly TrackingManager trackingManager = DataSetTrackingManagerProvider.Instance.Manager;

        public DriverDataSet()
        {
            if (!trackingManager.IsAction)
            {
                DataSetTrackingManagerProvider.Instance.DataSet.Drivers.Add(this);
                TrackingDataSetCache.Instance.SetDataSet(DataSetTrackingManagerProvider.Instance.DataSet);
            }
        }
     
        public Guid Id { get; private set; } = Guid.NewGuid();

        public override void Initialize() => DataSetTrackingManagerProvider.Instance.DataSet.Drivers.Add(this);

        public DriverDataSet DeepCopy()
        {
            using (var scope = new StopTrackingScope(trackingManager))
            {

                var copy = this.DeepCopyDriver<DriverDataSet, CarDataSet, AddressDataSet>();
                copy.Id = Id;

                return copy;
            }
        }

        public void Set(DriverDataSet item)
        {
            using (var scope = new StopTrackingScope(trackingManager))
            {
                this.SetDriver<DriverDataSet, CarDataSet, AddressDataSet>(item);
                Id = item.Id;
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