namespace Xce.TrackingItem.TrackingAction
{
    public interface ITrackingAction
    {
        void Apply();
        void Revert();
    }
}
