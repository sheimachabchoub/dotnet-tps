using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdersAPI.Data;
using OrdersAPI.Models;

namespace OrdersAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class OrdersController(OrdersApiContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders(CancellationToken cancellationToken)
        => Ok(await context.Orders.AsNoTracking().ToListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Order>> GetOrder(int id, CancellationToken cancellationToken)
    {
        var order = await context.Orders.FindAsync([id], cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<Order>> PostOrder(Order order, CancellationToken cancellationToken)
    {
        if (!await context.Products.AnyAsync(product => product.Id == order.ProductId, cancellationToken))
        {
            return BadRequest("The product is not synchronized in OrdersAPI yet.");
        }

        context.Orders.Add(order);
        await context.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetOrder), new { id = order.OrdersId }, order);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutOrder(int id, Order order, CancellationToken cancellationToken)
    {
        if (id != order.OrdersId)
        {
            return BadRequest("The route id and order id must match.");
        }

        if (!await context.Orders.AnyAsync(item => item.OrdersId == id, cancellationToken))
        {
            return NotFound();
        }

        context.Entry(order).State = EntityState.Modified;
        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
