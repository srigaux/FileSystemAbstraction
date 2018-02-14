using System;
using FileSystemAbstraction.Adapters;
using FileSystemAbstraction.Schemes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FileSystemAbstraction
{
    public static class FileSystemServiceCollectionExtensions
    {
        public static FileSystemBuilder AddFileSystem(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // Add dependentServices();

            services.TryAddSingleton<IFileSystem, FileSystem>();
            services.TryAddSingleton<IFileSystemAdapterProvider, FileSystemAdapterProvider>();
            services.TryAddSingleton<IFileSystemSchemeProvider, FileSystemSchemeProvider>();
            services.TryAddSingleton<FileSystemOptions>();


            return new FileSystemBuilder(services);
        }

        public static FileSystemBuilder AddFileSystem(this IServiceCollection services, string defaultScheme)
        {
            return services.AddFileSystem(o => o.DefaultScheme = defaultScheme);
        }

        public static FileSystemBuilder AddFileSystem(this IServiceCollection services, Action<FileSystemOptions> configureOptions)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            var fileSystemBuilder = services.AddFileSystem();
            services.Configure(configureOptions);
            return fileSystemBuilder;
        }

        public static IServiceCollection AddFileSystemScheme<TOptions, TAdapter>(this IServiceCollection services, string scheme, Action<FileSystemSchemeBuilder> configureScheme, Action<TOptions> configureOptions)
            where TOptions : FileSystemSchemeOptions, new()
            where TAdapter : FileSystemAdapter<TOptions>
        {
            services.AddFileSystem(fs => fs.AddScheme(scheme, s =>
            {
                s.AdapterType = typeof(TAdapter);

                if (configureScheme != null)
                    configureScheme(s);
            }));

            if (configureOptions != null)
                services.Configure(scheme, configureOptions);

            services.AddTransient<TAdapter>(); //TODO FS AddSingleton ??

            return services;
        }

        public static IServiceCollection AddFileSystemScheme<TOptions, TAdapter>(this IServiceCollection services, string scheme, Action<TOptions> configureOptions)
            where TOptions : FileSystemSchemeOptions, new()
            where TAdapter : FileSystemAdapter<TOptions>
            => services.AddFileSystemScheme<TOptions, TAdapter>(scheme, null, configureOptions);
    }
}