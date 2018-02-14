using Microsoft.WindowsAzure.Storage.Blob;

namespace FileSystemAbstraction.Adapters.AzureBlobStorage
{
    public interface IAzureCloudBlobClientFactory
    {
        CloudBlobClient CreateCloudBlobClient();
    }
}