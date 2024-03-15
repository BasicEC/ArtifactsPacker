# ArtifactsPacker

It's a simple console tool that allows to you to remove similar files in a directory with the ability to restore them later.

# Examples

## Publish
```bash
dotnet publish ArtifactsPacker/ArtifactsPacker.csproj -o publish
```

## Show help

```bash
dotnet publish/ArtifactsPacker.dll help
```
Console output:
```
ArtifactsPacker 1.0.0
Copyright (C) 2024 ArtifactsPacker

  pack       Pack files in specified directory.

  unpack     Unpack files in specified directory.

  help       Display more information on a specific command.

  version    Display version information.
```

```bash
dotnet publish/ArtifactsPacker.dll help pack
```
Console output:
```
ArtifactsPacker 1.0.0
Copyright (C) 2024 ArtifactsPacker

  --src        Required. Path to a directory to pack

  --trg        Required. Path to a directory to place a result

  --archive    Place the packed result into an archive

  --help       Display this help screen.

  --version    Display version information.
```

## Pack

```bash
dotnet publish/ArtifactsPacker.dll pack --src "./ArtifactsPacker.Tests/TestFiles/PackTestIn" --trg "./out.zip" --archive
```
Console output:
```
info: ArtifactsPacker.Services.PackService[0]
      Start hash calculation
info: ArtifactsPacker.Services.PackService[0]
      Hash calculation complete
info: ArtifactsPacker.Services.PackService[0]
      Start packing
info: ArtifactsPacker.Services.PackService[0]
      Packing complete
```

## Unpack
```bash
mkdir unpack_result
dotnet publish/ArtifactsPacker.dll unpack --src "./out.zip" --trg "unpack_result" --archive
```
Console output:
```
info: ArtifactsPacker.Services.PackService[0]
      Start unpacking
info: ArtifactsPacker.Services.PackService[0]
      Unpacking complete
```