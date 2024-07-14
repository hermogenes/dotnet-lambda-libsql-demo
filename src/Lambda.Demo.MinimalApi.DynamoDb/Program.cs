using System.Text.Json.Serialization.Metadata;
using Amazon.Lambda.Serialization.SystemTextJson;
using Lambda.Demo.MinimalApi.DynamoDb;
using Lambda.Demo.Shared;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolver = ApiLibSqlSerializerContext.Default;
});

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi, new SourceGeneratorLambdaJsonSerializer<ApiLibSqlSerializerContext>());

builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole(options =>
{
    options.IncludeScopes = true;
    options.UseUtcTimestamp = true;
    options.TimestampFormat = "hh:mm:ss ";
}); 

var app = builder.Build();

var handlers = new ProductService(new DynamoDbProductStore(), app.Logger);

app.MapGet("/", async (context) =>
{
    var result = await handlers.GetAllProducts();
    await context.WriteResponse(result.Item1, result.Item2, SharedSerializerContext.Default.ProductArray);
});

app.MapDelete("/{id}", async (string id, HttpContext context) =>
{
    var result = await handlers.DeleteProduct(id);

    await context.WriteResponse(result.Item1, result.Item2, SharedSerializerContext.Default.String);
});

app.MapPut("/{id}", async (string id, HttpContext context) =>
{
    var result = await handlers.PutProduct(id, context.Request.Body);

    await context.WriteResponse(result.Item1, result.Item2, SharedSerializerContext.Default.String);
});

app.MapGet("/{id}", async (string id, HttpContext context) =>
{
    var result = await handlers.GetProduct(id);

    if (result.Item2 is null)
    {
        await context.WriteResponse(result.Item1, $"{id} not found", SharedSerializerContext.Default.String);
        return;
    }

    await context.WriteResponse(result.Item1, result.Item2, SharedSerializerContext.Default.Product);
});

app.Run();

internal static class HttpContextExtensions
{
    internal static async Task WriteResponse<T>(this HttpContext context, int statusCode, T body, JsonTypeInfo<T> typeInfo)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        if (body is not null)
        {
            await context.Response.WriteAsJsonAsync(body, typeInfo);
        }
    }
}