using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Bogus;
using Xce.TrackingItem;
using Xce.TRackingItem.TestModel;
using Xce.TRackingItem.TestModel.Base;
using Xce.TRackingItem.TestModel.Multi;
using Xce.TRackingItem.TestModel.PropertySave;

namespace UndoRedo
{
   
    
    
    public class MainViewModel
    {
        private readonly TrackingManager trackingManager;

        private bool scopeEnabled;

        private TrackingScope scope;

        public MainViewModel()
        {
            trackingManager = PropertyTrackingManagerProvider.Instance.GetTrackingManagers().First();

            var test = ModelGeneratorConfig<DriverMulti, CarMulti, AddressMulti>();

            Model = new TestModel(trackingManager)
            {
                Drivers = new List<object>(test.Generate(50))
            };

            Undo = new RelayCommand(trackingManager.Revert, () => trackingManager.CanRevert);
            Redo = new RelayCommand(trackingManager.Remake, () => trackingManager.CanRemake);

            StartScope = new RelayCommand(StartTrackingScope, () => !scopeEnabled);
            StopScope = new RelayCommand(StopTrackingScope, () => scopeEnabled);
        }


        public TestModel Model { get; set; } 

        public ICommand Undo { get; }
        public ICommand Redo { get; }
        public ICommand StartScope { get; }
        public ICommand StopScope { get; }

        private void StopTrackingScope()
        {
            scopeEnabled = false;
            scope.Dispose();
            scope = null;
        }

        private void StartTrackingScope()
        {
            scopeEnabled = true;
            scope = trackingManager.NewScope();
        }


        private Faker<T> GetFakerAdress<T>() where T : Address
        {
            return  new Faker<T>().RuleFor(x => x.BuildingNumber, f => f.Address.BuildingNumber())
                                  .RuleFor(x => x.CardinalDirection, f => f.Address.CardinalDirection())
                                  .RuleFor(x => x.City, f => f.Address.City())
                                  .RuleFor(x => x.CityPrefix, f => f.Address.CityPrefix())
                                  .RuleFor(x => x.CitySUffix, f => f.Address.CitySuffix())
                                  .RuleFor(x => x.Country, f => f.Address.Country())
                                  .RuleFor(x => x.CountryCode, f => f.Address.CountryCode())
                                  .RuleFor(x => x.County, f => f.Address.County())
                                  .RuleFor(x => x.Direction, f => f.Address.Direction())
                                  .RuleFor(x => x.Latitude, f => f.Address.Latitude())
                                  .RuleFor(x => x.Longitude, f => f.Address.Longitude())
                                  .RuleFor(x => x.OrdinalDirection, f => f.Address.OrdinalDirection())
                                  .RuleFor(x => x.SecondaryAddress, f => f.Address.SecondaryAddress())
                                  .RuleFor(x => x.State, f => f.Address.State())
                                  .RuleFor(x => x.StateAbbr, f => f.Address.StateAbbr())
                                  .RuleFor(x => x.StreetAddress, f => f.Address.StreetAddress())
                                  .RuleFor(x => x.StreetName, f => f.Address.StreetName())
                                  .RuleFor(x => x.StreetSuffix, f => f.Address.StreetSuffix())
                                  .RuleFor(x => x.ZipCode, f => f.Address.ZipCode());
        }

        private Faker<T> GetFakerCar<T>() where T : Car
        {
            return new Faker<T>().RuleFor(x => x.Fuel, f => f.Vehicle.Fuel())
                                   .RuleFor(x => x.Manufacturer, f => f.Vehicle.Manufacturer())
                                   .RuleFor(x => x.Model, f => f.Vehicle.Model())
                                   .RuleFor(x => x.Type, f => f.Vehicle.Type())
                                   .RuleFor(x => x.Vin, f => f.Vehicle.Vin());
        }

        private Faker<TDriver> ModelGeneratorConfig<TDriver, TCar, TAddress>()
            where TDriver : Driver<TCar, TAddress>
            where TCar : Car
            where TAddress : Address
        {
            var fakerCar = GetFakerCar<TCar>();
            var fakerAddr = GetFakerAdress<TAddress>();

            var driversGenerator = new Faker<TDriver>().RuleFor(x => x.FirstName, f => f.Name.FirstName())
                                                       .RuleFor(x => x.LastName, f => f.Name.LastName())
                                                       .RuleFor(x => x.PhoneNumber, f => f.Phone.PhoneNumber())
                                                       .RuleFor(x => x.Addresses, f => fakerAddr.Generate(f.Random.Int(1, 10)))
                                                       .RuleFor(x => x.Cars, f => fakerCar.Generate(f.Random.Int(10, 1000)));
            return driversGenerator;
        }

    }
}

