using System;
using System.Collections.Generic;
using System.Linq;
using OpenRealEstate.NET.Core;
using CsvAgent = OpenRealEstate.NET.Transmorgrifiers.Csv.Agent;

namespace OpenRealEstate.NET.Transmorgrifiers.Csv
{
    public abstract class Listing
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
        public IEnumerable<CsvAgent> Agents { get; set; } = Enumerable.Empty<CsvAgent>();

        public abstract Core.Listing ToOreListing();

        protected void CopyOverListingData(Core.Listing listing)
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
                CountryIsoCode = "AU",
            };

            // Sets the street address, based on the existing location data.
            CalculateStreetNumberAndStreet(listing.Address);

            listing.Agents = (from agent in Agents
                              select new Core.Agent
                              {
                                  Name = agent.Name,
                                  Communications = new List<Communication>
                                  {
                                      new Communication
                                      {
                                          CommunicationType = CommunicationType.Mobile,
                                          Details = agent.PhoneNumber
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

            var streetNumber = string.Empty;
            var street = string.Empty;

            if (streetSplit != null &&
                streetSplit.Any())
            {
                if (streetSplit.Length == 1)
                {
                    // Only 1 item, so lets just use that for the street NAME.
                    street = string.Join(" ", streetSplit);
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
        }
    }
}