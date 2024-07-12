using Amazon.Lambda.Serialization.SystemTextJson;
using Lambda.Demo.MinimalApi.DynamoDb;

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

var handlers = new Handlers(new DynamoDbProductStore(), app.Logger);

app.MapGet("/", handlers.GetAllProducts);

app.MapDelete("/{id}", handlers.DeleteProduct);

app.MapPut("/{id}", handlers.PutProduct);

app.MapGet("/{id}", handlers.GetProduct);

app.Run();
