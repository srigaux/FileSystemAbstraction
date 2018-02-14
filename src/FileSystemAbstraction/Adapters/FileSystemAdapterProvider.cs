using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileSystemAbstraction.Schemes;
using Microsoft.Extensions.DependencyInjection;

namespace FileSystemAbstraction.Adapters
{
    /// <inheritdoc />
    /// <summary>
    /// Implementation of <see cref="T:FileSystemAbstraction.Adapters.IFileSystemAdapterProvider" />.
    /// </summary>
    public class FileSystemAdapterProvider : IFileSystemAdapterProvider
    {
        private readonly Dictionary<string, IFileSystemAdapter> _adapterMap
            = new Dictionary<string, IFileSystemAdapter>(StringComparer.Ordinal);

        /// <summary>Constructor.</summary>
        /// <param name="schemes">The <see cref="IFileSystemSchemeProvider" />.</param>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/></param>
        public FileSystemAdapterProvider(IFileSystemSchemeProvider schemes, IServiceProvider serviceProvider)
        {
            Schemes = schemes;
            ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// The <see cref="IFileSystemSchemeProvider" />.
        /// </summary>
        public IFileSystemSchemeProvider Schemes { get; }

        public IServiceProvider ServiceProvider { get; }

        /// <summary>Returns the adapter instance that will be used.</summary>
        /// <param name="fileSystemScheme">The name of the fileSystm scheme being handled.</param>
        /// <returns>The adapter instance.</returns>
        public async Task<IFileSystemAdapter> GetAdapterAsync(string fileSystemScheme)
        {
            if (_adapterMap.TryGetValue(fileSystemScheme, out var adapter))
                return adapter;

            var scheme = await Schemes.GetSchemeAsync(fileSystemScheme);

            if (scheme == null)
                return null;

            adapter = (
                    ServiceProvider.GetService(scheme.AdapterType)
                    ?? ActivatorUtilities.CreateInstance(ServiceProvider, scheme.AdapterType, Array.Empty<object>()))
                as IFileSystemAdapter;

            if (adapter != null)
            {
                await adapter.InitializeAsync(scheme);
                _adapterMap[fileSystemScheme] = adapter;
            }

            return adapter;
        }
    }
}