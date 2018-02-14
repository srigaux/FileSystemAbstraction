using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileSystemAbstraction.Schemes
{
    public interface IFileSystemSchemeProvider
    {
        /// <summary>
        /// Returns all currently registered <see cref="FileSystemScheme" />s.
        /// </summary>
        /// <returns>All currently registered <see cref="FileSystemScheme" />s.</returns>
        Task<IEnumerable<FileSystemScheme>> GetAllSchemesAsync();

        /// <summary>
        /// Returns the <see cref="FileSystemScheme" /> matching the name, or null.
        /// </summary>
        /// <param name="name">The name of the FileSystemScheme.</param>
        /// <returns>The scheme or null if not found.</returns>
        Task<FileSystemScheme> GetSchemeAsync(string name);

        /// <summary>
        /// Returns the scheme that will be used by default" />.
        /// This is typically specified via <see cref="FileSystemOptions" />.
        /// </summary>
        /// <returns>The scheme that will be used by default.</returns>
        Task<FileSystemScheme> GetDefaultSchemeAsync();

        /// <summary>
        /// Registers a scheme for use by <see cref="IFileSystem" />.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        void AddScheme(FileSystemScheme scheme);

        /// <summary>
        /// Removes a scheme, preventing it from being used by <see cref="IFileSystem" />.
        /// </summary>
        /// <param name="name">The name of the FileSystemScheme being removed.</param>
        void RemoveScheme(string name);

        /// <summary>
        /// Returns the schemes in priority order for request handling.
        /// </summary>
        /// <returns>The schemes in priority order for request handling</returns>
        Task<IEnumerable<FileSystemScheme>> GetAdapterSchemesAsync();
    }
}