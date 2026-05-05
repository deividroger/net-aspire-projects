using catalog.Services;

namespace catalog.Endpoints;

public static class ProductEnpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/products");

        group.MapPost("/", async (Product product, ProductService service) =>
        {
            await service.CreateProductAsync(product);
            return Results.Created($"/{product.Id}", product);
        })
        .WithName("CreateProduct")
        .Produces<Product>(StatusCodes.Status201Created);

        group.MapGet("/", async (ProductService service) =>
        {
            var products = await service.GetProductsAsync();
            return Results.Ok(products);
        })
        .WithName("GetProducts")
        .Produces<List<Product>>(StatusCodes.Status200OK);

        group.MapGet("{id}", async (int id, ProductService service) =>
        {
            var product = await service.GetProductByIdAsync(id);
            return product is not null ? Results.Ok(product) : Results.NotFound();
        })
        .WithName("GetProductById")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id}", async (int id, Product updatedProduct, ProductService service) =>
        {
            var existingProduct = await service.GetProductByIdAsync(id);
            if (existingProduct is null) return Results.NotFound();
            existingProduct.Name = updatedProduct.Name;
            existingProduct.Description = updatedProduct.Description;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.ImageUrl = updatedProduct.ImageUrl;
            await service.UpdateProductAsync(existingProduct);
            return Results.NoContent();
        })
        .WithName("UpdateProduct")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id}", async (int id, ProductService service) =>
        {
            var existingProduct = await service.GetProductByIdAsync(id);
            if (existingProduct is null) return Results.NotFound();
            await service.DeleteProductAsync(id);
            return Results.NoContent();
        })
        .WithName("DeleteProduct")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        //Support AI
        group.MapGet("/support/{query}", async (string query, ProductAIService service) =>
        {
            var response = await service.SupportAsync(query);
            return Results.Ok(response);
        }).WithName("SupportAI")
        .Produces(StatusCodes.Status200OK);

        // Tradicional search
        group.MapGet("search/{query}", async (string query, ProductService service) =>
        {
            var products = await service.SearchProductsAsync(query);
            return Results.Ok(products);
        }).WithName("SearchProducts")
        .Produces<List<Product>>(StatusCodes.Status200OK);

        //ai search
        group.MapGet("search/ai/{query}", async (string query, ProductAIService service) =>
        {
            var products = await service.SearchProductsAsync(query);
            return Results.Ok(products);
        }).WithName("SearchProductsAI")
        .Produces<List<Product>>(StatusCodes.Status200OK);
    }
}