using System.Collections.Generic;

namespace OpenRealEstate.NET.Transmorgrifiers.Csv
{
    public class ParsedFileResult
    {
        public ICollection<Core.Listing> Listings { get; set; } = new List<Core.Listing>();
        public ICollection<Error> Errors { get; set; } = new List<Error>();
    }
}