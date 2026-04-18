namespace catalog.Data;

public class DataSeeder
{
    public static void Seed(ProductDbContext context)
    {
        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product
                {
                    Name = "Product 1",
                    Description = "Description for product 1",
                    Price = 9.99m,
                    ImageUrl = "https://via.placeholder.com/150"
                },
                new Product
                {
                    Name = "Product 2",
                    Description = "Description for product 2",
                    Price = 19.99m,
                    ImageUrl = "https://via.placeholder.com/150"
                },
                new Product
                {
                    Name = "Product 3",
                    Description = "Description for product 3",
                    Price = 29.99m,
                    ImageUrl = "https://via.placeholder.com/150"
                }
            );
            context.SaveChanges();
        }
    }
}