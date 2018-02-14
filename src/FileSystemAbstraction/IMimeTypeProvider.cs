using System.Threading;
using System.Threading.Tasks;

namespace FileSystemAbstraction
{
    public interface IMimeTypeProvider
    {
        Task<string> GetMimeTypeAsync(string key, CancellationToken cancellationToken = default(CancellationToken));
    }
}