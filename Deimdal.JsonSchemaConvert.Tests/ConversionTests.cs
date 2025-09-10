using Newtonsoft.Json.Schema;
using Xunit;

namespace Deimdal.JsonSchemaConvert.Tests;

public class ConversionTests
{
    [Theory]
    [InlineData("petstore.json", "petstore_converted_2019_09.json", SchemaVersion.Draft2019_09)]
    [InlineData("petstore.json", "petstore_converted_2020_12.json", SchemaVersion.Draft2020_12)]
    [InlineData("petstore.json", "petstore_converted_7.json", SchemaVersion.Draft7)]
    [InlineData("petstore.json", "petstore_converted_2019_09.yaml", SchemaVersion.Draft2019_09)]
    [InlineData("petstore.json", "petstore_converted_2020_12.yaml", SchemaVersion.Draft2020_12)]
    [InlineData("petstore.json", "petstore_converted_7.yaml", SchemaVersion.Draft7)]
    public async Task ConvertJsonSuccess(string input, string output, SchemaVersion version)
    {
        var sourcePath = Path.Combine(AppContext.BaseDirectory, "Schemas", input);
        var actual = await Converter.ConvertSchema(sourcePath, version);

        var pathActual = Path.Combine(AppContext.BaseDirectory, "Schemas", output);
        var expected = await File.ReadAllTextAsync(pathActual);
        Assert.Equal(expected, actual);
    }
}
