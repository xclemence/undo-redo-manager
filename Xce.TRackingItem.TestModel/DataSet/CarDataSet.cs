﻿using System;
using Xce.TrackingItem.Interfaces;
using Xce.TrackingItem.TestModel.Base;
using Xce.TrackingItem.TrackingAction;

namespace Xce.TrackingItem.TestModel.DataSet
{
    public class CarDataSet : Car, ICopiable<CarDataSet>, ISettable<CarDataSet>, IIdentifiable
    {
        private readonly TrackingManager trackingManager = DataSetTrackingManagerProvider.Instance.Manager;

        public CarDataSet()
        {
            if (!trackingManager.IsAction)
            {
                DataSetTrackingManagerProvider.Instance.DataSet.Cars.Add(this);
                TrackingDataSetCache.Instance.SetDataSet(DataSetTrackingManagerProvider.Instance.DataSet);
            }
        }

        public Guid Id { get; private set; } = Guid.NewGuid();

        public override void Initialize() => DataSetTrackingManagerProvider.Instance.DataSet.Cars.Add(this);

        public CarDataSet DeepCopy()
        {
            using (var scope = new StopTrackingScope(trackingManager))
            {
                var copy = this.DeepCopyCar();
                copy.Id = Id;

                return copy;
            }
        }

        public void Set(CarDataSet item)
        {
            using (var scope = new StopTrackingScope(trackingManager))
            {
                this.SetCar(item);
                Id = item.Id;
            }
        }

        protected override void OnAfterSetProperty<TObject, TValue>(TObject item, TValue field, TValue value, string callerName)
        {
            if (trackingManager.IsAction)
                return;

            trackingManager.AddAction(() =>
            {
                using (var scope = new StopTrackingScope(trackingManager))
                    return DataSetTrackingManagerProvider.Instance.DataSet.GetTrackingDatSetUpdate();
            });
        }
    }
}