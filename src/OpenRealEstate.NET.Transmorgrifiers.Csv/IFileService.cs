using System.IO;
using System.Threading.Tasks;

namespace OpenRealEstate.Transmorgrifiers.Csv
{
    public interface IFileService
    {
        Task<ParsedResult> ParseFileAsync(TextReader textReader);
        Task<ParsedResult> ParseContentAsync(string csvContent);
    }
}
