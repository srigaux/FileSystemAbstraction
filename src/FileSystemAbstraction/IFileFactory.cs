using System.Threading;
using System.Threading.Tasks;

namespace FileSystemAbstraction
{
    public interface IFileFactory
    {
        Task<IFile> CreateFileAsync(string key, IFileSystem fileSystem, CancellationToken cancellationToken = default(CancellationToken));
    }
}