namespace Xce.UndoRedo.Models.Base
{
    internal static class ModelExtensions
    {
        public static void SetDriver<TDriver, TCar, TAddr>(this TDriver target, TDriver source) 
            where TDriver : Driver<TCar, TAddr>
            where TCar : Car
            where TAddr : Address
        {
            target.FirstName = source.FirstName;
            target.LastName = source.LastName;
            target.PhoneNumber = source.PhoneNumber;
        }

        public static TDriver DeepCopyDriver<TDriver, TCar, TAddr>(this TDriver item)
            where TDriver : Driver<TCar, TAddr>, new()
            where TCar : Car
            where TAddr : Address
        {
            return new TDriver
            {
                FirstName = item.FirstName,
                LastName = item.LastName,
                PhoneNumber = item.PhoneNumber
            };
        }

        public static void SetCar<T>(this T target, T source) where T : Car
        {
            target.Fuel = source.Fuel;
            target.Manufacturer = source.Manufacturer;
            target.Model = source.Model;
            target.Type = source.Type;
            target.Vin = source.Vin;
        }

        public static T DeepCopyCar<T>(this T item) where T : Car, new()
        {
            return new T
            {
                Fuel = item.Fuel,
                Manufacturer = item.Manufacturer,
                Model = item.Model,
                Type = item.Type,
                Vin = item.Vin,
            };
        }

        public static void SetAddress<T>(this T target, T source) where T : Address
        {
            target.BuildingNumber = source.BuildingNumber;
            target.CardinalDirection = source.CardinalDirection;
            target.City = source.City;
            target.CityPrefix = source.CityPrefix;
            target.CitySUffix = source.CitySUffix;
            target.Country = source.Country;
            target.CountryCode = source.CountryCode;
            target.County = source.County;
            target.Direction = source.Direction;
            target.Latitude = source.Latitude;
            target.Longitude = source.Longitude;
            target.OrdinalDirection = source.OrdinalDirection;
            target.SecondaryAddress = source.SecondaryAddress;
            target.State = source.State;
            target.StateAbbr = source.StateAbbr;
            target.StreetAddress = source.StreetAddress;
            target.StreetName = source.StreetName;
            target.StreetSuffix = source.StreetSuffix;
            target.ZipCode = source.ZipCode;
        }

        public static T DeepCopyAddress<T>(this T item) where T : Address, new()
        {
            return new T
            {
                BuildingNumber = item.BuildingNumber,
                CardinalDirection = item.CardinalDirection,
                City = item.City,
                CityPrefix = item.CityPrefix,
                CitySUffix = item.CitySUffix,
                Country = item.Country,
                CountryCode = item.CountryCode,
                County = item.County,
                Direction = item.Direction,
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                OrdinalDirection = item.OrdinalDirection,
                SecondaryAddress = item.SecondaryAddress,
                State = item.State,
                StateAbbr = item.StateAbbr,
                StreetAddress = item.StreetAddress,
                StreetName = item.StreetName,
                StreetSuffix = item.StreetSuffix,
                ZipCode = item.ZipCode,
            };
        }

    }
}
