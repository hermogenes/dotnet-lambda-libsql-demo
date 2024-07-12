using System.Diagnostics.CodeAnalysis;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace Lambda.Demo.MinimalApi.DynamoDb;

public class DynamoDbProductStore : IProductStore
{
    private static readonly string ProductTableName =
        Environment.GetEnvironmentVariable("PRODUCT_TABLE_NAME") ?? throw new Exception("Missing ");

    private readonly AmazonDynamoDBClient _dynamoDbClient;

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(DynamoDbProductStore))]
    public DynamoDbProductStore()
    {
        _dynamoDbClient = new AmazonDynamoDBClient();
        _dynamoDbClient.DescribeTableAsync(ProductTableName).GetAwaiter().GetResult();
    }

    public async Task<Product?> GetProduct(string id)
    {
        var getItemResponse = await _dynamoDbClient.GetItemAsync(new GetItemRequest(ProductTableName,
            new Dictionary<string, AttributeValue>(1)
            {
                { ProductMapper.Pk, new AttributeValue(id) }
            }));

        return getItemResponse.IsItemSet ? ProductMapper.ProductFromDynamoDb(getItemResponse.Item) : null;
    }

    public async Task PutProduct(Product product)
    {
        await _dynamoDbClient.PutItemAsync(ProductTableName, ProductMapper.ProductToDynamoDb(product));
    }

    public async Task DeleteProduct(string id)
    {
        await _dynamoDbClient.DeleteItemAsync(ProductTableName, new Dictionary<string, AttributeValue>(1)
        {
            { ProductMapper.Pk, new AttributeValue(id) }
        });
    }

    public async Task<Product[]> GetAllProducts()
    {
        var data = await _dynamoDbClient.ScanAsync(new ScanRequest
        {
            TableName = ProductTableName,
            Limit = 20
        });

        return data.Items.Select(ProductMapper.ProductFromDynamoDb).ToArray();
    }
}