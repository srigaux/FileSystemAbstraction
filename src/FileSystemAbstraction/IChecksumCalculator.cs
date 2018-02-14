using System.Threading;
using System.Threading.Tasks;

namespace FileSystemAbstraction
{
    public interface IChecksumCalculator
    {
        Task<string> GetChecksumAsync(string key, CancellationToken cancellationToken = default(CancellationToken));
    }
}