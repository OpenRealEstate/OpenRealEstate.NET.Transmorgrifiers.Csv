namespace OpenRealEstate.Transmorgrifiers.Csv
{
    internal sealed class CsvResidentialListingCsvMap : CsvListingCsvMap<CsvResidentialListing>
    {
        internal CsvResidentialListingCsvMap()
        {
            Map(m => m.SoldOn).Name("sold_date").Index(12);
            Map(m => m.SoldPrice).Name("sold_price").Index(13);
        }
    }
}