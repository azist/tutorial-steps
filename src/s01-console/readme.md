# Step 1 - CLI (Command Line Interface)

This is the very first step. We are going to build a CLI - command line interface app which takes input from
the command line args. We need to first re-visit the conventions of project organization.



## Project Structure Recap 

This section must be carefully studied to eliminate many problems during dev and DevOps activities
in the future.

This is just a recap. See Azos repository for standards/conventions details.

**All project source, elements/artifacts and documentation MUST be included in the same repository**.
Anyone who gets the project source should be able to build, test, deploy project, this includes: source
vector graphic files, source project files (e.g. 3d Design) etc. The only **exception is for 
media files larger than 10 mb** or smaller media files which change often (e.g. audio/video files) in 
which case these files need to be stored in some accessible content storage location having their URL disclosed 
under a suitable `/elm` subdirectory.

Every .Net project must have at least one `readme.md` file in the root, explaining what the project does,

Project structure overview:
```
 /                   <---- repo mount root
    doc/             <---- project documentation in *.MD
    elm/             <---- related elements: scripts, devops, db, graphics source
    out/             <---- build output added to GITINGORE (not checked-in)
      Debug/
      Release/
    src/             <---- Primary application source code
      *.sln          <---- Solution file(s)
      ProjectX       <---- Solution projects
        Class1.cs
        readme.md
```



### Use proper .gitignore
The gitignore file must exclude `obj` and `bin` project sub folders used by MsBuild.
See the `.gitignore` in this project for guidance

### Use LF
Do not use CRLF line endings. Use LF line endings for files but CMD(which should be avoided).
LF works cross platform.

### Use Linux Paths
Do not use Windows-specific "\" backslash. Use "/" forward slash which works on both Windows and Linux.
Do not use hard-coded root paths containing drive letters etc.. use relative paths
```
BAD:
 c:\mydata\myapp
 /mnt/sda0/c/mydata

GOOD:
 $(~HOME)/mydata/myapp
```

### Use root `Directory.Build.props`
MSBuild applies the settings from the upper directories to lower ones.
A good project structure should use a root-level MsBuild file which declares global variables.

A typical global variable use-case is the package version `AzosVersion`:

```xml
<Project>
 <PropertyGroup>
    <Version>......</Version>
    <AzosVersion>1.11.0</AzosVersion>
    .......
 </PropertyGroup>
 ......
</Project>
```

The subordinate projects of the solution then reference the global version tag. (see below)

### All server application must use SERVER GC
A server GC mode works better for server apps (such as API servers).
To enable server mode GC, we add a line to project file directly (using "Edit project")

```xml
<Project Sdk="Microsoft.NET.Sdk">
 .....
  <PropertyGroup>
    <TargetFramework>netcoreapp......</TargetFramework>
.....
    <ServerGarbageCollection>true</ServerGarbageCollection>
  </PropertyGroup>
 ......
</Project>
```


### Package references use Project Variables 
When your solution grows large it becomes harder and harder to maintain all references
when package versions change. .NET provides solution out-of-box. Instead of hard-coding
Nuget reference versions using VS tools, use project file editor:

```xml

 <ItemGroup>
    <PackageReference Include="Azos" Version="$(AzosVersion)" />
 </ItemGroup>
```

Notice the use of variable `$(AzosVersion)`. Now you can reference this variable across 
the whole solution.

The variable is defined in the `Directory.Build.props` file in source root. (see above)


## Console App Examples


tbd....