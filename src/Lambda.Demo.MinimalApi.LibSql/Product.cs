using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Lambda.Demo.MinimalApi.LibSql;

[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Product))]
public record Product(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("price")] decimal Price);