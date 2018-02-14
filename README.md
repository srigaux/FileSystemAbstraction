# FileSystemAbstraction

FileSystemAbstraction provides a filesystem abstraction layer in `.net`.

Inspired by [Gaufrette](https://knplabs.github.io/Gaufrette/) (PHP) :
>The filesystem abstraction layer permits you to develop your application without the need to know were all those medias will be stored and how.
>
>Another advantage of this is the possibility to update the files location without any impact on the code apart from the definition of your filesystem. In example, if your project grows up very fast and if your server reaches its limits, you can easily move your medias in an Azure server or any other solution.

## Status

[![FileSystemAbstractionNuGet](https://img.shields.io/nuget/v/FileSystemAbstraction.svg?style=flat-square&label=FileSystemAbstraction)](https://www.nuget.org/packages/FileSystemAbstraction/)

| Adapters           | Status                                                               |
|--------------------|----------------------------------------------------------------------|
| Local              | [![FileSystemAbstractionAdaptersLocalNuGet](https://img.shields.io/nuget/v/FileSystemAbstraction.Adapters.Local.svg?style=flat-square&label=FileSystemAbstraction.Adapters.Local)](https://www.nuget.org/packages/FileSystemAbstraction.Adapters.Local/) |
| Azure Blob Storage | [![FileSystemAbstractionAdaptersAzureBlobStorageNuGet](https://img.shields.io/nuget/v/FileSystemAbstraction.Adapters.AzureBlobStorage.svg?style=flat-square&label=FileSystemAbstraction.Adapters.AzureBlobStorage)](https://www.nuget.org/packages/FileSystemAbstraction.Adapters.AzureBlobStorage/) |

## Installation

You can install it through _NuGet_:
```ps
Install-Package FileSystemAbstraction
```

## Basic Usage
Following an example with the local filesystem adapter. To setup adapters, look up their respective documentation.

### 1. Install the Local Adapter package:

```ps
Install-Package FileSystemAbstraction.Adapters.Local
```

### 2. Configure the _FileSystem_

In _AspNetCore_ project : `Startup.cs` 
```csharp

public void ConfigureServices(IServiceCollection services)
{
    //...

    services
        .AddFileSystem(LocalFileSystemDefaults.DefaultScheme)
        .AddLocal(@"c:\name\of\base\directory");

    //...
}
```
### 3. Use the _FileSystem_ to read/write

Through [_**FileSystem** API_](./docs/api_file-system.md):

```csharp

// Get your fileSystem by DependencyInjection
IFileSystem fs = services.GetService<IFileSystem>(); 

// Check the file existence :
var exists = await fs.ExistsAsync("foo/bar.txt"); // false
if (!exists) 
{
    // Write "Hello world" to 'bar.txt'
    byte[] bytes = Encoding.UTF8.GetBytes("Hello world");
    await fs.WriteAsync("foo/bar.txt", bytes);
}

// Read 'bar.txt' content :
bytes = await fs.ReadAllBytesAsync("foo/bar.txt");
var result = Encoding.UTF8.GetString(bytes); // Hello world
```

Through [_**File** API_](./docs/api_file.md)

```csharp

// Get your fileSystem by DependencyInjection
IFileSystem fs = services.GetService<IFileSystem>(); 

// Get a file reference: (linked to c:\name\of\base\directory\foo\bar.txt)
// No file realy created at this point
IFile file = await fileSystem.GetAsync("foo/bar.txt");

// Check the file existence :
var exists = await file.ExistsAsync(); // false
if (!exists) 
{
    // Write "Hello world" to 'bar.txt'
    byte[] bytes = Encoding.UTF8.GetBytes("Hello world");
    await file.WriteAsync(bytes);
}

// Read 'bar.txt' content :
bytes = await file.ReadAllBytesAsync();
var result = Encoding.UTF8.GetString(bytes); // Hello world
```

## Documentation
[Check out the documentation](./docs/index.md)