using System;
using System.Collections.Generic;
using Xce.TrackingItem.TrackingAction;

namespace Xce.TrackingItem
{
    public class TrackingManager : IDisposable
    {
        private readonly Stack<TrackingScope> trackingScopes = new Stack<TrackingScope>();

        public TrackingManager()
        {
            BaseScope = NewScope();
        }

        public bool IsAction { get; set; }

        public bool CanRevert=> (GetCurrentRegisterScope()?.LastActions.Count ?? 0) != 0;
        public bool CanRemake => (GetCurrentRegisterScope()?.RevertedActions.Count ?? 0) != 0;

        public int ScopeNumber => trackingScopes.Count;

        public TrackingScope CurrentScope => trackingScopes.Peek();
        public TrackingScope BaseScope { get; }

        public void AddAction(ITrackingAction action)
        {
            if (IsAction) return;

            GetCurrentRegisterScope()?.AddAction(action);
        }

        public void AddActions(IList<ITrackingAction> actions)
        {
            if (IsAction) return;

            GetCurrentRegisterScope()?.AddActions(actions);
        }

        public void AddAction(Func<ITrackingAction> action)
        {
            if (IsAction) return;

            GetCurrentRegisterScope()?.AddAction(action);
        }

        public void Revert() => ExecuteAction(GetCurrentRegisterScope().Revert);

        public void Remake() => ExecuteAction(GetCurrentRegisterScope().Remake);

        public void Rollback() => ExecuteAction(GetCurrentRegisterScope().Rollback);

        private void ExecuteAction(Action actionToExecute)
        {
            IsAction = true;
            try
            {
                actionToExecute();
            }
            finally
            {
                IsAction = false;
            }
        }

        private TrackingScope GetCurrentRegisterScope()
        {
            if (trackingScopes.Count == 0)
                return null;

            return trackingScopes.Peek();
        }

        public TrackingScope NewScope() 
        {
            var current = GetCurrentRegisterScope();
            var scope = new TrackingScope(current, _ => trackingScopes.Pop());
            trackingScopes.Push(scope);

            return scope;
        }

        public void Dispose()
        {
            BaseScope.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
