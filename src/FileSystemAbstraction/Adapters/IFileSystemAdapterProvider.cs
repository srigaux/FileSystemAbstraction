using System.Threading.Tasks;

namespace FileSystemAbstraction.Adapters
{
    /// <summary>
    /// Provides the appropriate <see cref="IFileSystemAdapter"/> instance for the fileSystemScheme.
    /// </summary>
    public interface IFileSystemAdapterProvider
    {
        /// <summary>Returns the adapter instance that will be used.</summary>
        /// <param name="fileSystemScheme">The name of the fileSystm scheme being handled.</param>
        /// <returns>The adapter instance.</returns>
        Task<IFileSystemAdapter> GetAdapterAsync(string fileSystemScheme);
    }
}