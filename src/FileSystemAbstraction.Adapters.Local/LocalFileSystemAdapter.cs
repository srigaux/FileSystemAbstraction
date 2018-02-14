using System;
using System.Collections.Generic;
using System.IO;
using IODirectory = System.IO.Directory;
using IOFile = System.IO.File;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileSystemAbstraction.Helpers;
using Microsoft.Extensions.Options;

namespace FileSystemAbstraction.Adapters.Local
{
    public class LocalFileSystemAdapter : FileSystemAdapter<LocalFileSystemOptions>, IFileSystemAdapter, IStreamFactory, IChecksumCalculator, ISizeCalculator, IMimeTypeProvider
    {
        public LocalFileSystemAdapter(IOptionsMonitor<LocalFileSystemOptions> optionsMonitor) : base(optionsMonitor)
        {
        }

        
        public string Directory => Options.Directory;
        public bool Create => Options.Create;

        
        protected override Task InitializeAdapterAsync()
        {
            EnsureDirectoryExists(Directory, Create);
            return Task.CompletedTask;
        }

        #region Implementation of IFileSystemAdapter

        public IEnumerable<string> GetKeys()
        {
            EnsureDirectoryExists(Directory, Create);
            
            return IODirectory
                .EnumerateFileSystemEntries(Directory, "*.*", SearchOption.AllDirectories)
                .Select(ComputeKey);
        }

        public override Task<List<string>> GetKeysAsync(CancellationToken cancellationToken)
            => Task.FromResult(GetKeys().ToList());

        public byte[] ReadAllBytes(string key)
        {
            var path = ComputePath(key);
            return IOFile.ReadAllBytes(path);
        }

        public override async Task<byte[]> ReadAllBytesAsync(string key, CancellationToken cancellationToken)
        {
            var path = ComputePath(key);
            using (var fileStream = IOFile.OpenRead(path))
            using (var memoryStream = new MemoryStream())
            {
                await fileStream.CopyToAsync(memoryStream, cancellationToken);
                return memoryStream.GetBuffer();
            }
        }

        public long Write(string key, byte[] content)
        {
            var path = ComputePath(key);
            var directory = Path.GetDirectoryName(path);
            EnsureDirectoryExists(directory, true); //TODO : replace true by Create ???

            IOFile.WriteAllBytes(path, content);

            return content.LongLength;
        }

        public override async Task<long> WriteAsync(string key, byte[] bytes, bool overwrite, CancellationToken cancellationToken)
        {
            var path = ComputePath(key);
            var directory = Path.GetDirectoryName(path);

            EnsureDirectoryExists(directory, true);  //TODO : replace true by Create ???

            using (var memoryStream = new MemoryStream(bytes, writable: false))
            using (var fileStream = IOFile.OpenWrite(path))
                await memoryStream.CopyToAsync(fileStream, cancellationToken);
            
            return bytes.LongLength;
        }

        public override async Task<long> WriteAsync(string key, Stream stream, bool overwrite, CancellationToken cancellationToken)
        {
            var path = ComputePath(key);
            var directory = Path.GetDirectoryName(path);

            EnsureDirectoryExists(directory, true);  //TODO : replace true by Create ???

            long length;
            using (var fileStream = new FileStream(path, overwrite ? FileMode.OpenOrCreate : FileMode.Create))
            {
                await stream.CopyToAsync(fileStream, bufferSize: 81920, cancellationToken: cancellationToken);
                length = fileStream.Length;
            }

            return length;
        }

        public DateTime GetLastModificationDate(string key)
        {
            var path = ComputePath(key);
            return new FileInfo(path).LastWriteTime;
        }

        public override Task<DateTime> GetLastModificationDateAsync(string key, CancellationToken cancellationToken) 
            => Task.FromResult(GetLastModificationDate(key));

        public bool IsDirectory(string key)
        {
            var path = ComputePath(key);
            return IODirectory.Exists(path);
        }

        public override Task<bool> IsDirectoryAsync(string key, CancellationToken cancellationToken)
        {
            var path = ComputePath(key);
            return Task.FromResult(IODirectory.Exists(path));
        }

        public bool Exists(string key)
        {
            var path = ComputePath(key);
            return IOFile.Exists(path);
        }

        public override Task<bool> ExistsAsync(string key, CancellationToken cancellationToken)
            => Task.FromResult(Exists(key));

        public bool Delete(string key)
        {
            var path = ComputePath(key);
            if (IsDirectory(path))
            {
                IODirectory.Delete(path, true);
            }

            IOFile.Delete(path);

            return true;
        }

        public override Task<bool> DeleteAsync(string key, CancellationToken cancellationToken)
            => Task.FromResult(Delete(key));

        public bool Rename(string sourceKey, string targetKey)
        {
            var sourcePath = ComputePath(sourceKey);
            var targetPath = ComputePath(targetKey);
            var targetDirectory = Path.GetDirectoryName(targetPath);
            EnsureDirectoryExists(targetDirectory, create: true);
            
            IOFile.Move(sourcePath, targetPath);

            return true;
        }


        public override Task<bool> RenameAsync(string sourceKey, string targetKey, CancellationToken cancellationToken)
            => Task.FromResult(Rename(sourceKey, targetKey));

        #endregion
        
        #region Implementation of IStreamFactory

        protected Stream GetStream(string path)
            => IOFile.Open(path, Create ? FileMode.OpenOrCreate : FileMode.Open);

        public void ReadToStream(string key, Stream toStream)
        {
            var path = ComputePath(key);

            using (var fileStream = GetStream(path))
                fileStream.CopyTo(toStream);
        }

        public async Task ReadToStreamAsync(string key, Stream toStream, CancellationToken cancellationToken)
        {
            var path = ComputePath(key);

            using (var fileStream = GetStream(path))
                await fileStream.CopyToAsync(toStream, cancellationToken);
            
        }

        #endregion

        #region Implementation of IChecksumCalculator

        public string GetChecksum(string key)
        {
            var path = ComputePath(key);
            var stream = GetStream(path);
            return ChecksumHelper.FromStream(stream);
        }

        public Task<string> GetChecksumAsync(string key, CancellationToken cancellationToken)
            => Task.FromResult(GetChecksum(key));

        #endregion

        #region Implementation of ISizeCalculator

        public long GetSize(string key)
        {
            var path = ComputePath(key);
            return new FileInfo(path).Length;
        }

        public Task<long> GetSizeAsync(string key, CancellationToken cancellationToken)
            => Task.FromResult(GetSize(key));

        #endregion

        #region Implementation of IMimeTypeProvider

        public string GetMimeType(string key)
        {
            var path = ComputePath(key);
            return MimeTypesHelper.GetMimeType(path);
        }

        public Task<string> GetMimeTypeAsync(string key, CancellationToken cancellationToken)
            => Task.FromResult(GetMimeType(key));

        #endregion

        #region Protected methods

        protected string ComputeKey(string path)
        {
            var nPath = NormalizePath(path);

            return nPath.Substring(Directory.Length).TrimStart(Path.DirectorySeparatorChar);
        }

        protected string ComputePath(string key)
        {
            EnsureDirectoryExists(Directory, Create);
            return NormalizePath(Directory + '/' + key);
        }

        protected void EnsureDirectoryExists(string directory, bool create = false)
        {
            if (IODirectory.Exists(directory))
                return;
            
            if (!create)
                throw new InvalidOperationException($"The directory '{directory}' does not exists.");

            IODirectory.CreateDirectory(directory);
        }

        protected string NormalizePath(string path)
        {
            path = PathHelper.Normalize(path);

            if (!path.StartsWith(Directory))
                throw new InvalidOperationException($"The path '{path}' is out of the filesystem. (Filesystem directory : {Directory})");

            return path;
        }

        #endregion
    }
}