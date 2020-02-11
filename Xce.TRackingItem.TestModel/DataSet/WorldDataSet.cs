using System.Collections.Generic;
using Xce.TrackingItem.Interfaces;
using Xce.TrackingItem.TestModel.Base;
using Xce.TrackingItem.TrackingAction;

namespace Xce.TrackingItem.TestModel.DataSet
{
    public class WorldDataSet : ICopiable<WorldDataSet>, ISettable<WorldDataSet>
    {
        private readonly TrackingManager trackingManager;

        internal WorldDataSet(TrackingManager manager)
        {
            trackingManager = manager;

            Drivers = new List<DriverDataSet>();
            Cars = new List<CarDataSet>();
            Addresses = new List<AddressDataSet>();

            if(!trackingManager.IsAction)
                TrackingDataSetCache.Instance.SetDataSet(this);
        }

        public IList<DriverDataSet> Drivers { get; set; }
        public IList<CarDataSet> Cars { get; set; }
        public IList<AddressDataSet> Addresses { get; set; }

        public WorldDataSet DeepCopy()
        {
            using (var scope = new StopTrackingScope(trackingManager))
            {
                var copy = new WorldDataSet(trackingManager)
                {
                    Drivers = Drivers.DeepCopy(),
                    Cars = Cars.DeepCopy(),
                    Addresses = Addresses.DeepCopy()
                };

                return copy;
            }
        }

        public void Set(WorldDataSet item)
        {
            using (var scope = new StopTrackingScope(trackingManager))
            {

                Drivers.Set(item.Drivers);
                Cars.Set(item.Cars);
                Addresses.Set(item.Addresses);
            }
        }

        public void Clear()
        {
            Drivers.Clear();
            Cars.Clear();
            Addresses.Clear();
        }
    }
}
