using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Lambda.Demo.MinimalApi.LibSql;

internal static class HttpContextExtensions
{
    public static void WriteResponse(this HttpContext context, HttpStatusCode statusCode)
    {
        context.Response.StatusCode = (int)statusCode;
    }
    
    public static async Task WriteResponse<TResponseType>(this HttpContext context, HttpStatusCode statusCode, TResponseType body, JsonTypeInfo<TResponseType> typeInfo) where TResponseType : class
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(body, typeInfo));
    }
}