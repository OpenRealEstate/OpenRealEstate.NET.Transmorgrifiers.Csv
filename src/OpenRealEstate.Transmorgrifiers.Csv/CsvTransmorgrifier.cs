using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using OpenRealEstate.Core;
using OpenRealEstate.Transmorgrifiers.Core;

namespace OpenRealEstate.Transmorgrifiers.Csv
{
    public class CsvTransmorgrifier : ITransmorgrifier
    {
        /// <inheritdoc />
        public string Name => "CSV";

        /// <inheritdoc />
        public async Task<ParsedResult> ParseAsync(string data, 
                                                   Listing existingListing = null, 
                                                   bool areBadCharactersRemoved = false)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentException(nameof(data));
            }

            using (var stringReader = new StringReader(data))
            {
                return await ParseAsync(stringReader);
            }
        }

        private async Task<ParsedResult> ParseAsync(TextReader textReader)
        {
            if (textReader == null)
            {
                throw new ArgumentNullException(nameof(textReader));
            }

            var result = new ParsedResult();

            try
            {
                var configuration = InitializeCsvReaderConfiguration(result);

                var listingResults = new List<ListingResult>();

                // We need to read the header first to determine what type of file this is : sold or rent?.
                using (var csvReader = new CsvReader(textReader, configuration))
                {
                    csvReader.Read();
                    csvReader.ReadHeader();
                    var headers = csvReader.Context.HeaderRecord;

                    bool isASoldListing;
                    if (headers.Contains("sold_date", StringComparer.OrdinalIgnoreCase))
                    {
                        isASoldListing = true;
                    }
                    else if (headers.Contains("rent_date", StringComparer.OrdinalIgnoreCase))
                    {
                        isASoldListing = false;
                    }
                    else
                    {
                        // No valid headers found.
                        var parsedError = new ParsedError("Listing csv header is missing the field 'sold_date' or 'rent_date'. As such, we cannot determine what type of csv file this is suppose to be.",
                                                          "csv headers");
                        result.Errors.Add(parsedError);
                        return result;
                    }

                    // Now read in the rest of the listings.
                    while (await csvReader.ReadAsync())
                    {
                        var listing = ParseCsvDataRow(isASoldListing, csvReader);

                        // NOTE: the listing is null if it failed to read/parse the line.
                        if (listing != null)
                        {
                            var listingResult = new ListingResult
                            {
                                Listing = listing,
                                SourceData = csvReader.ToString()
                            };
                            result.Listings.Add(listingResult);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                var parsedError = new ParsedError(exception.Message, "reading csv data.");
                result.Errors.Add(parsedError);
            }

            return result;
        }

        private Configuration InitializeCsvReaderConfiguration(ParsedResult parsedResult)
        {
            if (parsedResult == null)
            {
                throw new ArgumentNullException(nameof(parsedResult));
            }

            void BadDataFound(ReadingContext context)
            {
                var parsedError = new ParsedError($"Bad data found on row '{context.RawRow}'", context.RawRecord);
                parsedResult.Errors.Add(parsedError);
            }

            void MissingFieldFound(string[] headerNames,
                                   int index,
                                   ReadingContext context)
            {
                var parsedError = new ParsedError($"Field with names ['{string.Join("', '", headerNames)}'] at index '{index}' was not found.", context.RawRecord);
                parsedResult.Errors.Add(parsedError);
            }

            bool ReadingExceptionOccured(CsvHelperException exception)
            {
                var parsedError = new ParsedError($"Reading exception: {exception.Message}", exception.ReadingContext.RawRecord);
                parsedResult.Errors.Add(parsedError);

                return false;
            }

            var configuration = new Configuration
            {
                HasHeaderRecord = true,
                PrepareHeaderForMatch = (header, index) => header.ToLowerInvariant(),
                BadDataFound = BadDataFound,
                MissingFieldFound = MissingFieldFound,
                ReadingExceptionOccurred = ReadingExceptionOccured
            };

            configuration.RegisterClassMap<CsvResidentialListingCsvMap>();
            configuration.RegisterClassMap<CsvRentalListingCsvMap>();

            return configuration;
        }

        private Listing ParseCsvDataRow(bool isASoldListing,
                                        CsvReader csvReader)
        {
            if (csvReader == null)
            {
                throw new ArgumentNullException(nameof(csvReader));
            }

            CsvListing listing;
            if (isASoldListing)
            {
                listing = csvReader.GetRecord<CsvResidentialListing>();
            }
            else
            {
                listing = csvReader.GetRecord<CsvRentalListing>();
            }

            return listing?.ToOreListing();
        }
    }
}
