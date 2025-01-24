# Json Schema version conversion utility
`dotnet` tool for JSON schema versions conversion

---

This software is free and licensed under Apache license 2.0.  
Transitive dependency library [Newtonsoft.Json.Schema](https://www.nuget.org/packages/newtonsoft.json.schema/) has [licensing restrictions](https://github.com/JamesNK/Newtonsoft.Json.Schema/blob/master/LICENSE.md) for commercial usage, please review it.

## Installation (global tool)
```shell
dotnet tool install deimdal.jsonschemaconvert --global
```

## Usage
```shell
convert-json-schema -s <source schema file or URL> -d <destination schema file> -v <destination version>
```

## See help for usage details
```shell
convert-json-schema -h
```
