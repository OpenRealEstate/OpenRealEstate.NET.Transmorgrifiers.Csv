using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace OpenRealEstate.NET.Transmorgrifiers.Csv.Tests.FileServiceTests
{
    public class ParseFileAsyncTests
    {
        [Theory]
        [InlineData("2017-09-24-ACT-sold.csv", 100, true)]
        [InlineData("2017-09-24-ACT-rent.csv", 188, false)]
        public async Task GivenAFile_ParseFileAsync_ReturnsACollectionOfListings(string fileName,
                                                                            int numberOfListings,
                                                                            bool isResidentialListing)
        {
            // Arrange.
            var fileService = new FileService();
            ParsedResult result;
            using (var streamReader = new StreamReader($"Sample Data\\{fileName}"))
            {
                // Act.
                result = await fileService.ParseFileAsync(streamReader);
            }

            // Assert.
            result.Listings.Count.ShouldBe(numberOfListings);

            Core.Listing expectedListing;

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
            result.Listings.First().AgencyId = agencyId;
            result.Listings.First().Title = agencyId;

            result.Listings.First().ShouldLookLike(expectedListing);
        }

        [Fact]
        public async Task GivenAFileWithNoContent_ParseFileAsync_ReturnsAnError()
        {
            // Arrange.
            var fileService = new FileService();
            ParsedResult result;
            using (var streamReader = new StreamReader("Sample Data\\EmptyFile.csv"))
            {
                // Act.
                result = await fileService.ParseFileAsync(streamReader);
            }

            // Assert.
            result.Listings.Count.ShouldBe(0);
            result.Errors.Count.ShouldBe(1);
            result.Errors.First().Message.ShouldBe("No header record was found.");
            result.Errors.First().RowData.ShouldBeNull();
        }

        [Fact]
        public async Task GivenAFileWithAMissingHeader_ParseFileAsync_ReturnsAnError()
        {
            // Arrange.
            var fileService = new FileService();
            ParsedResult result;
            using (var streamReader = new StreamReader("Sample Data\\2017-09-24-ACT-rent-missing-header.csv"))
            {
                // Act.
                result = await fileService.ParseFileAsync(streamReader);
            }

            // Assert.
            result.Listings.Count.ShouldBe(0);
            result.Errors.Count.ShouldBe(5); // 1x Header missing error, 4x failed to parse data because of missing header.
            result.Errors.First().Message.ShouldNotBeNullOrWhiteSpace();
            result.Errors.First().RowData.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task GivenAFileWithSomeBadPostcodeRowData_ParseFileAsync_ReturnsAnError()
        {
            // Arrange.
            var fileService = new FileService();
            ParsedResult result;
            using (var streamReader = new StreamReader("Sample Data\\2017-09-24-ACT-rent-bad-postcode-data.csv"))
            {
                // Act.
                result = await fileService.ParseFileAsync(streamReader);
            }

            // Assert.
            result.Listings.Count.ShouldBe(185); // 185 rows of legit data.
            result.Errors.Count.ShouldBe(3); // 3 rows had bad postcode values.
            result.Errors.First().Message.ShouldNotBeNullOrWhiteSpace();
            result.Errors.First().RowData.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task GivenAFileWithSomeBadPropertyTypeRowData_ParseFileAsync_ReturnsAnError()
        {
            // Arrange.
            var fileService = new FileService();
            ParsedResult result;
            using (var streamReader = new StreamReader("Sample Data\\2017-09-24-ACT-sold-bad-property-type.csv"))
            {
                // Act.
                result = await fileService.ParseFileAsync(streamReader);
            }

            // Assert.
            result.Listings.Count.ShouldBe(97); // 97 rows of legit data.
            result.Errors.Count.ShouldBe(3); // 3 rows had bad postcode values.
            result.Errors.First().Message.ShouldNotBeNullOrWhiteSpace();
            result.Errors.First().RowData.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task GivenAFileWithSomeBadRowDataField_ParseFileAsync_ReturnsAnError()
        {
            // Arrange.
            var fileService = new FileService();
            ParsedResult result;
            using (var streamReader = new StreamReader("Sample Data\\2017-09-24-ACT-rent-bad-row-data.csv"))
            {
                // Act.
                result = await fileService.ParseFileAsync(streamReader);
            }

            // Assert.
            result.Listings.Count.ShouldBe(5); // 6 rows of legit data.
            result.Errors.Count.ShouldBe(3); // 4 rows were missing the IMAGE field data.
            result.Errors.First().Message.ShouldNotBeNullOrWhiteSpace();
            result.Errors.First().RowData.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task GivenAFileWithSomeMissingRowDataField_ParseFileAsync_ReturnsAnError()
        {
            // Arrange.
            var fileService = new FileService();
            ParsedResult result;
            using (var streamReader = new StreamReader("Sample Data\\2017-09-24-ACT-rent-missing-row-data.csv"))
            {
                // Act.
                result = await fileService.ParseFileAsync(streamReader);
            }

            // Assert.
            result.Listings.Count.ShouldBe(6); // 6 rows of legit data.
            result.Errors.Count.ShouldBe(4); // 4 rows were missing the IMAGE field data.
            result.Errors.First().Message.ShouldNotBeNullOrWhiteSpace();
            result.Errors.First().RowData.ShouldNotBeNullOrWhiteSpace();
        }
    }
}
