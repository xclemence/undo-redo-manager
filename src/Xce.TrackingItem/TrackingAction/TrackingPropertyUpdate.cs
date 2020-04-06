using System;

namespace Xce.TrackingItem.TrackingAction
{
    public class TrackingPropertyUpdate<TObject, TValue> : ITrackingAction
    {
        public TrackingPropertyUpdate(TValue oldValue, TValue newValue, TObject parentObject, Action<TObject, TValue> setterAction)
        {
            OldValue = oldValue;
            NewValue = newValue;
            ParentObject = parentObject;
            SetterAction = setterAction ?? throw new ArgumentNullException(nameof(setterAction));
        }

        public TValue OldValue { get; }
        public TValue NewValue { get; }

        public TObject ParentObject { get; }

        public Action<TObject, TValue> SetterAction { get; }

        public void Apply() => SetterAction(ParentObject, NewValue);

        public void Revert() => SetterAction(ParentObject, OldValue);
    }
}
