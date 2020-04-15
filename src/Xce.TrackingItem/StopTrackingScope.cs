using System;

namespace Xce.TrackingItem
{
    public class StopTrackingScope : IDisposable
    {
        private readonly TrackingManager trackingManager;
        private readonly bool oldValue;

        public StopTrackingScope(TrackingManager trackingManager)
        {
            this.trackingManager = trackingManager ?? throw new ArgumentNullException(nameof(trackingManager));

            oldValue = this.trackingManager.IsAction;
            this.trackingManager.IsAction = true;
        }

        public void Dispose() => trackingManager.IsAction = oldValue;
    }
}
