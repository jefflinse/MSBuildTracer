# MSBuildTracer
A tracer tool for querying MSBuild projects for properties, targets, imports, etc.

## Properties

``MSBuildTracer project.csproj -p CSharpTargetsPath``

##### Output
```
[CSharpTargetsPath]
Location:  C:\Program Files (x86)\MSBuild\14.0\bin\Microsoft.CSharp.targets:36
U-Value:   $(MSBuildFrameworkToolsPath)\Microsoft.CSharp.targets
E-Value:   C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\\Microsoft.CSharp.targets
```

## Targets

``MSBuildTracer project.csproj -t ResolveAssemblyReferences``

##### Output
```
ResolveAssemblyReferences
|   GetFrameworkPaths
|   GetReferenceAssemblyPaths
|   |   GetWinFXPath
|   PrepareForBuild
|   |   GetFrameworkPaths
|   |   GetReferenceAssemblyPaths
|   |   |   GetWinFXPath
|   ResolveSDKReferences
|   |   GetInstalledSDKLocations
|   ExpandSDKReferences
|   |   ResolveSDKReferences
|   |   |   GetInstalledSDKLocations
```

## Imports

``MSBuildTracer project.csproj -i``

##### Output
```
3: C:\Program Files (x86)\MSBuild\14.0\Microsoft.Common.props

84: C:\Program Files (x86)\MSBuild\14.0\bin\Microsoft.CSharp.targets
    142: C:\Program Files (x86)\MSBuild\4.0\Microsoft.Common.targets\ImportBefore\*
    154: C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\\Microsoft.CSharp.targets
```
