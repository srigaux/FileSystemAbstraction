using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileSystemAbstraction.Schemes;
using Microsoft.Extensions.Options;

namespace FileSystemAbstraction.Adapters
{
    public abstract class FileSystemAdapter<TOptions> : IFileSystemAdapter where TOptions : FileSystemSchemeOptions, new()
    {
        public FileSystemScheme Scheme { get; private set; }
        public TOptions Options { get; private set; }
        protected IOptionsMonitor<TOptions> OptionsMonitor { get; }

        protected FileSystemAdapter(IOptionsMonitor<TOptions> optionsMonitor)
        {
            OptionsMonitor = optionsMonitor;
        }

        public async Task InitializeAsync(FileSystemScheme scheme)
        {
            Scheme = scheme ?? throw new ArgumentNullException(nameof(scheme));

            var options = OptionsMonitor.Get(Scheme.Name);

            if (options == null)
                options = Activator.CreateInstance<TOptions>();

            Options = options;
            Options.Validate();

            await InitializeAdapterAsync();
        }

        protected virtual Task InitializeAdapterAsync()
            => Task.CompletedTask;

        #region Abastract Implementation of IFileSystemAdapter

        public abstract Task<List<string>> GetKeysAsync(CancellationToken cancellationToken);
        public abstract Task<byte[]> ReadAllBytesAsync(string key, CancellationToken cancellationToken);
        public abstract Task<long> WriteAsync(string key, byte[] bytes, bool overwrite, CancellationToken cancellationToken);
        public abstract Task<long> WriteAsync(string key, Stream stream, bool overwrite, CancellationToken cancellationToken);
        public abstract Task<DateTime> GetLastModificationDateAsync(string key, CancellationToken cancellationToken);
        public abstract Task<bool> IsDirectoryAsync(string key, CancellationToken cancellationToken);
        public abstract Task<bool> ExistsAsync(string key, CancellationToken cancellationToken);
        public abstract Task<bool> DeleteAsync(string key, CancellationToken cancellationToken);
        public abstract Task<bool> RenameAsync(string key, string newKey, CancellationToken cancellationToken);

        #endregion
    }
}