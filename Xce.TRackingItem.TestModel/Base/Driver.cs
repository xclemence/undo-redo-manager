using System.Collections.Generic;

namespace Xce.TRackingItem.TestModel.Base
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

        public IList<TCar> Cars { get; set; }
        public IList<TAddr> Addresses { get; set; }
    }
}
