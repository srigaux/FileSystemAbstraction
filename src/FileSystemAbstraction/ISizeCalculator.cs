using System.Threading;
using System.Threading.Tasks;

namespace FileSystemAbstraction
{
    public interface ISizeCalculator
    {
        Task<long> GetSizeAsync(string key, CancellationToken cancellationToken = default(CancellationToken));
    }
}