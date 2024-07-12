using System.Text.Json.Serialization;

namespace Lambda.Demo.MinimalApi.DynamoDb;

[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(Product))]
[JsonSerializable(typeof(Product[]))]
public partial class SharedSerializerContext : JsonSerializerContext
{

}
