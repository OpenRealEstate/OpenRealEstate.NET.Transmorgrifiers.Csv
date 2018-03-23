using OpenRealEstate.Core;
using OpenRealEstate.Core.Rental;
using System;

namespace OpenRealEstate.Transmorgrifiers.Csv
{
    internal class CsvRentalListing : CsvListing
    {
        public DateTime LeasedOn { get; set; }
        public int RentPrice { get; set; }

        public override Listing ToOreListing()
        {
            var listing = new RentalListing();

            CopyOverListingData(listing);

            listing.CreatedOn = LeasedOn;
            listing.UpdatedOn = LeasedOn;
            listing.StatusType = StatusType.Leased;
            listing.SourceStatus = StatusType.Leased.ToString();

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