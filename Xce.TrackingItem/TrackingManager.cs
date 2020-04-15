﻿using System;
using System.Collections.Generic;
using Xce.TrackingItem.TrackingAction;

namespace Xce.TrackingItem
{
    public class TrackingManager : IDisposable
    {
        private readonly Stack<TrackingScope> trackingScopes = new Stack<TrackingScope>();
        private readonly TrackingScope baseScope;

        public TrackingManager()
        {
            baseScope = NewScope();
        }

        public bool IsAction { get; set; }

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

        public bool CanRevert=> (GetCurrentRegisterScope()?.LastActions.Count ?? 0) != 0;
        public bool CanRemake => (GetCurrentRegisterScope()?.RevertedActions.Count ?? 0) != 0;

        public TrackingScope CurrentScope => trackingScopes.Peek();

        public void Revert()
        {
            IsAction = true;
            GetCurrentRegisterScope().Revert();
            IsAction = false;
        }

        public void Remake()
        {
            IsAction = true;
            GetCurrentRegisterScope().Remake();
            IsAction = false;
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
            baseScope.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
