using Xce.TrackingItem.Interfaces;

namespace Xce.TrackingItem.TrackingAction
{
    public class TrackingDataSetCache
    {
        public static TrackingDataSetCache Instance { get; } = new TrackingDataSetCache();

        private object currentDataSet;

        public T SetDataSet<T>(T dataSet) where T : class, ICopiable<T>
        {
            var item = dataSet.DeepCopy();
            currentDataSet = item;
            return item;
        }

        public T GetDataSet<T>() where T : class => currentDataSet as T;


        public void Clear() => currentDataSet = null;
    }

    public class DataSetTrackingAction<TObject> : ITrackingAction
        where TObject : class, ICopiable<TObject>, ISettable<TObject>
    {
        public DataSetTrackingAction(TObject newItem)
        {
            OldDataSet = TrackingDataSetCache.Instance.GetDataSet<TObject>();
            NewDataSet = TrackingDataSetCache.Instance.SetDataSet(newItem);

            ReferenceDataSet = newItem;
        }

        public TObject OldDataSet { get; }
        public TObject NewDataSet { get; }
        public TObject ReferenceDataSet { get; }

        public void Apply() => ReferenceDataSet.SetItem(NewDataSet);

        public void Revert() => ReferenceDataSet.SetItem(OldDataSet);

        public override string ToString() => $"Data Set: {ReferenceDataSet.GetType().Name}";
    }
}
