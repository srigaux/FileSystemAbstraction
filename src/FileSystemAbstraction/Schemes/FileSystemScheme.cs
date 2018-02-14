using System;
using FileSystemAbstraction.Adapters;

namespace FileSystemAbstraction.Schemes
{
    /// <summary>
    /// FileSystemSchemes assign a name to a specific <see cref="IFileSystem" />
    /// fileSystemType.
    /// </summary>
    public class FileSystemScheme
    {
        /// <summary>Constructor.</summary>
        /// <param name="name">The name for the filesystem scheme.</param>
        /// <param name="adapterType">The <see cref="IFileSystem" /> type that handles this scheme.</param>
        public FileSystemScheme(string name, Type adapterType)
        {
            if (name == null) 
                throw new ArgumentNullException(nameof(name));

            if (adapterType == null)
                throw new ArgumentNullException(nameof(adapterType));

            if (!typeof(IFileSystemAdapter).IsAssignableFrom(adapterType))
                throw new ArgumentException($"{nameof(adapterType)} must implement {nameof(IFileSystemAdapter)}", nameof(adapterType));

            Name = name;
            AdapterType = adapterType;
        }

        /// <summary>The name of the filesystem scheme.</summary>
        public string Name { get; }

        /// <summary>
        /// The <see cref="IFileSystemAdapter" /> type that handles this scheme.
        /// </summary>
        public Type AdapterType { get; }
    }
}