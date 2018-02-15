# FileSystem API

If you have only one _Adapter_ (Local/Azure BlobStorage/...) you can use the general usage.

Otherwise you should use the [_**Multiple Adapters** FileSystem API_](./api_multiple-file-system.md).

## General Usage

__Write Files__
```csharp
await fileSystem.WriteAsync("path/to/file.txt", byteArray);
await fileSystem.WriteAsync("path/to/file.txt", stream);

// overwrite
await fileSystem.WriteAsync("path/to/file.txt", byteArray, overwrite:true);
await fileSystem.WriteAsync("path/to/file.txt", stream,    overwrite:true);

// overwrite and/or cancellationToken
await fileSystem.WriteAsync("path/to/file.txt", byteArray, overwrite:true, cancellationToken: cancel);
await fileSystem.WriteAsync("path/to/file.txt", stream,    overwrite:true, cancellationToken: cancel);
```

__Read Files__
```csharp
// Read to byte array
byte[] byteArray = await fileSystem.ReadAllBytesAsync("path/to/file.txt");
byte[] byteArray = await fileSystem.ReadAllBytesAsync("path/to/file.txt", cancellationToken: cancel);

//Read to stream
await fileSystem.ReadToStreamAsync("path/to/file.txt", stream);
await fileSystem.ReadToStreamAsync("path/to/file.txt", stream, cancellationToken: cancel);
```

__Check if a file exists__
```csharp
bool exists = await fileSystem.ExistsAsync("path/to/file.txt");
bool exists = await fileSystem.ExistsAsync("path/to/file.txt", cancellationToken: cancel);
```

> __NOTE:__ This only has consistent behaviour for files, not directories. Directories are less important in Flysystem, they're created implicitly and often ignored because not every adapter (filesystem type) supports directories.

__Delete Files__
```csharp
await fileSystem.DeleteAsync("path/to/file.txt");
await fileSystem.DeleteAsync("path/to/file.txt", cancellationToken: cancel);
```

__Rename Files__
```csharp
await fileSystem.RenameAsync("filename.txt", "newname.txt");
await fileSystem.RenameAsync("filename.txt", "newname.txt", cancellationToken: cancel);
```

__Copy Files__
```csharp
//TODO await fileSystem.CopyAsync("filename.txt", "copy.txt");
//TODO await fileSystem.CopyAsync("filename.txt", "copy.txt", cancellationToken: cancel);
```

__Get Mimetypes__
```csharp
string mimeType = await fileSystem.GetMimeTypeAsync("path/to/file.txt");
string mimeType = await fileSystem.GetMimeTypeAsync("path/to/file.txt", cancellationToken: cancel);
```

__Get Timestamps__
```csharp
DateTime date = await fileSystem.GetLastModificationDateAsync("path/to/file.txt");
DateTime date = await fileSystem.GetLastModificationDateAsync("path/to/file.txt", cancellationToken: cancel);
```

__Get File sizes (in bytes)__
```csharp
long sizeInBytes = await fileSystem.GetSizeAsync("path/to/file.txt");
long sizeInBytes = await fileSystem.GetSizeAsync("path/to/file.txt", cancellationToken: cancel);
```

__Get Checksum__
```csharp
string checksum = await fileSystem.GetChecksumAsync("path/to/file.txt");
string checksum = await fileSystem.GetChecksumAsync("path/to/file.txt", cancellationToken: cancel);
```