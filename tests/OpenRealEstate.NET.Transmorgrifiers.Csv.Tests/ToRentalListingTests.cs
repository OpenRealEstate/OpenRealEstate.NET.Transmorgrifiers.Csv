using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using OpenRealEstate.NET.Core;
using OpenRealEstate.NET.Core.Rental;
using Xunit;
using ListingAgent = OpenRealEstate.NET.Core.Agent;

namespace OpenRealEstate.NET.Transmorgrifiers.Csv.Tests
{
    public class ToRentalListingTests
    {
        public static TheoryData<string, string, LeasedListing, RentalListing> LeasedListings
        {
            get
            {
                var listing = Builder<LeasedListing>.CreateNew()
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

                var rentalListing = new RentalListing
                {
                    CreatedOn = listing.LeasedOn,
                    UpdatedOn = listing.LeasedOn,
                    StatusType = StatusType.Leased,
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
                    Pricing = new RentalPricing
                    {
                        RentedOn = listing.LeasedOn,
                        RentalPrice = listing.RentPrice
                    },
                    Title = $"Rented: ${listing.RentPrice}.",
                    Id = listing.Id.ToString()
                };

                var data = new TheoryData<string, string, LeasedListing, RentalListing>
                {
                    // Street Number, Street, CSV Listing, Result ORE Listing
                    {null, null, listing, rentalListing},
                    {"", "", listing, rentalListing},
                    {"", "asdsadsad", listing, rentalListing},
                    {"1", "Smith Street", listing, rentalListing},
                    {"1/2", "Smith Street", listing, rentalListing},
                    {"1a", "Smith Street", listing, rentalListing},
                    {"1123", "Smith Street", listing, rentalListing},
                    {"unit-5", "Smith and High Street", listing, rentalListing}
                };

                return data;
            }
        }

        [Theory]
        [MemberData(nameof(LeasedListings))]
        public void GivenALeasedListing_CopyOverListingData_ReturnsAnOREListing(string streetNumber,
                                                                                string street,
                                                                                LeasedListing leasedListing,
                                                                                RentalListing expectedRentalListing)
        {
            // Arrange.
            leasedListing.Street = $"{streetNumber} {street}";
            expectedRentalListing.Address.StreetNumber = string.IsNullOrWhiteSpace(streetNumber)
                                                             ? string.Empty
                                                             : streetNumber;
            expectedRentalListing.Address.Street = string.IsNullOrWhiteSpace(street)
                                                       ? string.Empty
                                                       : street;

            // Act.
            var newListing = leasedListing.ToOreListing();

            // Arrange.
            expectedRentalListing.AgencyId = newListing.AgencyId; // Was created dynamically.
            newListing.ShouldLookLike(expectedRentalListing);
        }
    }
}