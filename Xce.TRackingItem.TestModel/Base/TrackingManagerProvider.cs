using System.Collections.Generic;
using System.Linq;
using Xce.TrackingItem;
using Xce.TrackingItem.TrackingAction;

namespace Xce.TRackingItem.TestModel.Base
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
                item.Remake();

            TrackingItemCache.Instance.Clear();
        }

        public abstract IEnumerable<TrackingManager> GetTrackingManagers();
    }

}
