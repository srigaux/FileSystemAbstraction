using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileSystemAbstraction.Adapters;
using FileSystemAbstraction.Exceptions;
using FileSystemAbstraction.Helpers;
using Microsoft.Extensions.Options;

namespace FileSystemAbstraction
{
    public class FileSystem : IFileSystem
    {
        public FileSystem(IFileSystemAdapterProvider adapterProvider, IOptionsMonitor<FileSystemOptions> options)
        {
            AdapterProvider = adapterProvider;
            Options = options.CurrentValue;
            FileRegister = new Dictionary<string, IFile>();
        }

        public IFileSystemAdapterProvider AdapterProvider { get; }

        public FileSystemOptions Options { get; }

        protected IDictionary<string, IFile> FileRegister { get; }

        protected string DefaultScheme => Options.DefaultScheme;

        
        #region Implementation of IFileSystem

        public Task<byte[]> ReadAllBytesAsync(string key, CancellationToken cancellationToken)
            =>  ReadAllBytesAsync(GetAdapterHolder(DefaultScheme), key, cancellationToken);

        public Task<byte[]> ReadAllBytesAsync(string scheme, string key, CancellationToken cancellationToken)
            =>  ReadAllBytesAsync(GetAdapterHolder(scheme), key, cancellationToken);

        public Task<byte[]> ReadAllBytesAsync(IFileSystemAdapter adapter, string key, CancellationToken cancellationToken)
            =>  ReadAllBytesAsync(GetAdapterHolder(adapter), key, cancellationToken);

        private Task<byte[]> ReadAllBytesAsync(AdapterHolder adapterHolder, string key, CancellationToken cancellationToken)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            
            return AdapterReadAllBytesAsync();

            async Task<byte[]> AdapterReadAllBytesAsync()
            {
                await ThrowIfNotExistsAsync(adapterHolder, key, cancellationToken);

                var adapter = await adapterHolder.GetAdapterAsync();
                var content = await adapter.ReadAllBytesAsync(key, cancellationToken);

                if (content == null)
                    throw new IOException($"Could not read the '{key} key content'");

                return content;
            }
        }

        public Task WriteAsync(string key, byte[] bytes, bool overwrite, CancellationToken cancellationToken)
            => WriteAsync(GetAdapterHolder(DefaultScheme), key, bytes, overwrite, cancellationToken);

        public Task WriteAsync(string scheme, string key, byte[] bytes, bool overwrite, CancellationToken cancellationToken)
            => WriteAsync(GetAdapterHolder(scheme), key, bytes, overwrite, cancellationToken);

        public Task WriteAsync(IFileSystemAdapter adapter, string key, byte[] bytes, bool overwrite, CancellationToken cancellationToken)
            => WriteAsync(GetAdapterHolder(adapter), key, bytes, overwrite, cancellationToken);

        private Task WriteAsync(AdapterHolder adapterHolder, string key, byte[] bytes, bool overwrite, CancellationToken cancellationToken)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            
            return WriteAsyncImpl();

            async Task WriteAsyncImpl()
            {
                if (!overwrite)
                {
                    var exists = await ExistsAsync(adapterHolder, key, cancellationToken);
                    if (exists)
                        throw new FileAlreadyExistsException(key); //TODO Message
                }

                var adapter = await adapterHolder.GetAdapterAsync();
                await adapter.WriteAsync(key, bytes, overwrite, cancellationToken);
            }
        }

        public Task WriteAsync(string key, Stream stream, bool overwrite, CancellationToken cancellationToken)
            => WriteAsync(GetAdapterHolder(DefaultScheme), key, stream, overwrite, cancellationToken);

        public Task WriteAsync(string scheme, string key, Stream stream, bool overwrite, CancellationToken cancellationToken)
            => WriteAsync(GetAdapterHolder(scheme), key, stream, overwrite, cancellationToken);

        public Task WriteAsync(IFileSystemAdapter adapter, string key, Stream stream, bool overwrite, CancellationToken cancellationToken)
            => WriteAsync(GetAdapterHolder(adapter), key, stream, overwrite, cancellationToken);

        private Task WriteAsync(AdapterHolder adapterHolder, string key, Stream stream, bool overwrite, CancellationToken cancellationToken)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            
            return WriteAsyncImpl();

            async Task WriteAsyncImpl()
            {
                if (!overwrite)
                {
                    var exists = await ExistsAsync(adapterHolder, key, cancellationToken);
                    if (exists)
                        throw new FileAlreadyExistsException(key); //TODO Message
                }
                
                var adapter = await adapterHolder.GetAdapterAsync();
                await adapter.WriteAsync(key, stream, overwrite, cancellationToken);
            }
        }

        public Task<long> GetSizeAsync(string key, CancellationToken cancellationToken)
            => GetSizeAsync(GetAdapterHolder(DefaultScheme), key, cancellationToken);

        public Task<long> GetSizeAsync(string scheme, string key, CancellationToken cancellationToken)
            => GetSizeAsync(GetAdapterHolder(scheme), key, cancellationToken);

        public Task<long> GetSizeAsync(IFileSystemAdapter adapter, string key, CancellationToken cancellationToken)
            => GetSizeAsync(GetAdapterHolder(adapter), key, cancellationToken);

        private Task<long> GetSizeAsync(AdapterHolder adapterHolder, string key, CancellationToken cancellationToken)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            
            return ReadAllBytesAndGetLengthAsync();

            async Task<long> ReadAllBytesAndGetLengthAsync()
            {
                var adapter = await adapterHolder.GetAdapterAsync();

                await ThrowIfNotExistsAsync(adapter, key, cancellationToken);
                
                if (adapter is ISizeCalculator sizeCalculator)
                    return await sizeCalculator.GetSizeAsync(key, cancellationToken);

                var bytes = await ReadAllBytesAsync(adapter, key, cancellationToken);
                return bytes.LongLength;
            }
        }


        public Task<DateTime> GetLastModificationDateAsync(string key, CancellationToken cancellationToken)
            => GetLastModificationDateAsync(GetAdapterHolder(DefaultScheme), key, cancellationToken);

        public Task<DateTime> GetLastModificationDateAsync(string scheme, string key, CancellationToken cancellationToken)
            => GetLastModificationDateAsync(GetAdapterHolder(scheme), key, cancellationToken);

        public Task<DateTime> GetLastModificationDateAsync(IFileSystemAdapter adapter, string key, CancellationToken cancellationToken)
            => GetLastModificationDateAsync(GetAdapterHolder(adapter), key, cancellationToken);

        private Task<DateTime> GetLastModificationDateAsync(AdapterHolder adapterHolder, string key, CancellationToken cancellationToken)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            
            return GetLastModificationDateAsyncImpl();

            async Task<DateTime> GetLastModificationDateAsyncImpl()
            {
                var adapter = await adapterHolder.GetAdapterAsync();

                await ThrowIfNotExistsAsync(adapter, key, cancellationToken);
                
                var date = await adapter.GetLastModificationDateAsync(key, cancellationToken);
                return date;
            }
        }


        public Task<string> GetChecksumAsync(string key, CancellationToken cancellationToken)
            => GetChecksumAsync(GetAdapterHolder(DefaultScheme), key, cancellationToken);

        public Task<string> GetChecksumAsync(string scheme, string key, CancellationToken cancellationToken)
            => GetChecksumAsync(GetAdapterHolder(scheme), key, cancellationToken);

        public Task<string> GetChecksumAsync(IFileSystemAdapter adapter, string key, CancellationToken cancellationToken)
            => GetChecksumAsync(GetAdapterHolder(adapter), key, cancellationToken);

        private Task<string> GetChecksumAsync(AdapterHolder adapterHolder, string key, CancellationToken cancellationToken)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            
            return GetChecksumAsyncImpl();

            async Task<string> GetChecksumAsyncImpl()
            {
                var adapter = await adapterHolder.GetAdapterAsync();

                await ThrowIfNotExistsAsync(adapter, key, cancellationToken);
                
                if (adapter is IChecksumCalculator checksumCalculator)
                    return await checksumCalculator.GetChecksumAsync(key, cancellationToken);

                var bytes = await ReadAllBytesAsync(adapter, key, cancellationToken);
                return ChecksumHelper.FromContent(bytes);
            }
        }



        public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken)
            => ExistsAsync(GetAdapterHolder(DefaultScheme), key, cancellationToken);

        public Task<bool> ExistsAsync(string scheme, string key, CancellationToken cancellationToken)
            => ExistsAsync(GetAdapterHolder(scheme), key, cancellationToken);

        public Task<bool> ExistsAsync(IFileSystemAdapter adapter, string key, CancellationToken cancellationToken)
            => ExistsAsync(GetAdapterHolder(adapter), key, cancellationToken);

        private Task<bool> ExistsAsync(AdapterHolder adapterHolder, string key, CancellationToken cancellationToken)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            
            return ExistsAsyncImpl();

            async Task<bool> ExistsAsyncImpl()
            {
                var adapter = await adapterHolder.GetAdapterAsync();
                var exists = await adapter.ExistsAsync(key, cancellationToken);
                return exists;    
            }
        }



        public Task DeleteAsync(string key, CancellationToken cancellationToken)
            => DeleteAsync(GetAdapterHolder(DefaultScheme), key, cancellationToken);

        public Task DeleteAsync(string scheme, string key, CancellationToken cancellationToken)
            => DeleteAsync(GetAdapterHolder(scheme), key, cancellationToken);

        public Task DeleteAsync(IFileSystemAdapter adapter, string key, CancellationToken cancellationToken)
            => DeleteAsync(GetAdapterHolder(adapter), key, cancellationToken);

        private Task DeleteAsync(AdapterHolder adapterHolder, string key, CancellationToken cancellationToken)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            
            return AdapterDeleteAndRemoveFromFileRegisterAsync();

            async Task AdapterDeleteAndRemoveFromFileRegisterAsync()
            {
                var adapter = await adapterHolder.GetAdapterAsync();

                await ThrowIfNotExistsAsync(adapter, key, cancellationToken);
                
                await adapter.DeleteAsync(key, cancellationToken);
                FileRegister.Remove(key);
            }
        }



        public Task<List<string>> GetKeysAsync(CancellationToken cancellationToken)
            => GetKeysAsync(GetAdapterHolder(DefaultScheme), cancellationToken);

        public Task<List<string>> GetKeysAsync(string scheme, CancellationToken cancellationToken)
            => GetKeysAsync(GetAdapterHolder(scheme), cancellationToken);

        public Task<List<string>> GetKeysAsync(IFileSystemAdapter adapter, CancellationToken cancellationToken)
            => GetKeysAsync(GetAdapterHolder(adapter), cancellationToken);
        
        private async Task<List<string>> GetKeysAsync(AdapterHolder adapterHolder, CancellationToken cancellationToken)
        {
            var adapter = await adapterHolder.GetAdapterAsync();
            return await adapter.GetKeysAsync(cancellationToken);
        }



        public Task<ListKeysResult> ListKeysAsync(string prefix, CancellationToken cancellationToken)
            => ListKeysAsync(GetAdapterHolder(DefaultScheme), prefix, cancellationToken);

        public Task<ListKeysResult> ListKeysAsync(string scheme, string prefix, CancellationToken cancellationToken)
            => ListKeysAsync(GetAdapterHolder(scheme), prefix, cancellationToken);

        public Task<ListKeysResult> ListKeysAsync(IFileSystemAdapter adapter, string prefix, CancellationToken cancellationToken)
            => ListKeysAsync(GetAdapterHolder(adapter), prefix, cancellationToken);

        private async Task<ListKeysResult> ListKeysAsync(AdapterHolder adapterHolder, string prefix, CancellationToken cancellationToken)
        {
            var adapter = await adapterHolder.GetAdapterAsync();

            if (adapter is IListKeysAware listKeysAware)
                return await listKeysAware.ListKeysAsync(prefix, cancellationToken);
            
            var result = new ListKeysResult();
            
            var checkPrefix = !string.IsNullOrWhiteSpace(prefix);

            var keys = await GetKeysAsync(adapter, cancellationToken);
            foreach (var key in keys)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!checkPrefix || key.StartsWith(prefix))
                {
                    var isDirectory = await adapter.IsDirectoryAsync(key, cancellationToken);
                        
                    if (isDirectory)
                        result.Directories.Add(key);
                    else
                        result.Keys.Add(key);
                }
            }

            return result;
        }


        public Task ReadToStreamAsync(string key, Stream toStream, CancellationToken cancellationToken)
            => ReadToStreamAsync(GetAdapterHolder(DefaultScheme), key, toStream, cancellationToken);
        public Task ReadToStreamAsync(string scheme, string key, Stream toStream, CancellationToken cancellationToken)
            => ReadToStreamAsync(GetAdapterHolder(scheme), key, toStream, cancellationToken);

        public Task ReadToStreamAsync(IFileSystemAdapter adapter, string key, Stream toStream, CancellationToken cancellationToken)
            => ReadToStreamAsync(GetAdapterHolder(adapter), key, toStream, cancellationToken);

        private Task ReadToStreamAsync(AdapterHolder adapterHolder, string key, Stream toStream, CancellationToken cancellationToken)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            
            return ReadAllBytesAndCreateStreamAsync();

            async Task ReadAllBytesAndCreateStreamAsync()
            {
                var adapter = await adapterHolder.GetAdapterAsync();

                if (adapter is IStreamFactory streamFactory)
                {
                    await streamFactory.ReadToStreamAsync(key, toStream, cancellationToken);
                }
                else
                {
                    var bytes = await ReadAllBytesAsync(adapter, key, cancellationToken);
                    using (var memoryBytes = new MemoryStream(bytes))
                        await memoryBytes.CopyToAsync(toStream, cancellationToken);
                }
            }
        }


        public Task RenameAsync(string sourceKey, string targetKey, CancellationToken cancellationToken)
        {
            var adapterHolder = GetAdapterHolder(DefaultScheme);
            return RenameAsync(adapterHolder, sourceKey, adapterHolder, targetKey, cancellationToken);
        }
        public Task RenameAsync(string sourceScheme, string sourceKey, string targetScheme, string targetKey, CancellationToken cancellationToken)
            => RenameAsync(GetAdapterHolder(sourceScheme), sourceKey, GetAdapterHolder(targetScheme), targetKey, cancellationToken);

        public Task RenameAsync(IFileSystemAdapter sourceAdapter, string sourceKey, IFileSystemAdapter targetAdapter, string targetKey, CancellationToken cancellationToken)
            => RenameAsync(GetAdapterHolder(sourceAdapter), sourceKey, GetAdapterHolder(targetAdapter), targetKey, cancellationToken);

        public Task RenameAsync(string sourceScheme, string sourceKey, IFileSystemAdapter targetAdapter, string targetKey, CancellationToken cancellationToken)
            => RenameAsync(GetAdapterHolder(sourceScheme), sourceKey, GetAdapterHolder(targetAdapter), targetKey, cancellationToken);

        public Task RenameAsync(IFileSystemAdapter sourceAdapter, string sourceKey, string targetScheme, string targetKey, CancellationToken cancellationToken)
            => RenameAsync(GetAdapterHolder(sourceAdapter), sourceKey, GetAdapterHolder(targetScheme), targetKey, cancellationToken);

        private Task RenameAsync(AdapterHolder sourceAdapterHolder, string sourceKey, AdapterHolder targetAdapterHolder, string targetKey, CancellationToken cancellationToken)
        {
            if (sourceKey == null) throw new ArgumentNullException(nameof(sourceKey));
            if (targetKey == null) throw new ArgumentNullException(nameof(targetKey));
            
            return CheckTargetExistenceAndRenameAsync();

            async Task CheckTargetExistenceAndRenameAsync()
            {
                await ThrowIfNotExistsAsync(sourceAdapterHolder, sourceKey, cancellationToken);

                var exists = await ExistsAsync(targetAdapterHolder, targetKey, cancellationToken);
                if (exists)
                    throw new UnexpectedFileException(targetKey); //TODO Message

                var adapter = await targetAdapterHolder.GetAdapterAsync();

                var renamedSuccess = await adapter.RenameAsync(sourceKey, targetKey, cancellationToken);
                if (!renamedSuccess)
                    throw new IOException($"Could not rename the '{sourceKey}' key to '{targetKey}'");

                if (FileRegister.ContainsKey(sourceKey))
                {
                    FileRegister[targetKey] = FileRegister[sourceKey];
                    FileRegister.Remove(sourceKey);
                }
            }
        }


        public Task<IFile> GetAsync(string key, bool create, CancellationToken cancellationToken)
            => GetAsync(GetAdapterHolder(DefaultScheme), key, create, cancellationToken);

        public Task<IFile> GetAsync(string scheme, string key, bool create, CancellationToken cancellationToken)
            => GetAsync(GetAdapterHolder(scheme), key, create, cancellationToken);

        public Task<IFile> GetAsync(IFileSystemAdapter adapter, string key, bool create, CancellationToken cancellationToken)
            => GetAsync(GetAdapterHolder(adapter), key, create, cancellationToken);

        private Task<IFile> GetAsync(AdapterHolder adapterHolder, string key, bool create, CancellationToken cancellationToken)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            return GetFileOrCreateOrThrowAsync();

            async Task<IFile> GetFileOrCreateOrThrowAsync()
            {
                if (!create)
                    await ThrowIfNotExistsAsync(adapterHolder, key, cancellationToken);

                return await CreateFileAsync(adapterHolder, key, cancellationToken);
            }
        }


        public Task<IFile> CreateFileAsync(string key, CancellationToken cancellationToken)
            => CreateFileAsync(GetAdapterHolder(DefaultScheme), key, cancellationToken);

        public Task<IFile> CreateFileAsync(string scheme, string key, CancellationToken cancellationToken)
            => CreateFileAsync(GetAdapterHolder(scheme), key, cancellationToken);

        public Task<IFile> CreateFileAsync(IFileSystemAdapter adapter, string key, CancellationToken cancellationToken)
            => CreateFileAsync(GetAdapterHolder(adapter), key, cancellationToken);

        private Task<IFile> CreateFileAsync(AdapterHolder adapterHolder, string key, CancellationToken cancellationToken)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            if (FileRegister.ContainsKey(key))
                return Task.FromResult(FileRegister[key]);
            
            return CreateFileAsyncImpl();

            async Task<IFile> CreateFileAsyncImpl()
            {
                var adapter = await adapterHolder.GetAdapterAsync();
                if (adapter is IFileFactory fileFactory)
                    FileRegister[key] = await fileFactory.CreateFileAsync(key, this, cancellationToken);
                else
                    FileRegister[key] = new File(adapter.Scheme.Name, key, this);

                return FileRegister[key];
            }
        }


        public Task ThrowIfNotExistsAsync(string key, CancellationToken cancellationToken)
            => ThrowIfNotExistsAsync(GetAdapterHolder(DefaultScheme), key, cancellationToken);

        public Task ThrowIfNotExistsAsync(string scheme, string key, CancellationToken cancellationToken)
            => ThrowIfNotExistsAsync(GetAdapterHolder(scheme), key, cancellationToken);

        public Task<IFileSystemAdapter> GetAdapterAsync(string scheme)
            => AdapterProvider.GetAdapterAsync(scheme);

        public Task ThrowIfNotExistsAsync(IFileSystemAdapter adapter, string key, CancellationToken cancellationToken)
            => ThrowIfNotExistsAsync(GetAdapterHolder(adapter), key, cancellationToken);

        private async Task ThrowIfNotExistsAsync(AdapterHolder adapterHolder, string key, CancellationToken cancellationToken)
        {
            var exists = await ExistsAsync(adapterHolder, key, cancellationToken);
            if (!exists)
            {
                var adapter = await adapterHolder.GetAdapterAsync();
                throw new FileNotFoundException($"FileSystem '{adapter.Scheme.Name}' could not find a file with key '{key}'", key);
            }
        }
        
        #endregion

        #region Implementation of IMimeTypeProvider

        public Task<string> GetMimeTypeAsync(string key, CancellationToken cancellationToken)
            => GetMimeTypeAsync(GetAdapterHolder(DefaultScheme), key, cancellationToken);

        public Task<string> GetMimeTypeAsync(string scheme, string key, CancellationToken cancellationToken)
            => GetMimeTypeAsync(GetAdapterHolder(scheme), key, cancellationToken);

        public Task<string> GetMimeTypeAsync(IFileSystemAdapter adapter, string key, CancellationToken cancellationToken)
            => GetMimeTypeAsync(GetAdapterHolder(adapter), key, cancellationToken);
        
        private Task<string> GetMimeTypeAsync(AdapterHolder adapterHolder, string key, CancellationToken cancellationToken)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            
            return GetMimeTypeAsyncImpl();

            async Task<string> GetMimeTypeAsyncImpl()
            {
                var adapter = await adapterHolder.GetAdapterAsync();

                await ThrowIfNotExistsAsync(adapter, key, cancellationToken);

                if (adapter is IMimeTypeProvider mimeTypeProvider)
                    return await mimeTypeProvider.GetMimeTypeAsync(key, cancellationToken);

                throw new InvalidOperationException($"FileSystem Adapter '{adapter.GetType().Name}' cannot provide MIME type");
            }
        }

        #endregion
        
        private AdapterHolder GetAdapterHolder(string scheme)
            => new AdapterHolder(scheme, this);

        private static AdapterHolder GetAdapterHolder(IFileSystemAdapter adapter)
            => new AdapterHolder(adapter);
        

        private class AdapterHolder
        {
            private readonly IFileSystemAdapter _adapter;
            private readonly string _scheme;
            private readonly IFileSystemAdapterProvider _adapterProvider;

            public AdapterHolder(string scheme, IFileSystemAdapterProvider adapterProvider)
            {
                _scheme = scheme;
                _adapterProvider = adapterProvider;
            }

            public AdapterHolder(string scheme, FileSystem fileSystem)
            {
                _scheme = scheme;
                _adapterProvider = fileSystem.AdapterProvider;
            }

            public AdapterHolder(IFileSystemAdapter adapter) => _adapter = adapter;

            public Task<IFileSystemAdapter> GetAdapterAsync()
            {
                if (_adapter != null)
                    return Task.FromResult(_adapter);

                return _adapterProvider.GetAdapterAsync(_scheme);
            }
        }
    }
}