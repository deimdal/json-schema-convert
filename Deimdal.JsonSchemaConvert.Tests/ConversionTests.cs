using Newtonsoft.Json.Schema;
using Xunit;

namespace Deimdal.JsonSchemaConvert.Tests;

public class ConversionTests
{
    [Theory]
    [InlineData("Schemas\\petstore.json", "Schemas\\petstore_converted_2019_09.json", SchemaVersion.Draft2019_09)]
    [InlineData("Schemas\\petstore.json", "Schemas\\petstore_converted_2020_12.json", SchemaVersion.Draft2020_12)]
    [InlineData("Schemas\\petstore.json", "Schemas\\petstore_converted_7.json", SchemaVersion.Draft7)]
    [InlineData("Schemas\\petstore.json", "Schemas\\petstore_converted_2019_09.yaml", SchemaVersion.Draft2019_09)]
    [InlineData("Schemas\\petstore.json", "Schemas\\petstore_converted_2020_12.yaml", SchemaVersion.Draft2020_12)]
    [InlineData("Schemas\\petstore.json", "Schemas\\petstore_converted_7.yaml", SchemaVersion.Draft7)]
    public async Task ConvertJsonSuccess(string input, string output, SchemaVersion version)
    {
        var actual = await Converter.ConvertSchema(input, version);

        var path = Path.Combine(AppContext.BaseDirectory, output);
        var expected = await File.ReadAllTextAsync(path);
        Assert.Equal(expected, actual);
    }
}
