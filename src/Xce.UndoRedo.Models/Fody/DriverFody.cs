using System.Collections.ObjectModel;
using Xce.TrackingItem.Attributes;
using Xce.UndoRedo.Models.Base;
using Xce.UndoRedo.Models.Interfaces;

namespace Xce.UndoRedo.Models.Fody
{
    [Tracking]
    public class DriverFody : AbsctractModel, IDriver<CarFody, AddressFody>
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

        [NoPropertyTracking, CollectionTracking]
        public ObservableCollection<AddressFody> Addresses { get; set; } = new ObservableCollection<AddressFody>();

        [NoPropertyTracking, CollectionTracking]
        public ObservableCollection<CarFody> Cars { get; set; } = new ObservableCollection<CarFody>();

        public void FinishCreation() { }
    }
}