using System;
using System.Collections.Generic;
using FileSystemAbstraction.Adapters;
using FileSystemAbstraction.Schemes;

namespace FileSystemAbstraction
{
    public class FileSystemOptions
    {
        private readonly IList<FileSystemSchemeBuilder> _schemes = new List<FileSystemSchemeBuilder>();


        /// <summary>
        /// Returns the schemes in the order they were added (important for request handling priority)
        /// </summary>
        public IEnumerable<FileSystemSchemeBuilder> Schemes 
            => _schemes;

        /// <summary>Maps schemes by name.</summary>
        public IDictionary<string, FileSystemSchemeBuilder> SchemeMap { get; } 
            = new Dictionary<string, FileSystemSchemeBuilder>(StringComparer.Ordinal);

        /// <summary>
        /// Adds an <see cref="FileSystemScheme" />.
        /// </summary>
        /// <param name="name">The name of the scheme being added.</param>
        /// <param name="configureBuilder">Configures the scheme.</param>
        public void AddScheme(string name, Action<FileSystemSchemeBuilder> configureBuilder)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (configureBuilder == null) throw new ArgumentNullException(nameof(configureBuilder));
            if (SchemeMap.ContainsKey(name)) throw new InvalidOperationException("FileSystem Scheme already exists: " + name);

            var fileSystemSchemeBuilder = new FileSystemSchemeBuilder(name);
            configureBuilder(fileSystemSchemeBuilder);
            _schemes.Add(fileSystemSchemeBuilder);
            SchemeMap[name] = fileSystemSchemeBuilder;
        }

        /// <summary>
        /// Adds an <see cref="FileSystemScheme" />.
        /// </summary>
        /// <typeparam name="TFileSystemAdapter">The <see cref="IFileSystem" /> responsible for the scheme.</typeparam>
        /// <param name="name">The name of the scheme being added.</param>
        public void AddScheme<TFileSystemAdapter>(string name) where TFileSystemAdapter : IFileSystemAdapter 
            => AddScheme(name, b => b.AdapterType = typeof(TFileSystemAdapter));

        /// <summary>
        /// Used as the fallback default scheme.
        /// </summary>
        public string DefaultScheme { get; set; }
    }
}