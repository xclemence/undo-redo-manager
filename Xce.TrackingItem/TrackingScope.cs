using System;
using System.Collections.Generic;
using System.Linq;
using Xce.TrackingItem.TrackingAction;

namespace Xce.TrackingItem
{
    public class TrackingScope : IDisposable
    {
        private readonly object actionLocker = new object();
        private readonly Action<TrackingScope> onDispose;

        internal TrackingScope(TrackingScope parent, Action<TrackingScope> onDispose)
        {
            Parent = parent;
            this.onDispose = onDispose;
        }

        public IList<ITrackingAction> LastActions { get; set; } = new List<ITrackingAction>();

        public IList<ITrackingAction> RevertedActions { get; set; } = new List<ITrackingAction>();

        private TrackingScope Parent { get; }

        public void AddAction(ITrackingAction action)
        {
            lock (actionLocker)
            {
                LastActions.Insert(0, action);
                RevertedActions.Clear();
            }
        }

        public void Revert()
        {
            lock (actionLocker)
            {
                var lastItem = LastActions.FirstOrDefault();

                if (lastItem == null)
                    return;

                lastItem.Revert();

                LastActions.Remove(lastItem);
                RevertedActions.Insert(0, lastItem);
            }
        }
        public void Remake()
        {
            lock (actionLocker)
            {

                var lastItem = RevertedActions.FirstOrDefault();

                if (lastItem == null)
                    return;

                lastItem.Apply();

                RevertedActions.Remove(lastItem);
                LastActions.Insert(0, lastItem);
            }
        }

        public void Dispose()
        {
            if(LastActions.Count != 0)
                Parent?.AddAction(new TrackingMultiUpdate(LastActions.ToList()));

            onDispose(this);
        }
    }
}
