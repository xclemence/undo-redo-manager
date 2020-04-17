using Bogus;
using Xce.UndoRedo.Models.Interfaces;

namespace Xce.UndoRedo
{
    public static class FakerProviders
    {
        public static Faker<T> GetFakerAddress<T>() where T : class, IAddress, new()
        {
            return new Faker<T>().RuleFor(x => x.BuildingNumber, f => f.Address.BuildingNumber())
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
                                 .RuleFor(x => x.ZipCode, f => f.Address.ZipCode())
                                 .FinishWith((f, x) => x.Initialize());
        }

        public static Faker<T> GetFakerCar<T>() where T : class, ICar, new()
        {
            return new Faker<T>().RuleFor(x => x.Fuel, f => f.Vehicle.Fuel())
                                 .RuleFor(x => x.Manufacturer, f => f.Vehicle.Manufacturer())
                                 .RuleFor(x => x.Model, f => f.Vehicle.Model())
                                 .RuleFor(x => x.Type, f => f.Vehicle.Type())
                                 .RuleFor(x => x.Vin, f => f.Vehicle.Vin())
                                 .FinishWith((f, x) => x.Initialize());
        }

        public static Faker<TDriver> GetFakerDriver<TDriver, TCar, TAddress>(int carMaxNumber, int addressMaxNumber)
            where TDriver : class, IDriver<TCar, TAddress>, new()
            where TCar : class, ICar, new()
            where TAddress : class, IAddress, new ()
        {
            var fakerCar = GetFakerCar<TCar>();
            var fakerAddr = GetFakerAddress<TAddress>();

            return new Faker<TDriver>().RuleFor(x => x.FirstName, f => f.Name.FirstName())
                                       .RuleFor(x => x.LastName, f => f.Name.LastName())
                                       .RuleFor(x => x.PhoneNumber, f => f.Phone.PhoneNumber())
                                       .FinishWith((f, u) =>
                                       {
                                            var addresses = fakerAddr.Generate(f.Random.Int(1, addressMaxNumber));

                                           foreach (var item in addresses)
                                               u.Addresses.Add(item);

                                           var cars = fakerCar.Generate(f.Random.Int(10, carMaxNumber));

                                           foreach (var item in cars)
                                               u.Cars.Add(item);

                                           u.Initialize();
                                       });
        }
    }
}
