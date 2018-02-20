using Xunit;

namespace OpenRealEstate.Transmorgrifiers.Csv.Tests
{
    public class SoldListingTests
    {
        [Fact]
        public void GivenAFile_ToListing_ReturnsAListing()
        {
            // Arrange.
            var csvListing = TestHelpers.FakeSoldListing;
            var expectedResidentialListing = TestHelpers.FakeResidentialListing;

            // Act.
            var residentialListing = csvListing.ToOreListing();

            // Assert.
            expectedResidentialListing.AgencyId = residentialListing.AgencyId; // Was created dynamically.

            residentialListing.ShouldLookLike(expectedResidentialListing);
        }
    }
}