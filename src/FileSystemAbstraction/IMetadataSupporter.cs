using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FileSystemAbstraction
{
    public interface IMetadataSupporter
    {
        Task SetMetadataAsync(string key, IDictionary<string, string> metadata, CancellationToken cancellationToken = default(CancellationToken));

        Task<IDictionary<string, string>> GetMetadataAsync(string key, CancellationToken cancellationToken = default(CancellationToken));
    }
}