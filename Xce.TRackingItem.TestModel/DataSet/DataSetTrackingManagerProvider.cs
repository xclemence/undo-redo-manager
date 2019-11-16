using Xce.TrackingItem.TestModel.Base;

namespace Xce.TrackingItem.TestModel.DataSet
{
    public class DataSetTrackingManagerProvider : TrackingManagerProvider
    {
        public DataSetTrackingManagerProvider() => DataSet = new WorldDataSet(Manager);

        public static DataSetTrackingManagerProvider Instance { get; } = new DataSetTrackingManagerProvider();

        public WorldDataSet DataSet { get; }
    }
}
