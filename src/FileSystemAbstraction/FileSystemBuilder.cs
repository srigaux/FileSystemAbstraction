using System;
using FileSystemAbstraction.Adapters;
using FileSystemAbstraction.Schemes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace FileSystemAbstraction
{
    /// <summary>
    /// Used to configure FileSystem
    /// </summary>
    public class FileSystemBuilder
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="services">The services being configured.</param>
        public FileSystemBuilder(IServiceCollection services) 
            => Services = services;

        /// <summary>
        /// The services being configured.
        /// </summary>
        public virtual IServiceCollection Services { get; }

        /// <summary>
        /// Adds a <see cref="FileSystemScheme"/> which can be used by <see cref="IFileSystem"/>.
        /// </summary>
        /// <typeparam name="TOptions">The <see cref="FileSystemSchemeOptions"/> type to configure the handler."/>.</typeparam>
        /// <typeparam name="TAdapter">The <see cref="FileSystemAdapter{TOptions}"/> used to handle this scheme.</typeparam>
        /// <param name="fileSystemScheme">The name of this scheme.</param>
        /// <param name="configureOptions">Used to configure the scheme options.</param>
        /// <returns>The builder.</returns>
        public virtual FileSystemBuilder AddScheme<TOptions, TAdapter>(string fileSystemScheme, Action<TOptions> configureOptions)
            where TOptions : FileSystemSchemeOptions, new()
            where TAdapter : FileSystemAdapter<TOptions>
        {
            Services.Configure<FileSystemOptions>(o =>
            {
                o.AddScheme(fileSystemScheme, scheme => {
                    scheme.AdapterType = typeof(TAdapter);
                });
            });

            if (configureOptions != null)
                Services.Configure(fileSystemScheme, configureOptions);

            Services.TryAddSingleton<TAdapter>(); //TODO FS Transient?
            
            return this;
        }
    }
}
