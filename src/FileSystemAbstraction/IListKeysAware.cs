using System.Threading;
using System.Threading.Tasks;

namespace FileSystemAbstraction
{
    public interface IListKeysAware
    {
        Task<ListKeysResult> ListKeysAsync(string prefix = "", CancellationToken cancellationToken = default(CancellationToken));
    }
}