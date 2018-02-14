using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileSystemAbstraction
{
    public interface IStreamFactory
    {
        Task ReadToStreamAsync(string key, Stream toStream, CancellationToken cancellationToken = default(CancellationToken));
    }
}