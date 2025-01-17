using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Lambda.Demo.Shared;

namespace Lambda.Demo.MinimalApi.DynamoDb;

public class DynamoDbProductStore : IProductStore
{
    private const string Pk = "id";
    private const string Name = "name";
    private const string Price = "price";

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
                { Pk, new AttributeValue(id) }
            }));

        return getItemResponse.IsItemSet ? ProductFromDynamoDb(getItemResponse.Item) : null;
    }

    public async Task PutProduct(Product product)
    {
        await _dynamoDbClient.PutItemAsync(ProductTableName, ProductToDynamoDb(product));
    }

    public async Task DeleteProduct(string id)
    {
        await _dynamoDbClient.DeleteItemAsync(ProductTableName, new Dictionary<string, AttributeValue>(1)
        {
            { Pk, new AttributeValue(id) }
        });
    }

    public async Task<Product[]> GetAllProducts()
    {
        var data = await _dynamoDbClient.ScanAsync(new ScanRequest
        {
            TableName = ProductTableName,
            Limit = 20
        });

        return data.Items.Select(ProductFromDynamoDb).ToArray();
    }

    private static Product ProductFromDynamoDb(Dictionary<string, AttributeValue> items)
    {
        var product = new Product(items[Pk].S, items[Name].S, decimal.Parse(items[Price].N));

        return product;
    }

    private static Dictionary<string, AttributeValue> ProductToDynamoDb(Product product)
    {
        var item = new Dictionary<string, AttributeValue>(3)
        {
            { Pk, new AttributeValue(product.Id) },
            { Name, new AttributeValue(product.Name) },
            {
                Price, new AttributeValue
                {
                    N = product.Price.ToString(CultureInfo.InvariantCulture)
                }
            }
        };

        return item;
    }
}
