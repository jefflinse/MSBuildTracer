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
because the file exists

58: C:\Program Files (x86)\MSBuild\14.0\bin\Microsoft.CSharp.targets

    142: C:\Program Files (x86)\MSBuild\4.0\Microsoft.Common.targets\ImportBefore\Microsoft.WindowsPhone.v8.0.MT.targets
    because '$(ImportByWildcardBefore40MicrosoftCommonTargets)' == 'true' and the file exists

    154: C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\Microsoft.CSharp.targets

         379: C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\Microsoft.Common.targets

              31: <path to input project>\MSBuildTracer.csproj.user
              because the file exists

              67: C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\Microsoft.NETFramework.props
              because '$(TargetFrameworkIdentifier)' == '.NETFramework' or '$(TargetFrameworkIdentifier)' == 'Silverlight'

              5033: C:\Program Files (x86)\MSBuild\14.0\bin\Microsoft.NETFramework.targets
              because ('$(TargetFrameworkIdentifier)' == ''  or '$(TargetFrameworkIdentifier)' == '.NETFramework') and ('$(TargetRuntime)' == 'Managed')

                    72: C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\Microsoft.NetFramework.targets

                        116: C:\Program Files (x86)\MSBuild\14.0\bin\Microsoft.WinFX.targets
                        because '$(TargetFrameworkVersion)' != 'v2.0' and '$(TargetCompactFramework)' != 'true' and the file exists

                             12: C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\Microsoft.WinFx.targets

                        117: C:\Program Files (x86)\MSBuild\14.0\bin\Microsoft.Data.Entity.targets
                        because '$(TargetFrameworkVersion)' != 'v2.0' and '$(TargetFrameworkVersion)' !=  'v3.0' and the file exists

                             12: C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\Microsoft.Data.Entity.targets

              5035: C:\Program Files (x86)\MSBuild\14.0\bin\Microsoft.Xaml.targets
              because ('$(TargetFrameworkVersion)' != 'v2.0' and '$(TargetFrameworkVersion)' != 'v3.5') and the file exists

                    19: C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\Microsoft.Xaml.targets

              5039: C:\Program Files (x86)\MSBuild\14.0\bin\Microsoft.WorkflowBuildExtensions.targets
              because ('$(TargetFrameworkVersion)' != 'v2.0' and '$(TargetFrameworkVersion)' != 'v3.5' and (!$([System.String]::IsNullOrEmpty('$(TargetFrameworkVersion)')) and !$(TargetFrameworkVersion.StartsWith('v4.0')))) and the file exists

                    25: C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\Microsoft.WorkflowBuildExtensions.targets

         380: C:\Program Files (x86)\MSBuild\14.0\bin\Microsoft.ServiceModel.targets
         because ('$(TargetFrameworkVersion)' != 'v2.0' and '$(TargetFrameworkVersion)' != 'v3.0' and '$(TargetFrameworkVersion)' != 'v3.5') and the file exists

              12: C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\Microsoft.ServiceModel.targets
```
