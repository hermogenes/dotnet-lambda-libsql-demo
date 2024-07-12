namespace Lambda.Demo.Shared;

public interface IProductStore
{
    Task<Product?> GetProduct(string id);

    Task PutProduct(Product product);

    Task DeleteProduct(string id);

    Task<Product[]> GetAllProducts();
}