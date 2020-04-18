using System.Collections.ObjectModel;
using Xce.TrackingItem.Attributes;

namespace Xce.TrackingItem.Fody.TestModel
{
    [Tracking]
    public class BaseCollectionModel
    {
        [CollectionTracking]
        public ObservableCollection<int> Items { get; } = new ObservableCollection<int>();
    }
}
