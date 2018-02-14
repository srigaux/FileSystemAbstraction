using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileSystemAbstraction.Schemes;

namespace FileSystemAbstraction.Adapters
{
    public interface IFileSystemAdapter
    {
        FileSystemScheme Scheme { get; }

        Task InitializeAsync(FileSystemScheme scheme);

        Task<List<string>> GetKeysAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<byte[]> ReadAllBytesAsync(string key, CancellationToken cancellationToken = default(CancellationToken));

        Task<long> WriteAsync(string key, byte[] bytes, bool overwrite = false, CancellationToken cancellationToken = default(CancellationToken));
        Task<long> WriteAsync(string key, Stream stream, bool overwrite = false, CancellationToken cancellationToken = default(CancellationToken));

        Task<DateTime> GetLastModificationDateAsync(string key, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> IsDirectoryAsync(string key, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> DeleteAsync(string key, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> RenameAsync(string key, string newKey, CancellationToken cancellationToken = default(CancellationToken));
    }
}