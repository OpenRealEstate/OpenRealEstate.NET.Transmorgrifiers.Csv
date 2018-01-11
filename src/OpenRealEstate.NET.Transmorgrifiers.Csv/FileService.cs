using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using OpenRealEstate.NET.Validation;

namespace OpenRealEstate.NET.Transmorgrifiers.Csv
{
    public class FileService : IFileService
    {
        public async Task<ParsedFileResult> ParseFileAsync(StreamReader streamReader)
        {
            if (streamReader == null)
            {
                throw new ArgumentNullException(nameof(streamReader));
            }

            var result = new ParsedFileResult();

            try
            {
                var configuration = InitializeCsvReaderConfiguration(result);

                var listings = new List<Core.Listing>();

                // We need to read the header first to determine what type of file this is : sold or rent?.
                using (var csvReader = new CsvReader(streamReader, configuration))
                {
                    csvReader.Read();
                    csvReader.ReadHeader();
                    var headers = csvReader.Context.HeaderRecord;

                    bool isASoldListing;
                    if (headers.Contains("sold_date"))
                    {
                        isASoldListing = true;
                    }
                    else if (headers.Contains("rent_date"))
                    {
                        isASoldListing = false;
                    }
                    else
                    {
                        // No valid headers found.
                        result.Errors.Add(new Error
                        {
                            Message =
                                "Listing csv header is missing the field 'sold_date' or 'rent_date'. As such, we cannot determine what type of csv file this is suppose to be."
                        });
                        return result;
                    }

                    // Now read in the rest of the listings.
                    while (await csvReader.ReadAsync())
                    {
                        var listing = ParseCsvDataRow(isASoldListing, csvReader);

                        // NOTE: the listing is null if it failed to read/parse the line.
                        if (listing != null)
                        {
                            listings.Add(listing);
                        }
                    }

                    // Validate all the listings that were parsed.
                    ValidateListings(listings, result);
                }
            }
            catch (Exception exception)
            {
                result.Errors.Add(new Error
                {
                    Message = exception.Message
                });
            }

            return result;
        }

        private Configuration InitializeCsvReaderConfiguration(ParsedFileResult parsedFileResult)
        {
            if (parsedFileResult == null)
            {
                throw new ArgumentNullException(nameof(parsedFileResult));
            }

            void BadDataFound(IReadingContext context)
            {
                parsedFileResult.Errors.Add(new Error
                {
                    Message = $"Bad data found on row '{context.RawRow}'",
                    RowData = context.RawRecord
                });
            }

            void MissingFieldFound(string[] headerNames,
                                   int index,
                                   IReadingContext context)
            {
                parsedFileResult.Errors.Add(new Error
                {
                    Message = $"Field with names ['{string.Join("', '", headerNames)}'] at index '{index}' was not found.",
                    RowData = context.RawRecord
                });
            }

            void ReadingExceptionOccured(CsvHelperException exception)
            {
                parsedFileResult.Errors.Add(new Error
                {
                    Message = $"Reading exception: {exception.Message}",
                    RowData = exception.ReadingContext.RawRecord
                });
            }

            var configuration = new Configuration
            {
                HasHeaderRecord = true,
                BadDataFound = BadDataFound,
                MissingFieldFound = MissingFieldFound,
                ReadingExceptionOccurred = ReadingExceptionOccured
            };

            configuration.RegisterClassMap<SoldListingCsvMap>();
            configuration.RegisterClassMap<LeasedListingCsvMap>();

            return configuration;
        }

        private Core.Listing ParseCsvDataRow(bool isASoldListing,
                                             CsvReader csvReader)
        {
            if (csvReader == null)
            {
                throw new ArgumentNullException(nameof(csvReader));
            }

            Listing listing;
            if (isASoldListing)
            {
                //csvReader.Configuration.RegisterClassMap<SoldListingCsvMap>();
                listing = csvReader.GetRecord<SoldListing>();
            }
            else
            {
                listing = csvReader.GetRecord<LeasedListing>();
            }

            return listing?.ToOreListing();
        }

        private void ValidateListings(IEnumerable<Core.Listing> listings,
                                      ParsedFileResult result)
        {
            if (listings == null)
            {
                throw new ArgumentNullException(nameof(listings));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            foreach (var listing in listings)
            {
                var validationResult = ValidatorMediator.Validate(listing);
                if (validationResult.IsValid)
                {
                    result.Listings.Add(listing);
                }
                else
                {
                    // We failed validation for this listing.
                    var errorMessages = string.Join(". ",
                                                    (from error in validationResult.Errors
                                                     select error.ErrorMessage));
                    var errorMessage = $"Listing '{listing}' failed validation: {errorMessages}";
                    result.Errors.Add(new Error
                    {
                        Message = errorMessage,
                        RowData = JsonConvertHelpers.SerializeObject(listing)
                    });
                }
            }
        }
    }
}