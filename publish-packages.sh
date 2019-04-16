#!/bin/bash

declare -a Projects=(
    "FileSystemAbstraction"
    "FileSystemAbstraction.Adapters.Local"
    "FileSystemAbstraction.Adapters.AzureBlobStorage"
)

rm -rf packages

dotnet pack --configuration Release --include-source --include-symbols -p:SymbolPackageFormat=snupkg --output ../../packages FileSystemAbstraction.sln

for package in packages/*.nupkg; do
    [ -e "$package" ] || continue
    
    echo ""
    echo ""
    echo "⬆ Uplaoding package $package ..."
    
    dotnet nuget push $package -s https://api.nuget.org/v3/index.json || echo "❌ Failed to upload package $package!"
done;

