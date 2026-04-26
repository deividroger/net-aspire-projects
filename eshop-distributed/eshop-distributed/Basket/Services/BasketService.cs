using Basket.ApiClients;
using Basket.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.Services;

public class BasketService (IDistributedCache cache, CatalogApiClient catalogApiClient)
{
    public async Task<ShoppingCart?> GetBasket(string userName)
    {
        var basket = await cache.GetStringAsync(userName);

        return string.IsNullOrEmpty(basket) ? null : 
            JsonSerializer.Deserialize<ShoppingCart>(basket);
    }
    public async Task<ShoppingCart?> UpdateBasket(ShoppingCart shoppingCart)
    {
        foreach (var item in shoppingCart.Items)
        {
            var product = await catalogApiClient.GetProductByIdAsync(item.ProductId);
            item.ProductName = product.Name;
            item.Price = product.Price;
        }

        await cache.SetStringAsync(shoppingCart.UserName,JsonSerializer.Serialize(shoppingCart));
        return await GetBasket(shoppingCart.UserName);
    }
    public async Task DeleteBasket(string userName)
    {
        await cache.RemoveAsync(userName);
    }

    public async Task UpdateBasketItemProductsPrices(int productId, decimal price)
    {
        var basket = await GetBasket("johndoe");
        var item = basket?.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item is not null)
        {
            item.Price = price;
            await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket));
        }
    }
}
