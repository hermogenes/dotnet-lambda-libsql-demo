using System.Net;
using System.Text.Json;

namespace Lambda.Demo.MinimalApi.LibSql;

public sealed class Handlers(IProductStore store, ILogger logger)
{
    public async Task GetAllProducts(HttpContext context)
    {
        logger.LogInformation("Received request to list all products");

        var products = await store.GetAllProducts();

        logger.LogInformation("Found {total} products(s)", products.Length);

        await context.WriteResponse(HttpStatusCode.OK, products, SharedSerializerContext.Default.ProductArray);
    }

    public async Task DeleteProduct(string id, HttpContext context)
    {
        try
        {
            logger.LogInformation("Received request to delete {id}", id);

            var product = await store.GetProduct(id);

            if (product is null)
            {
                logger.LogWarning("Id {id} not found.", id);

                context.WriteResponse(HttpStatusCode.NotFound);

                return;
            }

            logger.LogInformation("Deleting {productName}", product.Name);

            await store.DeleteProduct(product.Id);

            logger.LogInformation("Delete complete");

            await context.WriteResponse(HttpStatusCode.OK, $"Product with id {id} deleted",
                SharedSerializerContext.Default.String);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failure deleting product");

            context.WriteResponse(HttpStatusCode.BadRequest);
        }
    }

    public async Task GetProduct(string id, HttpContext context)
    {
        logger.LogInformation("Received request to get {id}", id);

        var product = await store.GetProduct(id);

        if (product is null)
        {
            logger.LogWarning("{id} not found", id);
            await context.WriteResponse(HttpStatusCode.NotFound, $"{id} not found",
                SharedSerializerContext.Default.String);
            return;
        }

        await context.WriteResponse(HttpStatusCode.OK, product, SharedSerializerContext.Default.Product);
    }

    public async Task PutProduct(string id, HttpContext context)
    {
        var product =
            await JsonSerializer.DeserializeAsync(context.Request.Body, SharedSerializerContext.Default.Product);

        if (product is null || id != product.Id)
        {
            await context.WriteResponse(HttpStatusCode.BadRequest,
                "Product ID in the body does not match path parameter", SharedSerializerContext.Default.String);
            return;
        }

        await store.PutProduct(product);

        await context.WriteResponse(HttpStatusCode.OK, $"Created product with id {id}",
            SharedSerializerContext.Default.String);
    }
}