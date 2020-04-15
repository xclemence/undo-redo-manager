namespace Xce.UndoRedo.Models.Interfaces
{
    public interface ICar: IAbsctractModel
    {
        string Fuel { get; set; }
        string Manufacturer { get; set; }
        string Model { get; set; }
        string Type { get; set; }
        string Vin { get; set; }
    }
}