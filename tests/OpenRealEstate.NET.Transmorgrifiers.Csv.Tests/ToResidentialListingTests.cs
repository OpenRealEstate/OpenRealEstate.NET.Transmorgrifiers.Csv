using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using OpenRealEstate.NET.Core;
using OpenRealEstate.NET.Core.Residential;
using Xunit;
using ListingAgent = OpenRealEstate.NET.Core.Agent;

namespace OpenRealEstate.NET.Transmorgrifiers.Csv.Tests
{
    public class ToResidentialListingTests
    {
        public static TheoryData<string, string, SoldListing, ResidentialListing> SoldListings
        {
            get
            {
                var listing = Builder<SoldListing>.CreateNew()
                                                  .With(x => x.PropertyType = PropertyType.BlockOfUnits.ToDescription())
                                                  .Build();

                var agents = (from agent in listing.Agents
                              select new ListingAgent
                              {
                                  Name = agent.Name,
                                  Order = 1,
                                  Communications = new List<Communication>
                                  {
                                      new Communication
                                      {
                                          CommunicationType = CommunicationType.Mobile,
                                          Details = agent.PhoneNumber
                                      }
                                  }
                              }).ToArray();

                var residentialListing = new ResidentialListing
                {
                    CreatedOn = listing.SoldOn,
                    UpdatedOn = listing.SoldOn,
                    StatusType = StatusType.Sold,
                    PropertyType = PropertyType.BlockOfUnits,
                    Address = new Address
                    {
                        CountryIsoCode = "AU",
                        Latitude = listing.Latitude,
                        Longitude = listing.Longitude,
                        Postcode = listing.Postcode,
                        State = listing.StateCode,
                        // Street is set in code.
                        // StreetNumber is set in code
                        Suburb = listing.Suburb
                    },
                    Agents = agents,
                    Features = new Features
                    {
                        Bathrooms = listing.Bathrooms,
                        Bedrooms = listing.Bedrooms,
                        CarParking = new CarParking
                        {
                            Garages = listing.CarSpaces
                        }
                    },
                    Pricing = new SalePricing
                    {
                        SoldOn = listing.SoldOn,
                        SoldPrice = listing.SoldPrice
                    },
                    Title = $"Sold: ${listing.SoldPrice}.",
                    Id = listing.Id.ToString()
                };

                var data = new TheoryData<string, string, SoldListing, ResidentialListing>
                {
                    // Street Number, Street, CSV Listing, Result ORE Listing
                    {null, null, listing, residentialListing},
                    {"", "", listing, residentialListing},
                    {"", "asdsadsad", listing, residentialListing},
                    {"1", "Smith Street", listing, residentialListing},
                    {"1/2", "Smith Street", listing, residentialListing},
                    {"1a", "Smith Street", listing, residentialListing},
                    {"1123", "Smith Street", listing, residentialListing},
                    {"unit-5", "Smith and High Street", listing, residentialListing}
                };

                return data;
            }
        }

        [Theory]
        [MemberData(nameof(SoldListings))]
        public void GivenAValidListing_CopyOverListingData_ReturnsAnOREListing(string streetNumber,
                                                                               string street,
                                                                               SoldListing soldListing,
                                                                               ResidentialListing expectedResidentialListing)
        {
            // Arrange.
            soldListing.Street = $"{streetNumber} {street}";
            expectedResidentialListing.Address.StreetNumber = string.IsNullOrWhiteSpace(streetNumber)
                                                                  ? string.Empty
                                                                  : streetNumber;
            expectedResidentialListing.Address.Street = string.IsNullOrWhiteSpace(street)
                                                            ? string.Empty
                                                            : street;

            // Act.
            var newListing = soldListing.ToOreListing();

            // Arrange.
            expectedResidentialListing.AgencyId = newListing.AgencyId; // Was created dynamically.
            newListing.ShouldLookLike(expectedResidentialListing);
        }
    }
}