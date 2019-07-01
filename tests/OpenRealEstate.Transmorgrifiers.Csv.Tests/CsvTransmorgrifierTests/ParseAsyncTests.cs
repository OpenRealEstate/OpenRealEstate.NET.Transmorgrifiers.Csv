using OpenRealEstate.Core;
using OpenRealEstate.Transmorgrifiers.Core;
using Shouldly;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace OpenRealEstate.Transmorgrifiers.Csv.Tests.FileServiceTests
{
    public class ParseAsyncTests
    {
        [Theory]
        [InlineData("2017-09-24-ACT-sold.csv", 100, true)]
        [InlineData("2017-09-24-ACT-rent.csv", 188, false)]
        [InlineData("2017-09-24-ACT-sold-uppercase-headers.csv", 100, true)]
        [InlineData("2017-09-24-ACT-sold-muddled-columns.csv", 100, true)]
        public async Task GivenAFile_ParseAsync_ReturnsACollectionOfListings(string fileName,
                                                                             int numberOfListings,
                                                                             bool isResidentialListing)
        {
            // Arrange.
            var csvTransmorgrifier = new CsvTransmorgrifier();
            ParsedResult result;

            var data = File.ReadAllText($"Sample Data\\{fileName}");
            
            // Act.
            result = await csvTransmorgrifier.ParseAsync(data);
            
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
            var firstListing = result.Listings.First();
            firstListing.Listing.AgencyId = agencyId;
            firstListing.Listing.Title = agencyId;
            firstListing.Listing.ShouldLookLike(expectedListing);
        }

        [Fact]
        public async Task GivenAFileWithAMissingHeader_ParseAsync_ReturnsAnError()
        {
            // Arrange.
            var csvTransmorgrifier = new CsvTransmorgrifier();
            ParsedResult result;
            var data = File.ReadAllText($"Sample Data\\2017-09-24-ACT-rent-missing-header.csv");

            // Act.
            result = await csvTransmorgrifier.ParseAsync(data);
        
            // Assert.
            result.Listings.Count.ShouldBe(0);
            result.Errors.Count.ShouldBe(5); // 1x Header missing error, 4x failed to parse data because of missing header.
            result.Errors.First().ExceptionMessage.ShouldNotBeNullOrWhiteSpace();
            result.Errors.First().InvalidData.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task GivenAFileWithSomeBadRowDataField_ParseAsync_ReturnsAnError()
        {
            // Arrange.
            var csvTransmorgrifier = new CsvTransmorgrifier();
            ParsedResult result;
            var data = File.ReadAllText("Sample Data\\2017-09-24-ACT-rent-bad-row-data.csv");
            
            // Act.
            result = await csvTransmorgrifier.ParseAsync(data);

            // Assert.
            result.Listings.Count.ShouldBe(5); // 6 rows of legit data.
            result.Errors.Count.ShouldBe(3); // 4 rows were missing the IMAGE field data.
            result.Errors.First().ExceptionMessage.ShouldNotBeNullOrWhiteSpace();
            result.Errors.First().InvalidData.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task GivenAFileWithSomeMissingRowDataField_ParseAsync_ReturnsAnError()
        {
            // Arrange.
            var csvTransmorgrifier = new CsvTransmorgrifier();
            ParsedResult result;
            var data = File.ReadAllText("Sample Data\\2017-09-24-ACT-rent-missing-row-data.csv");
            
            // Act.
            result = await csvTransmorgrifier.ParseAsync(data);
            
            // Assert.
            result.Listings.Count.ShouldBe(6); // 6 rows of legit data.
            result.Errors.Count.ShouldBe(4); // 4 rows were missing the IMAGE field data.
            result.Errors.First().ExceptionMessage.ShouldNotBeNullOrWhiteSpace();
            result.Errors.First().InvalidData.ShouldNotBeNullOrWhiteSpace();
        }
    }
}
