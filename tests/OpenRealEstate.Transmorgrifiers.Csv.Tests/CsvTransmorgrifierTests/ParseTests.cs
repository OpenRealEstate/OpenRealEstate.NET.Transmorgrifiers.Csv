using System.IO;
using System.Linq;
using OpenRealEstate.Core;
using OpenRealEstate.Transmorgrifiers.Core;
using Shouldly;
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
        [InlineData("2017-09-24-ACT-sold-muddled-columns.csv", 100, true)]
        [InlineData("2017-09-24-ACT-rent.csv", 188, false)]
        [InlineData("2017-09-24-ACT-rent-uppercase-headers.csv", 188, false)]
        public void GivenAFile_Parse_ReturnsACollectionOfListings(string fileName,
                                                                  int numberOfListings,
                                                                  bool isResidentialListing)
        {
            // Arrange.
            var content = File.ReadAllText($"Sample Data\\{fileName}");

            var csvTransmorgrifier = new CsvTransmorgrifier();

            // Act.
            var result = csvTransmorgrifier.Parse(content);

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
        public void GivenASingleRow_Parse_ReturnsACollectionOfListings(string address,
                                                                       string streetNumber,
                                                                       string street)
        {
            // Arrange.
            var content = File.ReadAllText($"Sample Data\\2017-09-24-ACT-sold-address-is-to-be-replaced.csv");
            content = content.Replace("<<REPLACE ADDRESS HERE>>", address);

            var csvTransmorgrifier = new CsvTransmorgrifier();

            // Act.
            var result = csvTransmorgrifier.Parse(content);

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

        [Fact]
        public void GivenAFileWithAMissingHeader_Parse_ReturnsAnError()
        {
            // Arrange.
            var csvTransmorgrifier = new CsvTransmorgrifier();
            ParsedResult result;
            var data = File.ReadAllText($"Sample Data\\2017-09-24-ACT-rent-missing-header.csv");

            // Act.
            result = csvTransmorgrifier.Parse(data);

            // Assert.
            result.Listings.Count.ShouldBe(0);
            result.Errors.Count.ShouldBe(5); // 1x Header missing error, 4x failed to parse data because of missing header.
            result.Errors.First().ExceptionMessage.ShouldNotBeNullOrWhiteSpace();
            result.Errors.First().InvalidData.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void GivenAFileWithSomeBadRowDataField_Parse_ReturnsAnError()
        {
            // Arrange.
            var csvTransmorgrifier = new CsvTransmorgrifier();
            ParsedResult result;
            var data = File.ReadAllText("Sample Data\\2017-09-24-ACT-rent-bad-row-data.csv");

            // Act.
            result = csvTransmorgrifier.Parse(data);

            // Assert.
            result.Listings.Count.ShouldBe(5); // 6 rows of legit data.
            result.Errors.Count.ShouldBe(3); // 4 rows were missing the IMAGE field data.
            result.Errors.First().ExceptionMessage.ShouldNotBeNullOrWhiteSpace();
            result.Errors.First().InvalidData.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void GivenAFileWithSomeMissingRowDataField_Parse_ReturnsAnError()
        {
            // Arrange.
            var csvTransmorgrifier = new CsvTransmorgrifier();
            ParsedResult result;
            var data = File.ReadAllText("Sample Data\\2017-09-24-ACT-rent-missing-row-data.csv");

            // Act.
            result = csvTransmorgrifier.Parse(data);

            // Assert.
            result.Listings.Count.ShouldBe(6); // 6 rows of legit data.
            result.Errors.Count.ShouldBe(4); // 4 rows were missing the IMAGE field data.
            result.Errors.First().ExceptionMessage.ShouldNotBeNullOrWhiteSpace();
            result.Errors.First().InvalidData.ShouldNotBeNullOrWhiteSpace();
        }
    }
}
