using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Lambda.Demo.Shared;

public class ProductService(IProductStore store, ILogger logger)
{
    public async Task<(int, string?)> GetAllProducts()
    {
        logger.LogInformation("Received request to list all products");

        var products = await store.GetAllProducts();

        logger.LogInformation("Found {total} products(s)", products.Length);

        return (200, JsonSerializer.Serialize(products, SharedSerializerContext.Default.ProductArray));
    }

    public async Task<(int, string?)> DeleteProduct(string id)
    {
        try
        {
            logger.LogInformation("Received request to delete {id}", id);

            var product = await store.GetProduct(id);

            if (product is null)
            {
                logger.LogWarning("Id {id} not found.", id);

                return (404, null);
            }

            logger.LogInformation("Deleting {productName}", product.Name);

            await store.DeleteProduct(product.Id);

            logger.LogInformation("Delete complete");

            return (200,
                JsonSerializer.Serialize($"Product with id {id} deleted", SharedSerializerContext.Default.String));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failure deleting product");

            return (400,
                JsonSerializer.Serialize($"Failure deleting product with id {id}",
                    SharedSerializerContext.Default.String));
        }
    }

    public async Task<(int, string?)> GetProduct(string id)
    {
        logger.LogInformation("Received request to get {id}", id);

        var product = await store.GetProduct(id);

        var code = product is null ? 404 : 200;
        var message = product is null
            ? JsonSerializer.Serialize($"{id} not found", SharedSerializerContext.Default.String)
            : JsonSerializer.Serialize(product, SharedSerializerContext.Default.Product);

        if (product is null)
        {
            logger.LogWarning("{id} not found", id);
        }

        return (code, message);
    }

    public async Task<(int, string?)> PutProduct(string id, Stream body)
    {
        var product =
            await JsonSerializer.DeserializeAsync(body, SharedSerializerContext.Default.Product);

        if (product is null || id != product.Id)
        {
            return (400,
                JsonSerializer.Serialize("Product ID in the body does not match path parameter",
                    SharedSerializerContext.Default.String));
        }

        await store.PutProduct(product);

        return (200, JsonSerializer.Serialize($"Created product with id {id}", SharedSerializerContext.Default.String));
    }
}
