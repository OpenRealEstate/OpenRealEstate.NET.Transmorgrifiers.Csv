using System.IO;
using System.Linq;
using CsvHelper;
using Shouldly;
using Xunit;

namespace OpenRealEstate.Transmorgrifiers.Csv.Tests
{
    public class SoldListingCsvMapTests
    {
        [Fact]
        public void GivenAFile_GetRecords_ReturnsAListOfListingsCorrectlyMapped()
        {
            // Arrange.
            var expectedListing = TestHelpers.FakeSoldListing;
            var textReader = new StreamReader($"Sample Data\\2017-09-24-ACT-sold.csv");
            var csv = new CsvReader(textReader);
            csv.Configuration.RegisterClassMap<SoldListingCsvMap>();

            // Act.
            var records = csv.GetRecords<SoldListing>()
                             .ToArray();

            // Assert.
            records.Length.ShouldBe(100);
            var firstRecord = records.First();

            firstRecord.ShouldLookLike(expectedListing);
        }
    }
}