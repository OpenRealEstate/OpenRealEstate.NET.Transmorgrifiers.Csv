using OpenRealEstate.Core;
using OpenRealEstate.Core.Residential;
using System;

namespace OpenRealEstate.Transmorgrifiers.Csv
{
    internal class CsvResidentialListing : CsvListing
    {
        public DateTime SoldOn { get; set; }
        public int SoldPrice { get; set; }

        public override Listing ToOreListing()
        {
            var listing = new ResidentialListing();

            CopyOverListingData(listing);

            listing.CreatedOn = SoldOn;
            listing.UpdatedOn = SoldOn;
            listing.StatusType = StatusType.Sold;
            listing.SourceStatus = StatusType.Sold.ToString();

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