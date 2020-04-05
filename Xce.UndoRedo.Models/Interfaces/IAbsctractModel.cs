using System.ComponentModel;

namespace Xce.UndoRedo.Models.Interfaces
{
    public interface IAbsctractModel
    {
        event PropertyChangedEventHandler PropertyChanged;

        void Initialize();
    }
}