﻿using Xce.TrackingItem;
using Xce.TrackingItem.TrackingAction;
using Xce.TRackingItem.TestModel.Base;

namespace Xce.TRackingItem.TestModel.ItemSave
{
    public class AddressItem : Address, ICopiable<AddressItem>, ISettable<AddressItem>
    {
        private readonly TrackingManager trackingManager = ItemTrackingManagerProvider.Instance.Manager;

        public AddressItem() : base()
        {
            if (!trackingManager.IsAction)
                TrackingItemCache.Instance.SetCacheObject(this);
        }

        public AddressItem DeepCopy()
        {
            trackingManager.IsAction = true;

            var copy = new AddressItem
            {
                BuildingNumber = BuildingNumber,
                CardinalDirection = CardinalDirection,
                City = City,
                CityPrefix = CityPrefix,
                CitySUffix = CitySUffix,
                Country = Country,
                CountryCode = CountryCode,
                County = County,
                Direction = Direction,
                Latitude = Latitude,
                Longitude = Longitude,
                OrdinalDirection = OrdinalDirection,
                SecondaryAddress = SecondaryAddress,
                State = State,
                StateAbbr = StateAbbr,
                StreetAddress = StreetAddress,
                StreetName = StreetName,
                StreetSuffix = StreetSuffix,
                ZipCode = ZipCode,
            };

            trackingManager.IsAction = false;

            return copy;
        }

        public void Set(AddressItem item)
        {
            trackingManager.IsAction = true;

            BuildingNumber = item.BuildingNumber;
            CardinalDirection = item.CardinalDirection;
            City = item.City;
            CityPrefix = item.CityPrefix;
            CitySUffix = item.CitySUffix;
            Country = item.Country;
            CountryCode = item.CountryCode;
            County = item.County;
            Direction = item.Direction;
            Latitude = item.Latitude;
            Longitude = item.Longitude;
            OrdinalDirection = item.OrdinalDirection;
            SecondaryAddress = item.SecondaryAddress;
            State = item.State;
            StateAbbr = item.StateAbbr;
            StreetAddress = item.StreetAddress;
            StreetName = item.StreetName;
            StreetSuffix = item.StreetSuffix;
            ZipCode = item.ZipCode;

            TrackingItemCache.Instance.SetCacheObject(this, item);

            trackingManager.IsAction = false;
        }
        
        protected override void OnAfterSetProperty<TObject, TValue>(TObject item, TValue field, TValue value, string callerName)
        {
            if (trackingManager.IsAction)
                return;

            trackingManager.AddAction(() =>
            {
                trackingManager.IsAction = true;

                var oldItem = TrackingItemCache.Instance.GetCacheObject(this);
                var newItem = TrackingItemCache.Instance.SetCacheObject(this);

                var item = this.GetTrackingItemUpdate(newItem, oldItem);
                
                trackingManager.IsAction = false;

                return item;
            });
        }
    }
}
