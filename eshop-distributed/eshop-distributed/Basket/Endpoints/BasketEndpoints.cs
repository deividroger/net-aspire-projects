using Basket.Models;

namespace Basket.Endpoints;

public static class BasketEndpoints
{
    public static void MapBasketEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("basket");
        group.MapGet("/{userName}", async (string userName, Services.BasketService basketService) =>
        {
            var basket = await basketService.GetBasket(userName);
            return basket is not null ? Results.Ok(basket) : Results.NotFound();
        }).WithName("GetBasket")
        .Produces<ShoppingCart>(StatusCodes.Status200OK);

        group.MapPost("/", async (ShoppingCart shoppingCart, Services.BasketService basketService) =>
        {
            var updatedBasket = await basketService.UpdateBasket(shoppingCart);
            return Results.Ok(updatedBasket);
        }).WithName("UpdateBasket");

        group.MapDelete("/{userName}", async (string userName, Services.BasketService basketService) =>
        {
            await basketService.DeleteBasket(userName);
            return Results.NoContent();
        }).WithName("DeleteBasket");
    }
}
