using System.Collections.ObjectModel;

namespace Xce.TrackingItem.TestModel.Base
{
    public abstract class Driver<TCar, TAddr> : AbsctractModel
     where TCar : Car
     where TAddr : Address
    {
        private string firstName;
        private string lastName;
        private string phoneNumber;

        public string FirstName
        {
            get => firstName;
            set => SetProperty(this, ref firstName, value);
        }

        public string LastName
        {
            get => lastName;
            set => SetProperty(this, ref lastName, value);
        }

        public string PhoneNumber
        {
            get => phoneNumber;
            set => SetProperty(this, ref phoneNumber, value);
        }



        public ObservableCollection<TCar> Cars { get; set; } = new ObservableCollection<TCar>();
        public ObservableCollection<TAddr> Addresses { get; set; } = new ObservableCollection<TAddr>();

        public virtual void FinishCreation() { }
    }
}
