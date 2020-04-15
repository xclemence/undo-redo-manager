using System.ComponentModel;
using Xce.TrackingItem.Attributes;
using Xce.UndoRedo.Models.Base;
using Xce.UndoRedo.Models.Interfaces;

namespace Xce.UndoRedo.Models.Fody
{
    [Tracking]
    public class AddressFody : AbsctractModel, IAddress
    {
        private string zipCode;
        private string city;
        private string streetAddress;
        private string cityPrefix;
        private string citySuffix;
        private string streetName;
        private string buildingNumber;
        private string streetSuffix;
        private string secondaryAdress;
        private string county;
        private string country;
        private string countryCode;
        private string state;
        private string stateAbbr;
        private double latitude;
        private double longitude;
        private string direction;
        private string cardinalDirection;
        private string ordinalDirection;

        public string ZipCode
        {
            get => zipCode;
            set => SetProperty(this, ref zipCode, value);
        }

        public string City
        {
            get => city;
            set => SetProperty(this, ref city, value);
        }

        public string StreetAddress
        {
            get => streetAddress;
            set => SetProperty(this, ref streetAddress, value);
        }

        public string CityPrefix
        {
            get => cityPrefix;
            set => SetProperty(this, ref cityPrefix, value);
        }

        public string CitySUffix
        {
            get => citySuffix;
            set => SetProperty(this, ref citySuffix, value);
        }

        public string StreetName
        {
            get => streetName;
            set => SetProperty(this, ref streetName, value);
        }

        public string BuildingNumber
        {
            get => buildingNumber;
            set => SetProperty(this, ref buildingNumber, value);
        }

        public string StreetSuffix
        {
            get => streetSuffix;
            set => SetProperty(this, ref streetSuffix, value);
        }

        public string SecondaryAddress
        {
            get => secondaryAdress;
            set => SetProperty(this, ref secondaryAdress, value);
        }

        public string County
        {
            get => county;
            set => SetProperty(this, ref county, value);
        }

        public string Country
        {
            get => country;
            set => SetProperty(this, ref country, value);
        }

        public string CountryCode
        {
            get => countryCode;
            set => SetProperty(this, ref countryCode, value);
        }

        public string State
        {
            get => state;
            set => SetProperty(this, ref state, value);
        }

        public string StateAbbr
        {
            get => stateAbbr;
            set => SetProperty(this, ref stateAbbr, value);
        }

        public double Latitude
        {
            get => latitude;
            set => SetProperty(this, ref latitude, value);
        }

        public double Longitude
        {
            get => longitude;
            set => SetProperty(this, ref longitude, value);
        }

        public string Direction
        {
            get => direction;
            set => SetProperty(this, ref direction, value);
        }

        public string CardinalDirection
        {
            get => cardinalDirection;
            set => SetProperty(this, ref cardinalDirection, value);
        }

        public string OrdinalDirection
        {
            get => ordinalDirection;
            set => SetProperty(this, ref ordinalDirection, value);
        }
    }
}
