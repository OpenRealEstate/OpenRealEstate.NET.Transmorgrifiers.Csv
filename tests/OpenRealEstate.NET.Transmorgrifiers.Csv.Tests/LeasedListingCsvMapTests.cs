using System.IO;
using System.Linq;
using CsvHelper;
using Shouldly;
using Xunit;

namespace OpenRealEstate.Transmorgrifiers.Csv.Tests
{
    public class LeasedListingCsvMapTests
    {
        [Fact]
        public void GivenAFile_GetRecords_ReturnsAListOfListingsCorrectlyMapped()
        {
            // Arrange.
            var expectedListing = TestHelpers.FakeRentedListing;
            var textReader = new StreamReader("Sample Data\\2017-09-24-ACT-rent.csv");
            var csv = new CsvReader(textReader);
            csv.Configuration.RegisterClassMap<LeasedListingCsvMap>();

            // Act.
            var records = csv.GetRecords<LeasedListing>()
                             .ToArray();

            // Assert.
            records.Length.ShouldBe(188);
            var firstRecord = records.First();

            firstRecord.ShouldLookLike(expectedListing);
        }
    }
}