using Lambda.Demo.Shared;
using LibSql.Http.Client.Interfaces;
using LibSql.Http.Client.Request;

namespace Lambda.Demo.MinimalApi.LibSql;

public class LibSqlProductStore : IProductStore
{
    private readonly ILibSqlHttpClient _client;

    public LibSqlProductStore(ILibSqlHttpClient client)
    {
        _client = client;
        // This is a hack to trigger HTTP connection as early as possible during function warm-up.
        // Probably there are better ways to do this "pre connection"
        _client.HealthCheckAsync().GetAwaiter().GetResult();
    }

    private const string SelectAllSql = "select id, name, price from products limit 20";

    private const string SelectByIdSql = "select id, name, price from products where id = ? limit 1";

    private const string DeleteSql = "delete from products where id = ?";

    private const string InsertSql =
        "insert into products (id, name, price) values (?, ?, ?) on conflict(id) do update set name = excluded.name, price = excluded.price;";

    public Task<Product?> GetProduct(string id) =>
        _client.QueryFirstOrDefaultAsync(new Statement(SelectByIdSql, [id]), SharedSerializerContext.Default.Product);

    public Task PutProduct(Product product) =>
        _client.ExecuteAsync(new Statement(InsertSql, [product.Id, product.Name, product.Price]));

    public Task DeleteProduct(string id) => _client.ExecuteAsync(new Statement(DeleteSql, [id]));

    public async Task<Product[]> GetAllProducts()
    {
        var products = await _client.QueryAsync(SelectAllSql, SharedSerializerContext.Default.Product);

        return products.ToArray();
    }
}