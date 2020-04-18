using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Xce.TrackingItem.Fody.TestModel
{
    public class ReferenceCollectionModel
    {
        private readonly TrackingManager trackingManager;

        public ReferenceCollectionModel()
        {
            trackingManager = TrackingManagerProvider.GetDefault();

            TestCollection.CollectionChanged += OnTestCollectionCollectionChanged;
        }

        ~ReferenceCollectionModel()
        {
            TestCollection.CollectionChanged -= OnTestCollectionCollectionChanged;
        }

        private void OnTestCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) =>
            trackingManager.AddActions(TrackingActionFactory.GetCollectionChangedTrackingActionLIst(TestCollection, e));

        public ObservableCollection<int> TestCollection { get; }
    }

    public sealed class ReferenceCollectionModel2
    {
        private readonly TrackingManager trackingManager;

        public ReferenceCollectionModel2()
        {
            trackingManager = TrackingManagerProvider.GetDefault();

            TestCollection.CollectionChanged += OnTestCollectionCollectionChanged;
        }

        ~ReferenceCollectionModel2()
        {
            TestCollection.CollectionChanged -= OnTestCollectionCollectionChanged;
        }

        private void OnTestCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) =>
            trackingManager.AddActions(TrackingActionFactory.GetCollectionChangedTrackingActionLIst(TestCollection, e));

        public ObservableCollection<int> TestCollection { get; set; }
    }
}
