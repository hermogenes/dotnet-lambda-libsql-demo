namespace Lambda.Demo.MinimalApi.LibSql;

public interface IProductStore
{
    Task<Product?> GetProduct(string id);

    Task PutProduct(Product product);

    Task DeleteProduct(string id);

    Task<Product[]> GetAllProducts();
}