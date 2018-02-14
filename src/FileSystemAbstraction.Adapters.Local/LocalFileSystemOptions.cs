using System;
using FileSystemAbstraction.Helpers;
using FileSystemAbstraction.Schemes;
using Microsoft.Extensions.Options;

namespace FileSystemAbstraction.Adapters.Local
{
    public class LocalFileSystemOptions : FileSystemSchemeOptions
    {
        public string Directory { get; set; }
        public bool Create { get; set; } = false;

        public override void Validate()
        {
            if (Directory == null)
                throw new InvalidOperationException($"The {nameof(Directory)} property option for the local file system should not be null.");
        }

        internal class PostConfigureOptions : IPostConfigureOptions<LocalFileSystemOptions>
        {
            public void PostConfigure(string name, LocalFileSystemOptions options)
            {
                options.Directory = PathHelper.Normalize(options.Directory);
            }
        }
    }
}