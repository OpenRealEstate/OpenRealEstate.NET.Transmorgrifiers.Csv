using System.Collections.Generic;

namespace OpenRealEstate.Transmorgrifiers.Csv
{
    public class ParsedResult
    {
        public ICollection<Core.Listing> Listings { get; set; } = new List<Core.Listing>();
        public ICollection<Error> Errors { get; set; } = new List<Error>();
    }
}