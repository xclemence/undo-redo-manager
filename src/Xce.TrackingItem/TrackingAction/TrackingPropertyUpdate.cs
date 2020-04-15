using System;

namespace Xce.TrackingItem.TrackingAction
{
    public class TrackingPropertyUpdate<TObject, TValue> : ITrackingAction
    {
        public TrackingPropertyUpdate(TValue oldValue, TValue newValue, TObject refenceObject, Action<TObject, TValue> setterAction)
        {
            OldValue = oldValue;
            NewValue = newValue;
            ReferenceObject = refenceObject;
            SetterAction = setterAction ?? throw new ArgumentNullException(nameof(setterAction));
        }

        public TValue OldValue { get; }
        public TValue NewValue { get; }

        public TObject ReferenceObject { get; }

        public Action<TObject, TValue> SetterAction { get; }

        public void Apply() => SetterAction(ReferenceObject, NewValue);

        public void Revert() => SetterAction(ReferenceObject, OldValue);
    }
}
