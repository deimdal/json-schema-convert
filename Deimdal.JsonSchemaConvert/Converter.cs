using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema;

namespace Deimdal.JsonSchemaConvert;

public static class Converter
{
    public static async Task ConvertSchema(string source, FileInfo destinationFile, SchemaVersion destinationVersion)
    {
        var schema = await ConvertSchema(source, destinationVersion);
        Console.WriteLine($"Saving to '{destinationFile}'...");

        if (destinationFile.Directory is { Exists: false })
            destinationFile.Directory.Create();

        await File.WriteAllTextAsync(destinationFile.FullName, schema);

        Console.WriteLine("Conversion finished");
    }

    public static async Task<string> ConvertSchema(string source, SchemaVersion version)
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
        return schema;
    }
}
