using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FileSystemAbstraction.Adapters.AzureBlobStorage
{
    public class AzureCloudBlobFactory : IAzureCloudBlobClientFactory
    {
        public AzureCloudBlobFactory(string connectionString)
        {
            ConnectionString = connectionString;
            
        }
        
        public string ConnectionString { get; }


        #region Implementation of IAzureCloudBlobFactory
        
        public CloudBlobClient CreateCloudBlobClient()
        {
            var storageAccount = CloudStorageAccount.Parse(ConnectionString);
            return storageAccount.CreateCloudBlobClient();
        }

        #endregion
    }
}