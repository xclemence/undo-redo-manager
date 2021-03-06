﻿using Xce.TrackingItem.Attributes;
using Xce.UndoRedo.Models.Base;
using Xce.UndoRedo.Models.Interfaces;

namespace Xce.UndoRedo.Models.Fody
{
    [Tracking]
    public class CarFody : AbsctractModel, ICar
    {
        private string vin;
        private string manufacturer;
        private string model;
        private string type;
        private string fuel;

        public string Vin
        {
            get => vin;
            set => SetProperty(this, ref vin, value);
        }

        public string Manufacturer
        {
            get => manufacturer;
            set => SetProperty(this, ref manufacturer, value);
        }

        public string Model
        {
            get => model;
            set => SetProperty(this, ref model, value);
        }

        public string Type
        {
            get => type;
            set => SetProperty(this, ref type, value);
        }
        public string Fuel
        {
            get => fuel;
            set => SetProperty(this, ref fuel, value);
        }

        public override string ToString() => $"{Manufacturer} {Model} : {Vin}";
    }
}