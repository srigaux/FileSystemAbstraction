using System;

namespace FileSystemAbstraction.Adapters.AzureBlobStorage
{
    public static class AzureBlobStorageFileSystemExtensions
    {
        public static FileSystemBuilder AddAzureBlobStorage(this FileSystemBuilder builder, string scheme, Action<AzureBlobStorageFileSystemOptions> configureOptions)
        {
            //builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<AzureBlobStorageFileSystemOptions>, AzureBlobStorageFileSystemOptions.PostConfigureOptions>());
            return builder.AddScheme<AzureBlobStorageFileSystemOptions, AzureBlobStorageFileSystemAdapter>(scheme, configureOptions);
        }

        public static FileSystemBuilder AddAzureBlobStorage(this FileSystemBuilder builder, Action<AzureBlobStorageFileSystemOptions> configureOptions)
            => builder.AddAzureBlobStorage(AzureBlobStorageFileSystemDefaults.DefaultScheme, configureOptions);

        public static FileSystemBuilder AddAzureBlobStorage(this FileSystemBuilder builder, string scheme, string containerName, string connectionString, bool detectContentType)
        {
            if (containerName == null) 
                throw new ArgumentNullException(nameof(containerName));

            if (connectionString == null) 
                throw new ArgumentNullException(nameof(connectionString));

            return builder.AddAzureBlobStorage(scheme, opt =>
            {
                opt.ContainerName = containerName;
                opt.BobClientFactory = new AzureCloudBlobFactory(connectionString);
                opt.DetectContentType = detectContentType;
            });
        }

        public static FileSystemBuilder AddAzureBlobStorage(this FileSystemBuilder builder, string scheme, string containerName, string connectionString)
            => builder.AddAzureBlobStorage(scheme, containerName, connectionString, detectContentType:true);

        public static FileSystemBuilder AddAzureBlobStorage(this FileSystemBuilder builder, string containerName, string connectionString, bool detectContentType)
            => builder.AddAzureBlobStorage(AzureBlobStorageFileSystemDefaults.DefaultScheme, containerName, connectionString, detectContentType);

        public static FileSystemBuilder AddAzureBlobStorage(this FileSystemBuilder builder, string containerName, string connectionString)
            => builder.AddAzureBlobStorage(AzureBlobStorageFileSystemDefaults.DefaultScheme, containerName, connectionString, detectContentType:true);
    }
}