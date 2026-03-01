// Test file to check OpenAPI namespace
using Microsoft.OpenApi;

class TestOpenApi
{
    public void Test()
    {
        // Try to access something from Microsoft.OpenApi
        var version = typeof(Microsoft.OpenApi.OpenApiDocument).Assembly.GetName().Version;
        Console.WriteLine(version);
    }
}
