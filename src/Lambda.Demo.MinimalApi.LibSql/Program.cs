using System.Net.Http.Headers;
using Amazon.Lambda.Serialization.SystemTextJson;
using Lambda.Demo.MinimalApi.LibSql;
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
    PooledConnectionLifetime = TimeSpan.FromMinutes(15) // Recreate every 15 minutes
};

var sharedClient = new HttpClient(handler)
{
    BaseAddress = new Uri(Environment.GetEnvironmentVariable("LIBSQL_CLIENT_URL") ?? throw new Exception("LIBSQL_CLIENT_URL not configured")),
    DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("LIBSQL_CLIENT_TOKEN"))}
};

var libSqlClient = new LibSqlHttpClient(sharedClient);

var handlers = new Handlers(new LibSqlProductStore(libSqlClient), app.Logger);

app.MapGet("/", handlers.GetAllProducts);

app.MapDelete("/{id}", handlers.DeleteProduct);

app.MapPut("/{id}", handlers.PutProduct);

app.MapGet("/{id}", handlers.GetProduct);

app.Run();