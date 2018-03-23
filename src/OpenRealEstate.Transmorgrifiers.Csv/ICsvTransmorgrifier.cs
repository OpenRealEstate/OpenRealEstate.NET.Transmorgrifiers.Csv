using OpenRealEstate.Transmorgrifiers.Core;
using System.IO;
using System.Threading.Tasks;

namespace OpenRealEstate.Transmorgrifiers.Csv
{
    public interface ICsvTransmorgrifier : ITransmorgrifier
    {
        Task<ParsedResult> ParseAsync(TextReader textReader);
    }
}
