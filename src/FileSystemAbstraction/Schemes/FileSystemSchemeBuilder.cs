using System;

namespace FileSystemAbstraction.Schemes
{
    /// <summary>
    /// Used to build <see cref="FileSystemScheme" />s.
    /// </summary>
    public class FileSystemSchemeBuilder
    {
        /// <summary>Constructor.</summary>
        /// <param name="name">The name of the scheme being built.</param>
        public FileSystemSchemeBuilder(string name)
        {
            Name = name;
        }

        /// <summary>The name of the scheme being built.</summary>
        public string Name { get; }

        /// <summary>
        /// The <see cref="IFileSystem" /> type responsible for this scheme.
        /// </summary>
        public Type AdapterType { get; set; }

        /// <summary>
        /// Builds the <see cref="FileSystemScheme" /> instance.
        /// </summary>
        /// <returns></returns>
        public FileSystemScheme Build()
            => new FileSystemScheme(Name, AdapterType);
    }
}