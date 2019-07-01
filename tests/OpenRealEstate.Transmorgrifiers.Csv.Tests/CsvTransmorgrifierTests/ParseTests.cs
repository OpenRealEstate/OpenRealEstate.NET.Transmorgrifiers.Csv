using OpenRealEstate.Core;
using Shouldly;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OpenRealEstate.Transmorgrifiers.Csv.Tests.FileServiceTests
{
    public class ParseTests
    {
        public static TheoryData<string, string, string> AddressData
        {
            get
            {
                return new TheoryData<string, string, string>
                {
                    // Address, street number, street
                    { null, null, null },
                    { "smithstreet", null, "smithstreet" }, // Street Address is only one word (e.g. street number was left out on purpose)
                    { "1 smith street", "1", "smith street" },
                    { "smith street", "smith", "street" },
                    { "1a smith street", "1a", "smith street" },
                    { "1/2 smith street", "1/2", "smith street" },
                    { "1123 smith street", "1123", "smith street" },
                    { "unit-5 smith street", "unit-5", "smith street" }
                };
            }
        }

        [Theory]
        [InlineData("2017-09-24-ACT-sold.csv", 100, true)]
        [InlineData("2017-09-24-ACT-sold-uppercase-headers.csv", 100, true)]
        [InlineData("2017-09-24-ACT-rent.csv", 188, false)]
        [InlineData("2017-09-24-ACT-rent-uppercase-headers.csv", 188, false)]
        public async Task GivenAFile_ParseAsync_ReturnsACollectionOfListings(string fileName,
                                                                             int numberOfListings,
                                                                             bool isResidentialListing)
        {
            // Arrange.
            var content = File.ReadAllText($"Sample Data\\{fileName}");

            var csvTransmorgrifier = new CsvTransmorgrifier();

            // Act.
            var result = await csvTransmorgrifier.ParseAsync(content);

            // Assert.
            result.Listings.Count.ShouldBe(numberOfListings);

            Listing expectedListing;

            if (isResidentialListing)
            {
                expectedListing = TestHelpers.FakeResidentialListing;
            }
            else
            {
                expectedListing = TestHelpers.FakeRentalListing;
            }

            const string agencyId = "pewpew";
            expectedListing.AgencyId = agencyId;
            expectedListing.Title = agencyId;
            result.Listings.First().Listing.AgencyId = agencyId;
            result.Listings.First().Listing.Title = agencyId;

            result.Listings.First().Listing.ShouldLookLike(expectedListing);
        }

        [Theory]
        [MemberData(nameof(AddressData))]
        public async Task GivenASingleRow_ParseAsync_ReturnsACollectionOfListings(string address,
                                                                                  string streetNumber,
                                                                                  string street)
        {
            // Arrange.
            var content = File.ReadAllText($"Sample Data\\2017-09-24-ACT-sold-address-is-to-be-replaced.csv");
            content = content.Replace("<<REPLACE ADDRESS HERE>>", address);

            var csvTransmorgrifier = new CsvTransmorgrifier();

            // Act.
            var result = await csvTransmorgrifier.ParseAsync(content);

            // Assert.
            result.Listings.Count.ShouldBe(1);

            var expectedListing = TestHelpers.FakeResidentialListing;

            const string agencyId = "pewpew";
            expectedListing.AgencyId = agencyId;
            expectedListing.Title = agencyId;
            expectedListing.Address.StreetNumber = streetNumber;
            expectedListing.Address.Street = street;
            expectedListing.Address.DisplayAddress = expectedListing.Address.ToFormattedAddress(isPostCodeIncluded: true);
            result.Listings.First().Listing.AgencyId = agencyId;
            result.Listings.First().Listing.Title = agencyId;

            result.Listings.First().Listing.ShouldLookLike(expectedListing);
        }
    }
}
