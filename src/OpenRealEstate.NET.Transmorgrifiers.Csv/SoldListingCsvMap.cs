using System.Collections.ObjectModel;
using CsvHelper.Configuration;

namespace OpenRealEstate.Transmorgrifiers.Csv
{
    public sealed class SoldListingCsvMap : ClassMap<SoldListing>
    {
        public SoldListingCsvMap()
        {
            Map(m => m.Id).Name("id");
            Map(m => m.StateCode).Name("state_code");
            Map(m => m.Latitude).Name("lat");
            Map(m => m.Longitude).Name("lng");
            Map(m => m.ImageUrl).Name("image");
            Map(m => m.PropertyType).Name("type");
            Map(m => m.Street).Name("address");
            Map(m => m.Suburb).Name("suburb_name");
            Map(m => m.Postcode).Name("postcode");
            Map(m => m.Bedrooms).Name("bedrooms");
            Map(m => m.Bathrooms).Name("bathrooms");
            Map(m => m.CarSpaces).Name("car_spaces");
            Map(m => m.SoldOn).Name("sold_date");
            Map(m => m.SoldPrice).Name("sold_price");
            Map(m => m.AgencyName).Name("agency_name");
            Map(m => m.Agents)
                .ConvertUsing(row =>
                {
                    var agent1Name = row.GetField<string>("agent_name");
                    var agent1Phone = row.GetField<string>("agent_phone");
                    var agent2Name = row.GetField<string>("agent_2_name");
                    var agent2Phone = row.GetField<string>("agent_2_phone");

                    var agents = new Collection<Agent>();

                    if (!string.IsNullOrWhiteSpace(agent1Name) &&
                        !string.IsNullOrWhiteSpace(agent1Phone))
                    {
                        var agent = new Agent
                        {
                            Name = agent1Name,
                            PhoneNumber = agent1Phone
                        };

                        agents.Add(agent);
                    }

                    if (!string.IsNullOrWhiteSpace(agent2Name) &&
                        !string.IsNullOrWhiteSpace(agent2Phone))
                    {
                        var agent = new Agent
                        {
                            Name = agent2Name,
                            PhoneNumber = agent2Phone
                        };

                        agents.Add(agent);
                    }

                    return agents;
                });
        }
    }
}