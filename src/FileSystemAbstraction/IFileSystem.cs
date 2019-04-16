using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileSystemAbstraction.Adapters;

namespace FileSystemAbstraction
{
    public interface IFileSystem
    {
        Task<IFile> GetAsync(string key, bool create = false, CancellationToken cancellationToken = default(CancellationToken));
        Task<IFile> GetAsync(string scheme, string key, bool create = false, CancellationToken cancellationToken = default(CancellationToken));

        Task<IFile> CreateFileAsync(string key, CancellationToken cancellationToken = default(CancellationToken));
        Task<IFile> CreateFileAsync(string scheme, string key, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<string>> GetKeysAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<List<string>> GetKeysAsync(string scheme, CancellationToken cancellationToken = default(CancellationToken));
        
        Task<byte[]> ReadAllBytesAsync(string key, CancellationToken cancellationToken = default(CancellationToken));
        Task<byte[]> ReadAllBytesAsync(string scheme, string key, CancellationToken cancellationToken = default(CancellationToken));
        Task<byte[]> ReadAllBytesAsync(IFile file, CancellationToken cancellationToken = default(CancellationToken));

        Task WriteAsync(string key, byte[] bytes, bool overwrite = false, CancellationToken cancellationToken = default(CancellationToken));
        Task WriteAsync(string scheme, string key, byte[] bytes, bool overwrite = false, CancellationToken cancellationToken = default(CancellationToken));
        Task WriteAsync(IFile file, byte[] bytes, bool overwrite = false, CancellationToken cancellationToken = default(CancellationToken));

        Task WriteAsync(string key, Stream stream, bool overwrite = false, CancellationToken cancellationToken = default(CancellationToken));
        Task WriteAsync(string scheme, string key, Stream stream, bool overwrite = false, CancellationToken cancellationToken = default(CancellationToken));
        Task WriteAsync(IFile file, Stream stream, bool overwrite = false, CancellationToken cancellationToken = default(CancellationToken));

        Task<long> GetSizeAsync(string key, CancellationToken cancellationToken = default(CancellationToken));
        Task<long> GetSizeAsync(string scheme, string key, CancellationToken cancellationToken = default(CancellationToken));
        Task<long> GetSizeAsync(IFile file, CancellationToken cancellationToken = default(CancellationToken));

        Task<DateTime> GetLastModificationDateAsync(string key, CancellationToken cancellationToken = default(CancellationToken));
        Task<DateTime> GetLastModificationDateAsync(string scheme, string key, CancellationToken cancellationToken = default(CancellationToken));
        Task<DateTime> GetLastModificationDateAsync(IFile file, CancellationToken cancellationToken = default(CancellationToken));
        
        Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> ExistsAsync(string scheme, string key, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> ExistsAsync(IFile file, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(string key, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteAsync(string scheme, string key, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteAsync(IFile file, CancellationToken cancellationToken = default(CancellationToken));
        
        Task RenameAsync(string sourceKey, string targetKey, CancellationToken cancellationToken = default(CancellationToken));
        Task RenameAsync(string scheme, string sourceKey, string targetKey, CancellationToken cancellationToken = default(CancellationToken));
        Task RenameAsync(IFile file, string targetKey, CancellationToken cancellationToken = default(CancellationToken));
        Task RenameAsync(IFile file, string targetScheme, string targetKey, CancellationToken cancellationToken = default(CancellationToken));

        Task ThrowIfNotExistsAsync(string key, CancellationToken cancellationToken = default(CancellationToken));
        Task ThrowIfNotExistsAsync(string scheme, string key, CancellationToken cancellationToken = default(CancellationToken));
        Task ThrowIfNotExistsAsync(IFile file, CancellationToken cancellationToken = default(CancellationToken));
        
        Task<IFileSystemAdapter> GetAdapterAsync(string scheme);
        Task<IFileSystemAdapter> GetAdapterAsync(IFile file);

        Task<string> GetMimeTypeAsync(string key, CancellationToken cancellationToken = default(CancellationToken));
        Task<string> GetMimeTypeAsync(string scheme, string key, CancellationToken cancellationToken = default(CancellationToken));
        Task<string> GetMimeTypeAsync(IFile file, CancellationToken cancellationToken = default(CancellationToken));

        Task<string> GetChecksumAsync(string key, CancellationToken cancellationToken = default(CancellationToken));
        Task<string> GetChecksumAsync(string scheme, string key, CancellationToken cancellationToken = default(CancellationToken));
        Task<string> GetChecksumAsync(IFile file, CancellationToken cancellationToken = default(CancellationToken));

        Task ReadToStreamAsync(string key, Stream toStream, CancellationToken cancellationToken = default(CancellationToken));
        Task ReadToStreamAsync(string scheme, string key, Stream toStream, CancellationToken cancellationToken = default(CancellationToken));
        Task ReadToStreamAsync(IFile file, Stream toStream, CancellationToken cancellationToken = default(CancellationToken));

        Task<ListKeysResult> ListKeysAsync(string prefix = "", CancellationToken cancellationToken = default(CancellationToken));
        Task<ListKeysResult> ListKeysAsync(string scheme, string prefix = "", CancellationToken cancellationToken = default(CancellationToken));
    }
}