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
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            Task.Run(() => RunAsync(serviceProvider)).GetAwaiter().GetResult();
        }

        static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddFileSystem()
                .AddLocal(Path.Combine(Directory.GetCurrentDirectory(), "Files"), create:true);
        }

        static async Task RunAsync(IServiceProvider services)
        {
            var fileSystem = services.GetService<IFileSystem>();

            var fileName = "test.txt";

            // Write 'Hello world!' to 'test.txt'
            await fileSystem.WriteAsync(fileName, Encoding.UTF8.GetBytes("Hello world!"), overwrite:true);

            // Read the content of th file 'test.text'
            var content = await fileSystem.ReadAllBytesAsync(fileName);

            Console.WriteLine("Content : " + Encoding.UTF8.GetString(content));
        }
    }
}
