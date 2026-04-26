using MassTransit;
using ServiceDefaults.Messaging.Events;

namespace catalog.Services;

public class ProductService(ProductDbContext dbContext, IBus bus)
{
    public async Task CreateProductAsync(Product product)
    {
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        return await dbContext.Products.ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await dbContext.Products.FindAsync(id);
    }

    public async Task UpdateProductAsync(Product product)
    {
        var existingProduct = await dbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == product.Id);

        //lacks outbox pattern, but for the sake of simplicity, we will publish the event directly here
        //just for learning purposes, in production, we should use the outbox pattern to ensure that the event is published only if the database transaction is successful
        if (existingProduct?.Price != product.Price)
        {
            var integrationEvent = new ProductPriceChangedIntegrationEvent
            {
                ProductId = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl
            };
            await bus.Publish(integrationEvent);
        }

        dbContext.Products.Update(product);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await dbContext.Products.FindAsync(id);
        if (product is null) return;
        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync();
    }
}
