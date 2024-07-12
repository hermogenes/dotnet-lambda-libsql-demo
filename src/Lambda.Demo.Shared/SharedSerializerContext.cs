using System.Text.Json.Serialization;

namespace Lambda.Demo.Shared;

[JsonSerializable(typeof(Product))]
[JsonSerializable(typeof(Product[]))]
public partial class SharedSerializerContext : JsonSerializerContext
{
    
}
