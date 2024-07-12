using Lambda.Demo.Shared;
using LibSql.Http.Client.Interfaces;

namespace Lambda.Demo.MinimalApi.LibSql;

public class LibSqlProductStore(ILibSqlHttpClient client) : IProductStore
{
    private const string SelectAllSql = "select id, name, price from products";

    private const string SelectByIdSql = "select id, name, price from products where id = ? limit 1";

    private const string DeleteSql = "delete from products where id = ?";

    private const string InsertSql =
        "insert into products (id, name, price) values (?, ?, ?) on conflict(id) do update set name = excluded.name, price = excluded.price;";

    public Task<Product?> GetProduct(string id) =>
        client.QueryFirstOrDefaultAsync((SelectByIdSql, [id]), SharedSerializerContext.Default.Product);

    public Task PutProduct(Product product) =>
        client.ExecuteAsync((InsertSql, [product.Id, product.Name, product.Price]));

    public Task DeleteProduct(string id) => client.ExecuteAsync((DeleteSql, [id]));

    public async Task<Product[]> GetAllProducts()
    {
        var products = await client.QueryAsync(SelectAllSql, SharedSerializerContext.Default.Product);

        return products.ToArray();
    }
}
