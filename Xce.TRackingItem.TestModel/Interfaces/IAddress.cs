namespace Xce.TrackingItem.TestModel.Interfaces
{
    public interface IAddress : IAbsctractModel
    {
        string BuildingNumber { get; set; }
        string CardinalDirection { get; set; }
        string City { get; set; }
        string CityPrefix { get; set; }
        string CitySUffix { get; set; }
        string Country { get; set; }
        string CountryCode { get; set; }
        string County { get; set; }
        string Direction { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }
        string OrdinalDirection { get; set; }
        string SecondaryAddress { get; set; }
        string State { get; set; }
        string StateAbbr { get; set; }
        string StreetAddress { get; set; }
        string StreetName { get; set; }
        string StreetSuffix { get; set; }
        string ZipCode { get; set; }
    }
}