using System;
using System.Collections.ObjectModel;
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

            TestCollection.CollectionChanged += OnTestCollectionCollectionChanged;
        }

        private void OnTestCollectionCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => throw new NotImplementedException();

        public int Value
        {
            get => value;
            set
            {
                trackingManager.AddAction(new PropertyTrackingAction<ReferenceModel, int>(this.Value, value, this, TrackingValue));
                this.value = value;
            }
        }

        public int Value2
        {
            get => value;
            set
            {
                this.value = value;

                if(Value2 != value)
                {
                    trackingManager.AddAction(TrackingActionFactory.GetTrackingPropertyUpdateFunc(this, Value, value, TrackingValue));
                }
            }
        }

        private static void TrackingValue(ReferenceModel model, int value)
        {
            model.Value = value;
        }

        public ObservableCollection<int> TestCollection { get; set; }

    }
}
