using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FileSystemAbstraction.Helpers
{
    public static class StreamExtensions
    {
        public static Task CopyToAsync(this Stream stream, Stream toStream, CancellationToken cancellationToken)
            => stream.CopyToAsync(toStream, bufferSize: 91820, cancellationToken: cancellationToken);
    }
}