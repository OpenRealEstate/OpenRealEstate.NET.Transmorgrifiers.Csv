namespace OpenRealEstate.Transmorgrifiers.Csv
{
    internal sealed class CsvRentalListingCsvMap : CsvListingCsvMap<CsvRentalListing>
    {
        internal CsvRentalListingCsvMap()
        {
            Map(m => m.LeasedOn).Name("rent_date").Index(12);
            Map(m => m.RentPrice).Name("rent_price").Index(13);
        }
    }
}