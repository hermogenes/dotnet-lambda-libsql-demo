using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Lambda.Demo.Shared;

public class ProductService(IProductStore store, ILogger logger)
{
    public async Task<(int, Product[])> GetAllProducts()
    {
        logger.LogInformation("Received request to list all products");

        var products = await store.GetAllProducts();

        logger.LogInformation("Found {total} products(s)", products.Length);

        return (200, products);
    }

    public async Task<(int, string)> DeleteProduct(string id)
    {
        try
        {
            logger.LogInformation("Received request to delete {id}", id);

            var product = await store.GetProduct(id);

            if (product is null)
            {
                logger.LogWarning("Id {id} not found.", id);

                return (404, "Id {id} not found.");
            }

            logger.LogInformation("Deleting {productName}", product.Name);

            await store.DeleteProduct(product.Id);

            logger.LogInformation("Delete complete");

            return (200, $"Product with id {id} deleted");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failure deleting product");

            return (400, $"Failure deleting product with id {id}");
        }
    }

    public async Task<(int, Product?)> GetProduct(string id)
    {
        logger.LogInformation("Received request to get {id}", id);

        var product = await store.GetProduct(id);

        var code = product is null ? 404 : 200;

        if (product is null)
        {
            logger.LogWarning("{id} not found", id);
        }

        return (code, product);
    }

    public async Task<(int, string)> PutProduct(string id, Stream body)
    {
        var product =
            await JsonSerializer.DeserializeAsync(body, SharedSerializerContext.Default.Product);

        if (product is null || id != product.Id)
        {
            return (400,
                "Product ID in the body does not match path parameter");
        }

        await store.PutProduct(product);

        return (200, $"Created product with id {id}");
    }
}