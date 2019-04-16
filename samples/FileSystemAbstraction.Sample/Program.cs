using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FileSystemAbstraction.Adapters.Local;
using Microsoft.Extensions.DependencyInjection;

namespace FileSystemAbstraction.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            await RunAsync(serviceProvider);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddFileSystem(defaultScheme: "Local1")
                .AddLocal(
                    scheme: "Local1", 
                    directory: Path.Combine(Directory.GetCurrentDirectory(), "Files/Local1"), 
                    create:true)
                .AddLocal(
                    scheme: "Local2",
                    directory: Path.Combine(Directory.GetCurrentDirectory(), "Files/Local2"),
                    create: true)
                ;
        }

        static async Task RunAsync(IServiceProvider services)
        {
            var fileSystem = services.GetService<IFileSystem>();

            // ====== FileSystem API Read/Write file with default scheme (Local1)
            await ReadWriteDefaultSchemeAsync(fileSystem);

            // ====== File API with another schemes =========
            var barFile = await FileApiReadWriteAsync(fileSystem);

            // ====== File API across schemes =========
            var movedFile = await FileApiMoveFileAcrossSchemesAsync(fileSystem, barFile);

            // ====== File API delete movedFile ========
            await FileApiDeleteAsync(fileSystem, movedFile);

            // ====== FileSystem API delete test.txt ========
            await FileSystemApiDeleteWithScheme(fileSystem);
        }

        private static async Task ReadWriteDefaultSchemeAsync(IFileSystem fileSystem)
        {
            const string fileName = "test.txt";


            // Write 'Hello world!' to 'test.txt' (using default scheme : Local1)
            //  --> Files/Local1/test.txt
            await fileSystem.WriteAsync(fileName, Encoding.UTF8.GetBytes("Hello world!"), overwrite: true);


            // Read the content of 'test.txt' (using default scheme : Local1)
            //  --> Files/Local1/test.txt
            var content = await fileSystem.ReadAllBytesAsync(fileName);
            Console.WriteLine("Content of 'Files/Local1/test.txt' : " + Encoding.UTF8.GetString(content));
        }

        private static async Task<IFile> FileApiReadWriteAsync(IFileSystem fileSystem)
        {
            const string fileName = "bar.txt";

            // Get not yet existing file 'bar.txt (using scheme : Local2)
            //  --> Files/Local2/bar.txt
            var barFile = await fileSystem.GetAsync("Local2", fileName, create: true);


            // Write 'Foo' to 'bar.txt' (using the second scheme : Local2)
            //  --> Files/Local2/bar.txt
            await barFile.WriteAsync(Encoding.UTF8.GetBytes("Foo"), overwrite: true);


            // Read the content of 'test.text' (using default scheme : Local2)
            //  --> Files/Local2/bar.txt
            var content = await barFile.ReadAllBytesAsync();
            Console.WriteLine("Content of 'Files/Local2/bar.txt' : " + Encoding.UTF8.GetString(content));

            return barFile;
        }

        private static async Task<IFile> FileApiMoveFileAcrossSchemesAsync(IFileSystem fileSystem, IFile fileToMove)
        {
            const string fileName = "bar1.txt";

            // Get not yet existing file 'bar1.txt (using scheme : Local1)
            //  --> Files/Local1/bar1.txt
            var newFile = await fileSystem.GetAsync("Local1", fileName, create: true);

            await fileToMove.MoveToAsync(newFile);

            // Read the content of 'bar1.txt' (using scheme : Local1)
            //  --> Files/Local1/bar1.txt
            var content = await fileSystem.ReadAllBytesAsync(fileName);
            Console.WriteLine("Content of 'Files/Local1/bar1.txt' : " + Encoding.UTF8.GetString(content));

            // Check existence of originalFile (should be deleted because of the move operation)
            var exists = await fileToMove.ExistsAsync();
            Console.WriteLine("'Files/Local2/bar.txt' existence : " + exists);

            return newFile;
        }

        private static async Task FileApiDeleteAsync(IFileSystem fileSystem, IFile fileToDelete)
        {
            await fileToDelete.DeleteAsync();
        }

        private static async Task FileSystemApiDeleteWithScheme(IFileSystem fileSystem)
        {
            await fileSystem.DeleteAsync("Local1", "test.txt");
        }
    }
}
