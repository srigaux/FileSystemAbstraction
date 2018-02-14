using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace FileSystemAbstraction.Adapters.Local
{
    public static class LocalFileSystemExtensions
    {
        public static FileSystemBuilder AddLocal(this FileSystemBuilder builder, string scheme, Action<LocalFileSystemOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<LocalFileSystemOptions>, LocalFileSystemOptions.PostConfigureOptions>());
            return builder.AddScheme<LocalFileSystemOptions, LocalFileSystemAdapter>(scheme, configureOptions);
        }

        public static FileSystemBuilder AddLocal(this FileSystemBuilder builder, Action<LocalFileSystemOptions> configureOptions)
            => builder.AddLocal(LocalFileSystemDefaults.DefaultScheme, configureOptions);

        public static FileSystemBuilder AddLocal(this FileSystemBuilder builder, string scheme, string directory, bool create = false)
            => builder.AddLocal(scheme, o =>
            {
                o.Directory = directory;
                o.Create = create;
            });

        public static FileSystemBuilder AddLocal(this FileSystemBuilder builder, string directory, bool create = false)
            => builder.AddLocal(LocalFileSystemDefaults.DefaultScheme, directory, create);
    }
}