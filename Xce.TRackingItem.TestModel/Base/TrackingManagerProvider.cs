using System.Collections.Generic;
using System.Linq;
using Xce.TrackingItem.TestModel.DataSet;
using Xce.TrackingItem.TrackingAction;

namespace Xce.TrackingItem.TestModel.Base
{
    public interface ITrackingManagerProvider
    {
        void Clear();
        IEnumerable<TrackingManager> GetTrackingManagers();
        void RemakeAllMulti();
        void RemakeMulti();
        void RevertAlltMulti();
        void RevertMulti();
    }

    public abstract class TrackingManagerProvider : ITrackingManagerProvider
    {
        internal TrackingManager Manager { get; } = new TrackingManager();

        public void RevertMulti()
        {
            var trackingManagers = GetTrackingManagers().ToList();
            foreach (var item in trackingManagers)
                item.Revert();
        }

        public void RemakeMulti()
        {
            var trackingManagers = GetTrackingManagers().ToList();

            foreach (var item in trackingManagers)
                item.Remake();
        }

        public void RemakeAllMulti()
        {
            var trackingManagers = GetTrackingManagers().ToList();

            var referenceManager = trackingManagers.First();

            while (referenceManager.CanRemake)
                RemakeMulti();
        }

        public void RevertAlltMulti()
        {
            var trackingManagers = GetTrackingManagers().ToList();

            var referenceManager = trackingManagers.First();

            while (referenceManager.CanRevert)
                RevertMulti();
        }

        public void Clear()
        {
            var trackingManagers = GetTrackingManagers().ToList();

            foreach (var item in trackingManagers)
                item.CurrentScope.Clear();

            TrackingItemCache.Instance.Clear();
            DataSetTrackingManagerProvider.Instance.DataSet?.Clear();
            TrackingActionFactory.ClearCache();
        }

        public virtual IEnumerable<TrackingManager> GetTrackingManagers()
        {
            yield return Manager;
        }
    }

}
