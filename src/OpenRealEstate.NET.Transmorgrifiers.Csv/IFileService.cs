using System.IO;
using System.Threading.Tasks;

namespace OpenRealEstate.NET.Transmorgrifiers.Csv
{
    public interface IFileService
    {
        Task<ParsedFileResult> ParseFileAsync(StreamReader streamReader);
    }
}
