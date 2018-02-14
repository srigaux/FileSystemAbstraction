using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileSystemAbstraction
{
    public class File : IFile
    {
        public File(string scheme, string key, IFileSystem fileSystem)
        {
            Scheme = scheme;
            Key = key;
            Name = key;
            FileSystem = fileSystem;
        }

        public string Scheme { get; }
        public string Key { get; private set; }

        public IFileSystem FileSystem { get; }

        //public dynamic MetaData { get; set; }
        

        /// <summary>
        /// Human readable filename (usually the end of the key).
        /// </summary>
        public string Name { get; }
        
        private byte[] _content;
        public async Task<byte[]> ReadAllBytesAsync(IDictionary<string, string> metadata, CancellationToken cancellationToken)
        {
            if (_content != null)
                return _content;

            await SetMetadataAsync(metadata, cancellationToken);

            _content = await FileSystem.ReadAllBytesAsync(Key, cancellationToken);

            return _content;
        }
        
        public Task ReadToStreamAsync(Stream toStream, CancellationToken cancellationToken) 
            => FileSystem.ReadToStreamAsync(Key, toStream, cancellationToken);

        public Task WriteAsync(byte[] bytes, bool overwrite, CancellationToken cancellationToken)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));

            return WriteAsyncImpl();

            async Task WriteAsyncImpl()
            {
                await FileSystem.WriteAsync(Key, bytes, overwrite, cancellationToken);

                _content = bytes;
            }
        }
        
        public Task WriteAsync(Stream stream, bool overwrite, CancellationToken cancellationToken)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            return WriteAsyncImpl();

            async Task WriteAsyncImpl()
            {
                await FileSystem.WriteAsync(Key, stream, overwrite, cancellationToken);

                _content = null;
            }
        }

        public async Task CopyToAsync(IFile otherFile, bool overwrite, CancellationToken cancellationToken)
        {
            using (var origStream = new MemoryStream())
            {
                await ReadToStreamAsync(origStream, cancellationToken: cancellationToken);
                origStream.Seek(0, SeekOrigin.Begin);

                await otherFile.WriteAsync(origStream, overwrite, cancellationToken);
            }
        }

        public async Task MoveToAsync(IFile otherFile, bool overwrite = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            await CopyToAsync(otherFile, overwrite, cancellationToken);
            await DeleteAsync(null, cancellationToken);
        }
        
        public Task CopyToAsync(Stream stream, CancellationToken cancellationToken)
            => ReadToStreamAsync(stream, cancellationToken);
        
        private long? _size;
        public async Task<long> GetSizeAsync(CancellationToken cancellationToken)
        {
            if (_size != null)
                return _size.Value;

            try
            {
                var size = await FileSystem.GetSizeAsync(Key, cancellationToken);
                _size = size;
                return size;
            }
            catch (FileNotFoundException)
            {}

            return 0;
        }

        public void SetSize(long size)
        {
            _size = size;
        }

        private DateTime? _lastModificationDate;
        public async Task<DateTime> GetLastModificationDateAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var lastModificationDate = await FileSystem.GetLastModificationDateAsync(Key, cancellationToken);
            _lastModificationDate = lastModificationDate;
            return lastModificationDate;
        }

        private string _checksum;
        public async Task<string> GetChecksumAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var checksum = await FileSystem.GetChecksumAsync(Key, cancellationToken);
            _checksum = checksum;
            return checksum;
        }

        private string _mimeType;
        public async Task<string> GetMimeTypeAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var mimeType = await FileSystem.GetMimeTypeAsync(Key, cancellationToken);
            _mimeType = mimeType;
            return mimeType;
        }


        public Task<bool> ExistsAsync(CancellationToken cancellationToken)
        {
            return FileSystem.ExistsAsync(Key, cancellationToken);
        }
        
        public async Task DeleteAsync(IDictionary<string, string> metadata, CancellationToken cancellationToken)
        {
            await SetMetadataAsync(metadata, cancellationToken);
            await FileSystem.DeleteAsync(Key, cancellationToken);
        }
        
        public async Task RenameAsync(string newKey, CancellationToken cancellationToken)
        {
            await FileSystem.RenameAsync(Key, newKey, cancellationToken);

            Key = newKey;
        }


        private IDictionary<string, string> _metadata;
        public async Task SetMetadataAsync(IDictionary<string, string> metadata, CancellationToken cancellationToken)
        {
            var metaDataSupporter = await GetMetaDataSupporterOrDefaultAsync();

            if (metaDataSupporter == null && metadata == null)
                return;

            await GetMetaDataSupporterOrThrowAsync();
            
            metadata = metadata ?? new Dictionary<string, string>();

            var metadataSupporter = (IMetadataSupporter)await FileSystem.GetAdapterAsync(Scheme);
            await metadataSupporter.SetMetadataAsync(Key, metadata, cancellationToken);

            _metadata = metadata;
        }

        public Task<IDictionary<string, string>> GetMetadataAsync(CancellationToken cancellationToken)
        {
            if (_metadata != null)
                return Task.FromResult(_metadata);
            

            return GetMetadataAsyncImpl();

            async Task<IDictionary<string, string>> GetMetadataAsyncImpl()
            {
                var metadataSupporter = await GetMetaDataSupporterOrThrowAsync();
                _metadata = await metadataSupporter.GetMetadataAsync(Key, cancellationToken);
                return _metadata;
            }
        }

        public async Task<IMetadataSupporter> GetMetaDataSupporterOrDefaultAsync()
        {
            var fileSystemAdapter = await FileSystem.GetAdapterAsync(Scheme);
            if (fileSystemAdapter is IMetadataSupporter metadataSupporter)
                return metadataSupporter;

            throw new NotSupportedException($"The '{Scheme}' filesystem does not support the metadata.");
        }

        public async Task<IMetadataSupporter> GetMetaDataSupporterOrThrowAsync()
        {
            var metaDataSupporter = await GetMetaDataSupporterOrDefaultAsync();

            if (metaDataSupporter == null)
                throw new NotSupportedException($"The '{Scheme}' filesystem does not support the metadata.");

            return metaDataSupporter;
        }
        
        public async Task<bool> SupportsMetadataAsync()
            => await GetMetaDataSupporterOrThrowAsync() != null;
    }
}