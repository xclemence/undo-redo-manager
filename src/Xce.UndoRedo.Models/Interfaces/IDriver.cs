using System.Collections.ObjectModel;

namespace Xce.UndoRedo.Models.Interfaces
{
    public interface IDriver<TCar, TAddr> : IAbsctractModel
        where TCar : ICar
        where TAddr : IAddress
    {
        ObservableCollection<TAddr> Addresses { get;}
        ObservableCollection<TCar> Cars { get; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string PhoneNumber { get; set; }

        void FinishCreation();
    }
}