using Xce.UndoRedo.Models.Base;

namespace Xce.UndoRedo.Models.DataSet
{
    public class DataSetTrackingManagerProvider : BaseTrackingManagerProvider
    {
        public DataSetTrackingManagerProvider() => DataSet = new WorldDataSet(Manager);

        public static DataSetTrackingManagerProvider Instance { get; } = new DataSetTrackingManagerProvider();

        public WorldDataSet DataSet { get; }
    }
}
