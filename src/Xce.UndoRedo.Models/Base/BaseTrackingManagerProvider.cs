using System.Collections.Generic;
using System.Linq;
using Xce.UndoRedo.Models.DataSet;
using Xce.TrackingItem.TrackingAction;
using Xce.TrackingItem;

namespace Xce.UndoRedo.Models.Base
{
    public interface ITrackingManagerProvider
    {
        void Clear();
        IEnumerable<TrackingManager> TrackingManagers { get; }

        void RemakeAllMulti();
        void RemakeMulti();
        void RevertAllMulti();
        void RevertMulti();
        void RollbackMulti();
    }

    public abstract class BaseTrackingManagerProvider : ITrackingManagerProvider
    {
        protected BaseTrackingManagerProvider() => Manager = new TrackingManager();

        protected internal TrackingManager Manager { get; set; } 

        public void RevertMulti()
        {
            var trackingManagers = TrackingManagers.ToList();
            foreach (var item in trackingManagers)
                item.Revert();
        }

        public void RemakeMulti()
        {
            var trackingManagers = TrackingManagers.ToList();

            foreach (var item in trackingManagers)
                item.Remake();
        }

        public void RemakeAllMulti()
        {
            var trackingManagers = TrackingManagers.ToList();

            var referenceManager = trackingManagers.First();

            while (referenceManager.CanRemake)
                RemakeMulti();
        }

        public void RevertAllMulti()
        {
            var trackingManagers = TrackingManagers.ToList();

            var referenceManager = trackingManagers.First();

            while (referenceManager.CanRevert)
                RevertMulti();
        }

        public void RollbackMulti()
        {
            var trackingManagers = TrackingManagers.ToList();
            foreach (var item in trackingManagers)
                item.Rollback();
        }

        public void Clear()
        {
            var trackingManagers = TrackingManagers.ToList();

            foreach (var item in trackingManagers)
                item.CurrentScope.Clear();

            TrackingItemCache.Instance.Clear();
            DataSetTrackingManagerProvider.Instance.DataSet?.Clear();
            TrackingActionFactory.ClearCache();
        }

        public virtual IEnumerable<TrackingManager> TrackingManagers
        {
            get
            {
                yield return Manager;
            }
        }
    }

}
