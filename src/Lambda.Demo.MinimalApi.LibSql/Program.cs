using System.Net;
using System.Net.Http.Headers;
using Amazon.Lambda.Serialization.SystemTextJson;
using Lambda.Demo.MinimalApi.LibSql;
using Lambda.Demo.Shared;
using LibSql.Http.Client;

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

var handler = new SocketsHttpHandler
{
    AutomaticDecompression = DecompressionMethods.All,
    PooledConnectionLifetime = TimeSpan.FromMinutes(15) // Recreate every 15 minutes
};

var sharedClient = new HttpClient(handler)
{
    BaseAddress = new Uri(Environment.GetEnvironmentVariable("LIBSQL_CLIENT_URL") ?? throw new Exception("LIBSQL_CLIENT_URL not configured")),
    DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("LIBSQL_CLIENT_TOKEN"))}
};

var libSqlClient = new LibSqlHttpClient(sharedClient);

var handlers = new ProductService(new LibSqlProductStore(libSqlClient), app.Logger);

app.MapGet("/", async (context) =>
{
    var result = await handlers.GetAllProducts();
    
    await context.WriteResponse(result.Item1, result.Item2);
});

app.MapDelete("/{id}", async (string id, HttpContext context) =>
{
    var result = await handlers.DeleteProduct(id);
    
    await context.WriteResponse(result.Item1, result.Item2);
});

app.MapPut("/{id}", async (string id, HttpContext context) =>
{
    var result = await handlers.PutProduct(id, context.Request.Body);
    
    await context.WriteResponse(result.Item1, result.Item2);
});

app.MapGet("/{id}", async (string id, HttpContext context) =>
{
    var result = await handlers.GetProduct(id);
    
    await context.WriteResponse(result.Item1, result.Item2);
});

app.Run();

internal static class HttpContextExtensions
{
    internal static async Task WriteResponse(this HttpContext context, int statusCode, string? body)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        if (body is not null)
        {
            await context.Response.WriteAsync(body);
        }
    }
}
