using System;
using OpenRealEstate.Core;
using OpenRealEstate.Core.Rental;

namespace OpenRealEstate.Transmorgrifiers.Csv
{
    public class LeasedListing : Listing
    {
        public DateTime LeasedOn { get; set; }
        public int RentPrice { get; set; }

        public override Core.Listing ToOreListing()
        {
            var listing = new RentalListing();

            CopyOverListingData(listing);

            listing.CreatedOn = LeasedOn;
            listing.UpdatedOn = LeasedOn;
            listing.StatusType = StatusType.Leased;

            listing.PropertyType = PropertyTypeHelpers.ToPropertyType(PropertyType);

            listing.Pricing = new RentalPricing
            {
                RentalPrice = RentPrice,
                RentedOn = LeasedOn
            };

            var rentalPriceText = RentPrice > 0
                                      ? $"${RentPrice:N0}"
                                      : "price undisclosed";

            listing.Title = $"Rented: {rentalPriceText}.";

            return listing;
        }
    }
}