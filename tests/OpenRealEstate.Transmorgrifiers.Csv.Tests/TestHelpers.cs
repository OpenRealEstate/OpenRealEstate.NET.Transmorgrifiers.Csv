using Newtonsoft.Json;
using OpenRealEstate.Core;
using OpenRealEstate.Core.Rental;
using OpenRealEstate.Core.Residential;
using Shouldly;
using System;
using System.Collections.Generic;
using ListingAgent = OpenRealEstate.Core.Agent;

namespace OpenRealEstate.Transmorgrifiers.Csv.Tests
{
    public static class TestHelpers
    {
        public static ResidentialListing FakeResidentialListing => new ResidentialListing
        {
            Id = "46639",
            PropertyType = PropertyType.Apartment,
            CreatedOn = new DateTime(2017, 09, 18),
            UpdatedOn = new DateTime(2017, 09, 18),
            StatusType = StatusType.Sold,
            SourceStatus = StatusType.Sold.ToString(),
            Address = new Address
            {
                StreetNumber = "138/31",
                Street = "Wentworth Avenue",
                Suburb = "Kingston",
                Postcode = "2604",
                State = "ACT",
                CountryIsoCode = "AU",
                Latitude = -35.3140930m,
                Longitude = 149.1455230m,
                DisplayAddress = "138/31 Wentworth Avenue, Kingston, ACT 2604"
            },
            Pricing = new SalePricing
            {
                SoldOn = new DateTime(2017, 09, 18),
                SoldPrice = 277000
            },
            Title = "Sold: price undisclosed.",
            Agents = new List<ListingAgent>
            {
                new ListingAgent
                {
                    Name = "Michael Kumm",
                    Communications = new List<Communication>
                    {
                        new Communication
                        {
                            CommunicationType = CommunicationType.Mobile,
                            Details = "0402 943 191"
                        }
                    }
                },
                new ListingAgent
                {
                    Name = "Han Solo",
                    Communications = new List<Communication>
                    {
                        new Communication
                        {
                            CommunicationType = CommunicationType.Mobile,
                            Details = "12345 12345"
                        }
                    }
                }
            },
            Features = new Features
            {
                Bedrooms = 1,
                Bathrooms = 1,
                CarParking = new CarParking
                {
                    Garages = 1
                }
            }
        };

        public static RentalListing FakeRentalListing => new RentalListing
        {
            Id = "19777",
            PropertyType = PropertyType.House,
            CreatedOn = new DateTime(2017, 08, 20),
            UpdatedOn = new DateTime(2017, 08, 20),
            StatusType = StatusType.Leased,
            SourceStatus = StatusType.Leased.ToString(),
            Address = new Address
            {
                StreetNumber = "15",
                Street = "Foxlow Close",
                Suburb = "Palmerston",
                Postcode = "2913",
                State = "ACT",
                CountryIsoCode = "AU",
                Latitude = -35.1890800m,
                Longitude = 149.1168700m,
                DisplayAddress = "15 Foxlow Close, Palmerston, ACT 2913"
            },
            Pricing = new RentalPricing
            {
                RentalPrice = 430,
                RentedOn = new DateTime(2017, 08, 20)
            },
            Title = "Sold: $430.",
            Agents = new List<ListingAgent>
            {
                new ListingAgent
                {
                    Name = "Kirsty Bohane",
                    Communications = new List<Communication>
                    {
                        new Communication
                        {
                            CommunicationType = CommunicationType.Mobile,
                            Details = "02 6140 5900"
                        }
                    }
                }
            },
            Features = new Features
            {
                Bedrooms = 3,
                Bathrooms = 1,
                CarParking = new CarParking
                {
                    Garages = 3
                }
            }
        };

        public static void ShouldLookLike<T>(this T actual,
                                             T expected)
        {
            var actualJson = JsonConvert.SerializeObject(actual);
            var expectedJson = JsonConvert.SerializeObject(expected);
            actualJson.ShouldBe(expectedJson);
        }
    }
}