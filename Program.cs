﻿using System;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema;

namespace Deimdal.JsonSchemaConvert;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        const SchemaVersion defaultVersion = SchemaVersion.Draft2019_09;

        var sourceOption = new Option<string>(["-s", "--source"])
        {
            IsRequired = true,
            AllowMultipleArgumentsPerToken = false,
            Description = "Source schema location (file or URL)."
        };
        var destFileOption = new Option<FileInfo>(["-d", "--dest-file"])
        {
            IsRequired = true,
            AllowMultipleArgumentsPerToken = false,
            Description = "Destination schema file name."
        };
        var destVersionOption = new Option<SchemaVersion>(
            aliases: ["-v", "--dest-version"],
            isDefault: true,
            parseArgument: result =>
            {
                if (result.Tokens.Count == 0)
                    return defaultVersion;

                var argument = result.Tokens.Single().Value;
                if (!Enum.TryParse<SchemaVersion>(argument, out var schemaVersion))
                    throw new FormatException($"Invalid schema version param: {argument}");
                return schemaVersion;
            })
        {
            AllowMultipleArgumentsPerToken = false,
            Description = $"Destination schema version.",
        };

        var rootCommand = new Command("convert-json-schema", "Json schema version conversion utility")
        {
            sourceOption,
            destFileOption,
            destVersionOption
        };

        rootCommand.SetHandler(ConvertSchema, sourceOption, destFileOption, destVersionOption);

        return await rootCommand.InvokeAsync(args);
    }

    private static async Task ConvertSchema(string source, FileInfo destinationFile, SchemaVersion version)
    {
        string schema;
        {
            string data;
            if (Uri.IsWellFormedUriString(source, UriKind.Absolute))
            {
                Console.WriteLine($"Downloading '{source}'...");
                using var client = new HttpClient();
                data = await client.GetStringAsync(source);
            }
            else
            {
                if (!File.Exists(source))
                    throw new FileNotFoundException($"File '{source}' not found.");
                data = await File.ReadAllTextAsync(source);
            }

            Console.WriteLine("Parsing schema...");
            var parsedSchema = JSchema.Parse(data);
            schema = parsedSchema.ToString(version);
        }
        Console.WriteLine($"Saving to '{destinationFile}'...");

        if (destinationFile.Directory is { Exists: false })
            destinationFile.Directory.Create();

        await File.WriteAllTextAsync(destinationFile.FullName, schema);

        Console.WriteLine("Conversion finished");
    }
}
