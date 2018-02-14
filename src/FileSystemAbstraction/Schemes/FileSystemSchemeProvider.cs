using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileSystemAbstraction.Adapters;
using Microsoft.Extensions.Options;

namespace FileSystemAbstraction.Schemes
{
    /// <summary>
    /// Implements <see cref="IFileSystemSchemeProvider" />.
    /// </summary>
    public class FileSystemSchemeProvider : IFileSystemSchemeProvider
    {
        private readonly object _lock = new object();
        private readonly IDictionary<string, FileSystemScheme> _map = new Dictionary<string, FileSystemScheme>(StringComparer.Ordinal);
        private readonly List<FileSystemScheme> _fileSystems = new List<FileSystemScheme>();
        private readonly FileSystemOptions _options;

        /// <summary>Constructor.</summary>
        /// <param name="options">The <see cref="FileSystemOptions" /> options.</param>
        public FileSystemSchemeProvider(IOptions<FileSystemOptions> options)
        {
            _options = options.Value;

            foreach (var scheme in _options.Schemes)
                AddScheme(scheme.Build());
        }

        /// <inheritdoc />
        public Task<FileSystemScheme> GetDefaultSchemeAsync()
        {
            if (_options.DefaultScheme == null)
                return Task.FromResult((FileSystemScheme) null);

            return GetSchemeAsync(_options.DefaultScheme);
        }

        /// <inheritdoc />
        public virtual Task<FileSystemScheme> GetSchemeAsync(string name) 
            => Task.FromResult(_map.ContainsKey(name) ? _map[name] : null);

        /// <inheritdoc />
        public virtual Task<IEnumerable<FileSystemScheme>> GetAdapterSchemesAsync()
        {
            lock (_lock)
            {
                return Task.FromResult((IEnumerable<FileSystemScheme>) _fileSystems);
            }
        }

        /// <inheritdoc />
        public virtual void AddScheme(FileSystemScheme scheme)
        {
            if (_map.ContainsKey(scheme.Name))
                throw new InvalidOperationException("FileSystem Scheme already exists: " + scheme.Name);

            lock (_lock)
            {
                if (_map.ContainsKey(scheme.Name))
                    throw new InvalidOperationException("FileSystem Scheme already exists: " + scheme.Name);

                if (typeof(IFileSystemAdapter).IsAssignableFrom(scheme.AdapterType))
                    _fileSystems.Add(scheme);

                _map[scheme.Name] = scheme;
            }
        }

        /// <inheritdoc />
        public virtual void RemoveScheme(string name)
        {
            if (!_map.ContainsKey(name))
                return;

            lock (_lock)
            {
                if (!_map.ContainsKey(name))
                    return;

                _fileSystems.Remove(_map[name]);
                _map.Remove(name);
            }
        }

        /// <inheritdoc />

        public virtual Task<IEnumerable<FileSystemScheme>> GetAllSchemesAsync() 
            => Task.FromResult((IEnumerable<FileSystemScheme>) _map.Values);
    }
}