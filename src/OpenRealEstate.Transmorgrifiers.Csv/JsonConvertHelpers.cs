using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace OpenRealEstate.Transmorgrifiers.Csv
{
    internal static class JsonConvertHelpers
    {
        private static JsonSerializerSettings JsonSerializerSettings => new JsonSerializerSettings
        {
            Converters = new JsonConverter[]
            {
                new StringEnumConverter()
            },
            Formatting = Formatting.Indented
        };

        internal static string SerializeObject(OpenRealEstate.Core.Listing listing)
        {
            return JsonConvert.SerializeObject(listing, JsonSerializerSettings);
        }

        internal static string SerializeObject(IEnumerable<OpenRealEstate.Core.Listing> listings)
        {
            return JsonConvert.SerializeObject(listings, JsonSerializerSettings);
        }
    }
}