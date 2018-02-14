# FileSystem API

## General Usage

__Get Files__
```csharp
IFile file = await fileSystem.GetAsync("path/to/file.txt");
IFile file = await fileSystem.GetAsync("path/to/file.txt", create:true); 
IFile file = await fileSystem.GetAsync("path/to/file.txt", create:true, cancellationToken: cancel);

// create 
//    if `true` : Create file if not exists
//    else : Throw if not exists
```

__Write Files__
```csharp
await file.WriteAsync(byteArray);
await file.WriteAsync(stream);

// overwrite
await file.WriteAsync(byteArray, overwrite:true);
await file.WriteAsync(stream,    overwrite:true);

// overwrite and/or cancellationToken
await file.WriteAsync(byteArray, overwrite:true, cancellationToken: cancel);
await file.WriteAsync(stream,    overwrite:true, cancellationToken: cancel);
```

__Read Files__
```csharp
// Read to byte array
byte[] byteArray = await file.ReadAllBytesAsync();
byte[] byteArray = await file.ReadAllBytesAsync(cancellationToken: cancel);

//Read to stream
await file.ReadToStreamAsync(stream);
await file.ReadToStreamAsync(stream, cancellationToken: cancel);
```

__Check if a file exists__
```csharp
bool exists = await file.ExistsAsync();
bool exists = await file.ExistsAsync(cancellationToken: cancel);
```

> __NOTE:__ This only has consistent behaviour for files, not directories. Directories are less important in Flysystem, they're created implicitly and often ignored because not every adapter (filesystem type) supports directories.

__Delete Files__
```csharp
await file.DeleteAsync();
await file.DeleteAsync(cancellationToken: cancel);
```

__Rename Files__
```csharp
await file.RenameAsync("newname.txt");
await file.RenameAsync("newname.txt", cancellationToken: cancel);
```

__Copy Files__
```csharp
//TODO IFile file = await file.CopyAsync("copy.txt");
//TODO IFile file = await file.CopyAsync("copy.txt", cancellationToken: cancel);

IFile otherFile = await fileSystem.GetAsync("path/to/other/file.txt");

await file.CopyToAsync(otherFile);
await file.CopyToAsync(otherFile, cancellationToken: cancel);
```

__Get Mimetypes__
```csharp
string mimeType = await file.GetMimeTypeAsync();
string mimeType = await file.GetMimeTypeAsync(cancellationToken: cancel);
```

__Get Timestamps__
```csharp
DateTime date = await file.GetLastModificationDateAsync();
DateTime date = await file.GetLastModificationDateAsync(cancellationToken: cancel);
```

__Get File sizes (in bytes)__
```csharp
long sizeInBytes = await file.GetSizeAsync();
long sizeInBytes = await file.GetSizeAsync(cancellationToken: cancel);
```

__Get Checksum__
```csharp
string checksum = await file.GetChecksumAsync();
string checksum = await file.GetChecksumAsync(cancellationToken: cancel);
```