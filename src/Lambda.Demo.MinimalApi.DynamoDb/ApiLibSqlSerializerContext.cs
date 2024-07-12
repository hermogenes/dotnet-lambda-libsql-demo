using System.Text.Json.Serialization;
using Amazon.Lambda.APIGatewayEvents;

namespace Lambda.Demo.MinimalApi.DynamoDb;

[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyRequest))]
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyResponse))]
public partial class ApiLibSqlSerializerContext : JsonSerializerContext
{

}
