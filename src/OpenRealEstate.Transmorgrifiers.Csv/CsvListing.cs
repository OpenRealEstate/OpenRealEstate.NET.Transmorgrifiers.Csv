using OpenRealEstate.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenRealEstate.Transmorgrifiers.Csv
{
    /// <summary>
    /// DTO class to help convert between csv-text <-> OpenRealEstate Listing.
    /// </summary>
    internal abstract class CsvListing
    {
        public int Id { get; set; }
        public string StateCode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string ImageUrl { get; set; }
        public string PropertyType { get; set; }
        public string Street { get; set; }
        public string Suburb { get; set; }
        public string Postcode { get; set; }
        public byte Bedrooms { get; set; }
        public byte Bathrooms { get; set; }
        public byte CarSpaces { get; set; }
        public string AgencyName { get; set; }
        public string Agent1Name { get; set; }
        public string Agent1Phone { get; set; }
        public string Agent2Name { get; set; }
        public string Agent2Phone { get; set; }

        public abstract Listing ToOreListing();

        protected void CopyOverListingData(Listing listing)
        {
            if (listing == null)
            {
                throw new ArgumentNullException(nameof(listing));
            }

            listing.Id = Id.ToString();
            listing.AgencyId = AgencyName;

            listing.Address = new Address
            {
                Suburb = Suburb,
                State = StateCode,
                Postcode = Postcode,
                Latitude = Latitude,
                Longitude = Longitude,
                CountryIsoCode = "AU"
            };

            // Sets the street address, based on the existing location data.
            CalculateStreetNumberAndStreet(listing.Address);

            var agents = new List<Tuple<string, string>>();

            if (!string.IsNullOrWhiteSpace(Agent1Name) && 
                !string.IsNullOrWhiteSpace(Agent1Phone))
            {
                agents.Add(new Tuple<String, string>(Agent1Name, Agent1Phone));
            }

            if (!string.IsNullOrWhiteSpace(Agent2Name) &&
                !string.IsNullOrWhiteSpace(Agent2Phone))
            {
                agents.Add(new Tuple<String, string>(Agent2Name, Agent2Phone));

            }
            listing.Agents = (from agent in agents
                              select new Agent
                              {
                                  Name = agent.Item1,
                                  Communications = new List<Communication>
                                  {
                                      new Communication
                                      {
                                          CommunicationType = CommunicationType.Mobile,
                                          Details = agent.Item2
                                      }
                                  }
                              }).ToArray();

            listing.Features = new Features
            {
                Bedrooms = Bedrooms,
                Bathrooms = Bathrooms,
                CarParking = new CarParking
                {
                    Garages = CarSpaces
                }
            };
        }

        private void CalculateStreetNumberAndStreet(Address address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            // Street parsing is _very_ rough!
            var streetSplit = string.IsNullOrWhiteSpace(Street)
                                  ? null
                                  : Street.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            string streetNumber = null;
            string street = null;

            if (streetSplit != null &&
                streetSplit.Any())
            {
                if (streetSplit.Length == 1)
                {
                    // Only 1 item, so lets just use that for the street NAME.
                    street = streetSplit.First();
                }
                else
                {
                    // Has more than 1 item, so lets use both.
                    streetNumber = streetSplit.First();
                    street = string.Join(" ", streetSplit.Skip(1));
                }
            }

            // Finally, lets remember this.
            address.StreetNumber = streetNumber;
            address.Street = street;

            // Don't forget to set the DisplayAddress now that we have completed setting up the address components.
            address.DisplayAddress = address.ToFormattedAddress(isPostCodeIncluded: true);

        }
    }
}