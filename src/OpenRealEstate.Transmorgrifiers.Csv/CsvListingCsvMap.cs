using CsvHelper.Configuration;

namespace OpenRealEstate.Transmorgrifiers.Csv
{
    internal abstract class CsvListingCsvMap<T> : ClassMap<T> where T : CsvListing
    {
        internal CsvListingCsvMap()
        {
            Map(m => m.Id).Name("id").Index(0);
            Map(m => m.StateCode).Name("state_code").Index(1);
            Map(m => m.Latitude).Name("lat").Index(2);
            Map(m => m.Longitude).Name("lng").Index(3);
            Map(m => m.ImageUrl).Name("image").Index(4);
            Map(m => m.PropertyType).Name("type").Index(5);
            Map(m => m.Street).Name("address").Index(6);
            Map(m => m.Suburb).Name("suburb_name").Index(7);
            Map(m => m.Postcode).Name("postcode").Index(8);
            Map(m => m.Bedrooms).Name("bedrooms").Index(9);
            Map(m => m.Bathrooms).Name("bathrooms").Index(10);
            Map(m => m.CarSpaces).Name("car_spaces").Index(11);
            // Index 12 and 13 are for the strongly typed SOLD or RENT fields.
            Map(m => m.AgencyName).Name("agency_name").Index(14);
            Map(m => m.Agent1Name).Name("agent_name").Index(15);
            Map(m => m.Agent1Phone).Name("agent_phone").Index(16);
            Map(m => m.Agent2Name).Name("agent_2_name").Index(17);
            Map(m => m.Agent2Phone).Name("agent_2_phone").Index(18);
        }
    }
}