using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace OpenRealEstate.NET.Transmorgrifiers.Csv.Tests.FileServiceTests
{
    public class ParseContentAsyncTests
    {
        [Theory]
        [InlineData("2017-09-24-ACT-sold.csv", 100, true)]
        [InlineData("2017-09-24-ACT-rent.csv", 188, false)]
        public async Task GivenAFile_ParseContentAsync_ReturnsACollectionOfListings(string fileName,
                                                                                    int numberOfListings,
                                                                                    bool isResidentialListing)
        {
            // Arrange.
            var content = File.ReadAllText($"Sample Data\\{fileName}");

            var fileService = new FileService();

            // Act.
            var result = await fileService.ParseContentAsync(content);

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
    }
}