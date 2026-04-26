using Basket.Services;
using MassTransit;
using ServiceDefaults.Messaging.Events;

namespace Basket.Handlers;

public class ProductPriceChangedIntegrationEventHandler(BasketService service) : IConsumer<ProductPriceChangedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductPriceChangedIntegrationEvent> context)
    {
        await service.UpdateBasketItemProductsPrices(context.Message.ProductId,
            context.Message.Price);

    }
}
