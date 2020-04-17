using System;
using System.Collections.Generic;
using System.Linq;
using Xce.TrackingItem.Extensions;
using Xce.TrackingItem.Logger;
using Xce.TrackingItem.TrackingAction;

namespace Xce.TrackingItem
{
    public sealed class TrackingScope : IDisposable
    {
        private readonly object actionLocker = new object();
        private readonly Action<TrackingScope> onDispose;

        public TrackingScope(TrackingScope parent, Action<TrackingScope> onDispose)
        {
            Parent = parent;
            this.onDispose = onDispose;
        }
        public IList<ITrackingAction> LastActions { get; } = new List<ITrackingAction>();

        public IList<ITrackingAction> RevertedActions { get; } = new List<ITrackingAction>();
        public IList<TrackingLog> Logs { get; } = new List<TrackingLog>();

        private TrackingScope Parent { get; }

        public void AddAction(ITrackingAction action)
        {
            lock (actionLocker)
            {
                LastActions.Insert(0, action);
                RevertedActions.Clear();
            }
        }

        public void AddAction(Func<ITrackingAction> action)
        {
            lock (actionLocker)
            {
                using (var logger = new TrackingLoggerScope(Logs, "Add"))
                {
                    var newItem = action();
                    logger.Type = newItem.GetType().Name;
                    LastActions.Insert(0, newItem);
                }

                RevertedActions.Clear();
            }
        }

        public void AddActions(IList<ITrackingAction> actions)
        {
            foreach (var item in actions)
                AddAction(item);
        }

        public void Revert()
        {
            lock (actionLocker)
            {
                var lastItem = LastActions.FirstOrDefault();

                if (lastItem == null)
                    return;

                using (var logger = new TrackingLoggerScope(Logs, "Revert", lastItem.GetType().Name))
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

                using (var logger = new TrackingLoggerScope(Logs, "Remake", lastItem.GetType().Name))
                    lastItem.Apply();

                RevertedActions.Remove(lastItem);
                LastActions.Insert(0, lastItem);
            }
        }

        public void Rollback()
        {
            lock (actionLocker)
            {
                using (var logger = new TrackingLoggerScope(Logs, "Rollback"))
                {
                    foreach (var item in LastActions)
                        item.Revert();
                }

                ClearTrackingItems();
            }
        }

        public void Clear()
        {
            ClearTrackingItems();
            Logs.Clear();
        }

        private void ClearTrackingItems()
        {
            LastActions.Clear();
            RevertedActions.Clear();
        }

        public void Dispose()
        {
            if (LastActions.Count != 0)
                Parent?.AddAction(new MultiTrackingAction(LastActions.Reduce().ToList()));

            onDispose(this);

            GC.SuppressFinalize(this);
        }
    }
}
