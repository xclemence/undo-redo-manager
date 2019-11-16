using System;

namespace Xce.TrackingItem
{
    public class StopTrackingScope : IDisposable
    {
        private readonly TrackingManager trackingManager;

        public StopTrackingScope(TrackingManager trackingManager)
        {
            this.trackingManager = trackingManager ?? throw new ArgumentNullException(nameof(trackingManager));

            this.trackingManager.IsAction = true;
        }

        public void Dispose() 
        {
            trackingManager.IsAction = false;
        }
    }
}
