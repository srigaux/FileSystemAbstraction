using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FileSystemAbstraction
{
    public class InMemoryFileRegister : IFileRegister
    {
        public InMemoryFileRegister()
        {
            Register = new ConcurrentDictionary<string, Lazy<Dictionary<string, IFile>>>();
        }

        private ConcurrentDictionary<string, Lazy<Dictionary<string, IFile>>> Register { get; }
        
        public Task<bool> RemoveAsync(string scheme, string key, CancellationToken cancellationToken)
        {
            if (scheme == null) throw new ArgumentNullException(nameof(scheme));
            if (key == null) throw new ArgumentNullException(nameof(key));

            var register = GetSchemeRegister(scheme);
            var deleted = register.Remove(key);

            return Task.FromResult(deleted);
        }

        public Task<bool> ContainsAsync(string scheme, string key, CancellationToken cancellationToken)
        {
            if (scheme == null) throw new ArgumentNullException(nameof(scheme));
            if (key == null) throw new ArgumentNullException(nameof(key));

            var register = GetSchemeRegister(scheme);
            var exists = register.ContainsKey(key);

            return Task.FromResult(exists);
        }

        public Task<IFile> GetOrDefaultAsync(string scheme, string key, CancellationToken cancellationToken)
        {
            if (scheme == null) throw new ArgumentNullException(nameof(scheme));
            if (key == null) throw new ArgumentNullException(nameof(key));

            var register = GetSchemeRegister(scheme);

            return register.TryGetValue(key, out var file)
                ? Task.FromResult(file) 
                : Task.FromResult<IFile>(null);
        }

        public Task StoreAsync(string scheme, string key, IFile file, CancellationToken cancellationToken)
        {
            if (scheme == null) throw new ArgumentNullException(nameof(scheme));
            if (key == null) throw new ArgumentNullException(nameof(key));

            var register = GetSchemeRegister(scheme);

            register[key] = file;

            return Task.CompletedTask;
        }

        private Dictionary<string, IFile> GetSchemeRegister(string scheme) => 
            Register.GetOrAdd(scheme, _ => new Lazy<Dictionary<string, IFile>>()).Value;
    }
}