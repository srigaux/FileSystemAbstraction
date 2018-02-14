using System;
using FileSystemAbstraction.Schemes;

namespace FileSystemAbstraction.Adapters.AzureBlobStorage
{
    public class AzureBlobStorageFileSystemOptions : FileSystemSchemeOptions
    {
        public bool CreateContainer { get; set; } = false;
        public string ContainerName { get; set; }
        public bool DetectContentType { get; set; } = false;
        public IAzureCloudBlobClientFactory BobClientFactory { get; set; }

        public override void Validate()
        {
            base.Validate();

            if (string.IsNullOrWhiteSpace(ContainerName))
                throw new ArgumentException("The container name should not be empty.", nameof(ContainerName));
        }
    }
}