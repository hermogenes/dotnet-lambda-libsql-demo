namespace Lambda.Demo.MinimalApi.DynamoDb;

public interface IProductStore
{
    Task<Product?> GetProduct(string id);

    Task PutProduct(Product product);

    Task DeleteProduct(string id);

    Task<Product[]> GetAllProducts();
}
