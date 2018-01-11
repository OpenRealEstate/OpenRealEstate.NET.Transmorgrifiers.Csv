using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using OpenRealEstate.NET.Core;
using OpenRealEstate.NET.Core.Rental;
using OpenRealEstate.NET.Core.Residential;
using Shouldly;
using ListingAgent = OpenRealEstate.NET.Core.Agent;

namespace OpenRealEstate.NET.Transmorgrifiers.Csv.Tests
{
    public static class TestHelpers
    {
        public static SoldListing FakeSoldListing => new SoldListing
        {
            Id = 46639,
            StateCode = "ACT",
            Latitude = -35.3140930m,
            Longitude = 149.1455230m,
            ImageUrl = "http://farm5.static.flickr.com/4426/36455713094_3abc99d732_m.jpg",
            PropertyType = "apartment",
            Street = "138/31 Wentworth Avenue",
            Suburb = "Kingston",
            Postcode = "2604",
            Bedrooms = 1,
            Bathrooms = 1,
            CarSpaces = 1,
            SoldOn = new DateTime(2017, 09, 18),
            SoldPrice = 0,
            AgencyName = "Peter Blackshaw Manuka - MANUKA",
            Agents = new List<Agent>
            {
                new Agent
                {
                    Name = "Michael Kumm",
                    PhoneNumber = "0402 943 191"
                },
                new Agent
                {
                    Name = "Han Solo",
                    PhoneNumber = "12345 12345"
                }
            }
        };

        public static ResidentialListing FakeResidentialListing => new ResidentialListing
        {
            Id = "46639",
            PropertyType = PropertyType.Apartment,
            CreatedOn = new DateTime(2017, 09, 18),
            UpdatedOn = new DateTime(2017, 09, 18),
            StatusType = StatusType.Sold,
            Address = new Address
            {
                StreetNumber = "138/31",
                Street = "Wentworth Avenue",
                Suburb = "Kingston",
                Postcode = "2604",
                State = "ACT",
                CountryIsoCode = "AU",
                Latitude = -35.3140930m,
                Longitude = 149.1455230m
            },
            Pricing = new SalePricing
            {
                SoldOn = new DateTime(2017, 09, 18),
                SoldPrice = 0
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

        public static LeasedListing FakeRentedListing => new LeasedListing
        {
            Id = 19777,
            StateCode = "ACT",
            Latitude = -35.1890800m,
            Longitude = 149.1168700m,
            ImageUrl = "http://i2.au.reastatic.net/160x120/20dd6871c6394de24dd0dfea0c492f2d7b2991cc22eb4aa3d693971ebcb70ba1/main.jpg",
            PropertyType = "house",
            Street = "15 Foxlow Close",
            Suburb = "Palmerston",
            Postcode = "2913",
            Bedrooms = 3,
            Bathrooms = 1,
            CarSpaces = 3,
            LeasedOn = new DateTime(2017, 08, 20),
            RentPrice = 430,
            AgencyName = "Distinct Property Management - Fyshwick",
            Agents = new List<Agent>
            {
                new Agent
                {
                    Name = "Kirsty Bohane",
                    PhoneNumber = "02 6140 5900"
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
            Address = new Address
            {
                StreetNumber = "15",
                Street = "Foxlow Close",
                Suburb = "Palmerston",
                Postcode = "2913",
                State = "ACT",
                CountryIsoCode = "AU",
                Latitude = -35.1890800m,
                Longitude = 149.1168700m
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