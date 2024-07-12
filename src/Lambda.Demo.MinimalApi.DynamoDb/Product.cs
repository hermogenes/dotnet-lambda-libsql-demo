using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Lambda.Demo.MinimalApi.DynamoDb;

[method: DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Product))]
public record Product(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("price")] decimal Price);
