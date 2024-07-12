using System.Globalization;
using Amazon.DynamoDBv2.Model;

namespace Lambda.Demo.MinimalApi.DynamoDb;

public class ProductMapper
{
    public const string Pk = "id";
    private const string Name = "name";
    private const string Price = "price";

    public static Product ProductFromDynamoDb(Dictionary<string, AttributeValue> items)
    {
        var product = new Product(items[Pk].S, items[Name].S, decimal.Parse(items[Price].N));

        return product;
    }

    public static Dictionary<string, AttributeValue> ProductToDynamoDb(Product product)
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