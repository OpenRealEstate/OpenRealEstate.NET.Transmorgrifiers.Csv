using System;
using OpenRealEstate.NET.Core;
using OpenRealEstate.NET.Core.Residential;

namespace OpenRealEstate.NET.Transmorgrifiers.Csv
{
    public class SoldListing : Listing
    {
        public DateTime SoldOn { get; set; }
        public int SoldPrice { get; set; }

        public override Core.Listing ToOreListing()
        {
            var listing = new ResidentialListing();

            CopyOverListingData(listing);

            listing.CreatedOn = SoldOn;
            listing.UpdatedOn = SoldOn;
            listing.StatusType = StatusType.Sold;

            listing.PropertyType = PropertyTypeHelpers.ToPropertyType(PropertyType);

            listing.Pricing = new SalePricing
            {
                SoldOn = SoldOn,
                SoldPrice = SoldPrice
            };

            var rentalPriceText = SoldPrice > 0
                                      ? $"${SoldPrice:N0}"
                                      : "price undisclosed";

            listing.Title = $"Sold: {rentalPriceText}.";

            return listing;
        }
    }
}