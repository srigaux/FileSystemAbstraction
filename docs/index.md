# Introduction

Like [_FlySystem_](https://github.com/thephpleague/flysystem/blob/master/docs/index.md) for PHP :

_FileSystemAbstraction_ is a filesystem abstraction which allows you to easily swap out a local filesystem for a remote one. 

## Goals
- [x] Have a generic API for handling common tasks across multiple file storage engines.
- [x] Have consistent output which you can rely on.
- [ ] Integrate well with other packages/frameworks.
- [ ] Be cacheable.
- [ ] Emulate directories in systems that support none, like AwsS3.
- [ ] Support third party plugins.
- [x] Make it easy to test your filesystem interactions.
- [x] Support streams for big file handling

## APIs:

If you have only one _FileSystem Adapter_ (Local/AzureBlobStorage/...), you should use :
- [_**FileSystem** API_](./api_file-system.md) : Access all methods through _FileSystem_
- [_**File** API_](./api_file.md) : Get a _IFile_ through _**FileSystem** API_ and access to its methods

Otherwise, you shoul use :
- [_**Multiple Adpaters FileSystem** API_](./api_multiple-file-system.md) : Access all methods through _FileSystem_ by _scheme_.
- [_**File** API_](./api_file.md) : Get a _IFile_ through _**Multiple Adpaters FileSystem** API_ and access to its methods