using System;
using System.CommandLine;
using System.CommandLine.Help;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema;

namespace Deimdal.JsonSchemaConvert;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        const SchemaVersion defaultVersion = SchemaVersion.Draft2019_09;

        var sourceOption = new Option<string>("--source", "-s")
        {
            Required = true,
            AllowMultipleArgumentsPerToken = false,
            Description = "Source schema location (file or URL)."
        };
        var destFileOption = new Option<FileInfo>("--dest-file", "-d")
        {
            Required = true,
            AllowMultipleArgumentsPerToken = false,
            Description = "Destination schema file name."
        };
        var destVersionOption = new Option<SchemaVersion>("--dest-version", "-v")
        {
            AllowMultipleArgumentsPerToken = false,
            Description = "Destination schema version.",
            DefaultValueFactory = _ => SchemaVersion.Draft2019_09,
            CustomParser = result =>
            {
                if (result.Tokens.Count == 0)
                    return defaultVersion;
                var argument = result.Tokens.Single().Value;
                return !Enum.TryParse<SchemaVersion>(argument, out var schemaVersion)
                    ? throw new FormatException($"Invalid schema version param: {argument}")
                    : schemaVersion;
            }
        };

        var rootCommand = new Command("convert-json-schema", "Json schema version conversion utility")
        {
            sourceOption,
            destFileOption,
            destVersionOption,
            new HelpOption(),
            new VersionOption()
        };

        rootCommand.SetAction((parseResult, _) => Converter.ConvertSchema(
            parseResult.GetRequiredValue(sourceOption),
            parseResult.GetRequiredValue(destFileOption),
            parseResult.GetValue(destVersionOption)));

        var parseResult = rootCommand.Parse(args);
        return await parseResult.InvokeAsync();
    }
}
