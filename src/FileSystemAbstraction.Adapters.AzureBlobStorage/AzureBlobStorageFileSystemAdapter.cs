using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileSystemAbstraction.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FileSystemAbstraction.Adapters.AzureBlobStorage
{
    public class AzureBlobStorageFileSystemAdapter : FileSystemAdapter<AzureBlobStorageFileSystemOptions>, IFileSystemAdapter, IMetadataSupporter, IMimeTypeProvider, IChecksumCalculator, ISizeCalculator, IStreamFactory
    {
        public AzureBlobStorageFileSystemAdapter(IOptionsMonitor<AzureBlobStorageFileSystemOptions> optionsMonitor) : base(optionsMonitor)
        {
        }

        public IAzureCloudBlobClientFactory BlobClientFactory => Options.BobClientFactory;
        public string ContainerName => Options.ContainerName;
        public bool DetectContentType => Options.DetectContentType;

        private CloudBlobClient _blobClient;
        public CloudBlobClient BlobClient => _blobClient ?? (_blobClient = BlobClientFactory.CreateCloudBlobClient());

        private CloudBlobContainer _container;
        public CloudBlobContainer Container => _container ?? (_container = BlobClient.GetContainerReference(ContainerName));
        
        public Task CreateContainerAsync()
        {
            return Container.CreateIfNotExistsAsync();
        }

        public Task DeleteContainerAsync()
        {
            return Container.DeleteIfExistsAsync();
        }
        
        protected override async Task InitializeAdapterAsync()
        {
            if (Options.CreateContainer)
                await CreateContainerAsync();
        }

        #region Implementation of IFileSystemAdapter

        public override async Task<List<string>> GetKeysAsync(CancellationToken cancellationToken)
        {
            var blobs = await ListBlobsAsync(null, true, BlobListingDetails.None, cancellationToken);
            return blobs.OfType<CloudBlockBlob>().Select(b => b.Name).ToList();
        }

        public async Task<List<IListBlobItem>> ListBlobsAsync(string prefix, bool useFlatBlobListing, BlobListingDetails blobListingDetails, CancellationToken cancellationToken)
        {
            BlobContinuationToken continuationToken = null;
            var results = new List<IListBlobItem>();
            do
            {
                var response = await Container.ListBlobsSegmentedAsync(prefix, useFlatBlobListing, blobListingDetails,
                    maxResults: null,
                    currentToken: continuationToken,
                    options: null,
                    operationContext: null,
                    cancellationToken: cancellationToken);

                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
            } while (continuationToken != null);

            return results;
        }

        public override async Task<byte[]> ReadAllBytesAsync(string key, CancellationToken cancellationToken)
        {
            var cloudBlob = GetCloudBlob(key);
            
            var length = await GetSizeAsync(cloudBlob, cancellationToken);

            var bytes = new byte[length];

            await cloudBlob.DownloadToByteArrayAsync(bytes, 
                index: 0, 
                accessCondition: null, 
                options: null, 
                operationContext: null, 
                progressHandler: null, 
                cancellationToken: cancellationToken);

            return bytes;
        }

        public Task ReadToStreamAsync(string key, Stream toStream, CancellationToken cancellationToken)
        {
            var cloudBlob = GetCloudBlob(key);

            return cloudBlob.DownloadToStreamAsync(toStream,
                accessCondition: null,
                options: null,
                operationContext: null,
                progressHandler: null,
                cancellationToken: cancellationToken);
        }

        public override async Task<long> WriteAsync(string key, byte[] bytes, bool overwrite, CancellationToken cancellationToken)
        {
            var cloudBlockBlob = GetCloudBlockBlob(key);

            await cloudBlockBlob.UploadFromByteArrayAsync(bytes, 0, bytes.Length,
                accessCondition: null,
                options: null,
                operationContext: null,
                cancellationToken: cancellationToken);

            await UpdateContentTypePropertiesAsync(key, cloudBlockBlob, cancellationToken);
            
            return bytes.LongLength;
        }

        public override async Task<long> WriteAsync(string key, Stream stream, bool overwrite, CancellationToken cancellationToken)
        {
            var cloudBlockBlob = GetCloudBlockBlob(key);

            await cloudBlockBlob.UploadFromStreamAsync(stream,  
                accessCondition: null,
                options: null,
                operationContext: null,
                cancellationToken: cancellationToken);

            await UpdateContentTypePropertiesAsync(key, cloudBlockBlob, cancellationToken);

            return stream.Length;
        }

        public Task<long> GetSizeAsync(string key, CancellationToken cancellationToken)
        {
            var cloudBlob = GetCloudBlob(key);
            return GetSizeAsync(cloudBlob, cancellationToken);
        }

        public async Task<long> GetSizeAsync(CloudBlob cloudBlob, CancellationToken cancellationToken)
        {
            await cloudBlob.FetchAttributesAsync(accessCondition: null, options: null, operationContext: null, cancellationToken: cancellationToken);
            return cloudBlob.Properties.Length;
        }

        public override async Task<DateTime> GetLastModificationDateAsync(string key, CancellationToken cancellationToken)
        {
            var cloudBlob = GetCloudBlob(key);
            await cloudBlob.FetchAttributesAsync(accessCondition: null, options: null, operationContext: null, cancellationToken: cancellationToken);
            
            if (cloudBlob.Properties.LastModified == null)
                throw new InvalidOperationException($"Could not get the last modified property of CloudBlob '{key}'.");

            return cloudBlob.Properties.LastModified.Value.DateTime;
        }

        public override Task<bool> IsDirectoryAsync(string key, CancellationToken cancellationToken)
        {
            return Task.FromResult(false); // TODO ???
        }

        public override Task<bool> ExistsAsync(string key, CancellationToken cancellationToken)
        {
            var cloudBlob = GetCloudBlob(key);
            return cloudBlob.ExistsAsync(
                options: null,
                operationContext: null,
                cancellationToken: cancellationToken);
        }

        public override Task<bool> DeleteAsync(string key, CancellationToken cancellationToken)
        {
            var cloudBlob = GetCloudBlob(key);
            return cloudBlob.DeleteIfExistsAsync(
                deleteSnapshotsOption:DeleteSnapshotsOption.IncludeSnapshots, //TODO snapshots ???
                accessCondition:null, 
                operationContext: null, 
                options: null,
                cancellationToken: cancellationToken);
        }

        public override async Task<bool> RenameAsync(string key, string newKey, CancellationToken cancellationToken)
        {
            var blob = GetCloudBlockBlob(key);
            var blobCopy = GetCloudBlockBlob(newKey);
            
            await blobCopy.StartCopyAsync(blob, 
                sourceAccessCondition: null,
                destAccessCondition: null,
                options: null,
                operationContext: null, 
                cancellationToken: cancellationToken);

            await blob.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots,
                accessCondition: null,
                options: null,
                operationContext: null,
                cancellationToken: cancellationToken);

            return true;
        }

        #endregion

        #region Implementation of IMetadataSupporter

        public Task SetMetadataAsync(string key, IDictionary<string, string> metadata, CancellationToken cancellationToken)
        {
            var blob = GetCloudBlob(key);
            
            blob.Metadata.Clear();

            foreach (var meta in metadata)
                blob.Metadata.Add(meta);

            return blob.SetMetadataAsync(
                accessCondition: null,
                options: null,
                operationContext: null,
                cancellationToken: cancellationToken);
        }

        public async Task<IDictionary<string, string>> GetMetadataAsync(string key, CancellationToken cancellationToken)
        {
            var blob = GetCloudBlob(key);

            await blob.FetchAttributesAsync(
                accessCondition: null,
                options: null,
                operationContext: null,
                cancellationToken: cancellationToken);

            return blob.Metadata;
        }

        #endregion

        #region Protected methods

        protected string ComputeKey(string path) => NormalizePath(path);

        protected string ComputePath(string key) => NormalizePath(key);

        protected CloudBlob GetCloudBlob(string key)
        {
            var path = ComputePath(key);
            var cloudBlob = Container.GetBlobReference(path);
            return cloudBlob;
        }

        protected CloudBlockBlob GetCloudBlockBlob(string key)
        {
            var path = ComputePath(key);
            var cloudBlockBlob = Container.GetBlockBlobReference(path);
            return cloudBlockBlob;
        }

        protected string NormalizePath(string path) => PathHelper.Normalize(path);

        protected Task UpdateContentTypePropertiesAsync(string key, CloudBlockBlob blob, CancellationToken cancellationToken)
        {
            if (DetectContentType)
            {
                var contentType = GetMimeType(key);
                blob.Properties.ContentType = contentType;
                return blob.SetPropertiesAsync(
                    accessCondition: null,
                    options: null,
                    operationContext: null,
                    cancellationToken: cancellationToken);
            }

            return Task.CompletedTask;
        }

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

        public async Task<string> GetChecksumAsync(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            var blob = GetCloudBlockBlob(key);

            await blob.FetchAttributesAsync(
                accessCondition: null,
                options: null,
                operationContext: null,
                cancellationToken: cancellationToken);

            return blob.Properties.ContentMD5;
        }
        
    }
}