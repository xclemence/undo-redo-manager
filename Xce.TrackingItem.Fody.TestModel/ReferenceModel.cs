using System;
using Xce.TrackingItem.TrackingAction;

namespace Xce.TrackingItem.Fody.TestModel
{
    public class ReferenceModel
    {
        private TrackingManager trackingManager;
        private int value;
        public ReferenceModel()
        {
            trackingManager = TrackingManagerProvider.GetDefault();
        }

        public int Value
        {
            get => value;
            set
            {
                trackingManager.AddAction(new TrackingPropertyUpdate<ReferenceModel, int>(this.Value, value, this, TrackingValue));
                this.value = value;
            }
        }

        private static void TrackingValue(ReferenceModel model, int value)
        {
            model.Value = value;
        }

    }
}
