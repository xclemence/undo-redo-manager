using System.ComponentModel;

namespace Xce.TrackingItem.TestModel.Interfaces
{
    public interface IAbsctractModel
    {
        event PropertyChangedEventHandler PropertyChanged;

        void Initialize();
    }
}