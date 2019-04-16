using System.Threading;
using System.Threading.Tasks;

namespace FileSystemAbstraction
{
    public interface IFileRegister
    {
        Task<bool> RemoveAsync(string scheme, string key,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> ContainsAsync(string scheme, string key,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IFile> GetOrDefaultAsync(string scheme, string key,
            CancellationToken cancellationToken = default(CancellationToken));

        Task StoreAsync(string scheme, string key, IFile file,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}