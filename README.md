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
$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props
$(MSBuildToolsPath)\Microsoft.CSharp.targets
|   $(MSBuildExtensionsPath)\4.0\Microsoft.Common.targets\ImportBefore\*
|   $(CSharpTargetsPath)
|   |   Microsoft.Common.targets
|   |   |   $(MSBuildProjectFullPath).user
|   |   |   Microsoft.NETFramework.props
|   |   |   $(MSBuildToolsPath)\Microsoft.NETFramework.targets
|   |   |   |   $(NetFrameworkTargetsPath)
|   |   |   |   |   $(MSBuildToolsPath)\Microsoft.WinFX.targets
|   |   |   |   |   |   $(MSBuildFrameworkToolsPath)\Microsoft.WinFx.targets
|   |   |   |   |   $(MSBuildToolsPath)\Microsoft.Data.Entity.targets
|   |   |   |   |   |   |   $(MSBuildFrameworkToolsPath)\Microsoft.Data.Entity.targets
|   |   |   $(MSBuildToolsPath)\Microsoft.Xaml.targets
|   |   |   |   |   |   |   |   $(MSBuildFrameworkToolsPath)\Microsoft.Xaml.targets
|   |   |   $(MSBuildToolsPath)\Microsoft.WorkflowBuildExtensions.targets
|   |   |   |   |   |   |   |   |   $(MSBuildFrameworkToolsPath)\Microsoft.WorkflowBuildExtensions.targets
|   |   $(MSBuildToolsPath)\Microsoft.ServiceModel.targets
|   |   |   |   |   |   |   |   |   |   $(MSBuildFrameworkToolsPath)\Microsoft.ServiceModel.targets
```
