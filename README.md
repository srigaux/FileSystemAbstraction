# FileSystemAbstraction

FileSystemAbstraction provides a filesystem abstraction layer in `.net`.

Inspired by [Gaufrette](https://knplabs.github.io/Gaufrette/) (PHP) :
>The filesystem abstraction layer permits you to develop your application without the need to know were all those medias will be stored and how.
>
>Another advantage of this is the possibility to update the files location without any impact on the code apart from the definition of your filesystem. In example, if your project grows up very fast and if your server reaches its limits, you can easily move your medias in an Azure server or any other solution.

# Status

[![FileSystemAbstractionNuGet](https://img.shields.io/nuget/v/FileSystemAbstraction.svg?style=flat-square&label=FileSystemAbstraction)](https://www.nuget.org/packages/FileSystemAbstraction/)

| Adapters           | Status                                                               |
|--------------------|----------------------------------------------------------------------|
| Local              | [![FileSystemAbstractionAdaptersLocalNuGet](https://img.shields.io/nuget/v/FileSystemAbstraction.Adapters.Local.svg?style=flat-square&label=FileSystemAbstraction.Adapters.Local)](https://www.nuget.org/packages/FileSystemAbstraction.Adapters.Local/) |
| Azure Blob Storage | [![FileSystemAbstractionAdaptersAzureBlobStorageNuGet](https://img.shields.io/nuget/v/FileSystemAbstraction.Adapters.AzureBlobStorage.svg?style=flat-square&label=FileSystemAbstraction.Adapters.AzureBlobStorage)](https://www.nuget.org/packages/FileSystemAbstraction.Adapters.AzureBlobStorage/) |
