namespace Xce.TrackingItem.Interfaces
{
    public interface ICopiable<out T> where T : ICopiable<T>
    {
        T DeepCopy();
    }
}
