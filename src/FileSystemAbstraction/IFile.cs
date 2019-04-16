using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileSystemAbstraction
{
    public interface IFile
    {
        string Scheme { get; }
        string Key { get; }

        /// <summary>
        /// Human readable filename (usually the end of the key).
        /// </summary>
        string Name { get; }

        Task<bool> SupportsMetadataAsync();


        Task<byte[]> ReadAllBytesAsync(IDictionary<string, string> metadata = null, CancellationToken cancellationToken = default(CancellationToken));
        Task ReadToStreamAsync(Stream toStream, CancellationToken cancellationToken = default(CancellationToken));

        Task WriteAsync(byte[] bytes, bool overwrite = false, CancellationToken cancellationToken = default(CancellationToken));
        Task WriteAsync(Stream stream, bool overwrite = false, CancellationToken cancellationToken = default(CancellationToken));

        Task CopyToAsync(IFile otherFile, bool overwrite = true, CancellationToken cancellationToken = default(CancellationToken));
        Task MoveToAsync(IFile otherFile, bool overwrite = true, CancellationToken cancellationToken = default(CancellationToken));
        
        Task CopyToAsync(Stream stream, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> ExistsAsync(CancellationToken cancellationToken = default(CancellationToken));
        
        Task DeleteAsync(IDictionary<string, string> metadata = null, CancellationToken cancellationToken = default(CancellationToken));
        Task RenameAsync(string newKey, CancellationToken cancellationToken = default(CancellationToken));
        
        Task<long> GetSizeAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<DateTime> GetLastModificationDateAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<string> GetChecksumAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<string> GetMimeTypeAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task SetMetadataAsync(IDictionary<string, string> metadata, CancellationToken cancellationToken = default(CancellationToken));
        Task<IDictionary<string, string>> GetMetadataAsync(CancellationToken cancellationToken = default(CancellationToken));

    }
}