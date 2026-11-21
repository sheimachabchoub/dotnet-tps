using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrdersAPI.Data;
using OrdersAPI.Models;
using Shared.Models.Messages;

namespace OrdersAPI.Consumers;

public sealed class ProductCreatedConsumer(OrdersApiContext context) : IConsumer<ProductCreated>
{
    public async Task Consume(ConsumeContext<ProductCreated> messageContext)
    {
        var message = messageContext.Message;
        var product = await context.Products.FirstOrDefaultAsync(item => item.Id == message.Id);

        if (product is null)
        {
            context.Products.Add(new Product { Id = message.Id, Name = message.Name });
        }
        else
        {
            product.Name = message.Name;
        }

        await context.SaveChangesAsync();
    }
}
