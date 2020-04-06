namespace Xce.TrackingItem
{
    public static class TrackingManagerProvider
    {
        private static readonly TrackingManager trackingManager = new TrackingManager();

        public static TrackingManager GetDefault() => trackingManager;
    }
}
