using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsAPI.Data;
using ProductsAPI.Models;
using Shared.Models.Messages;

namespace ProductsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController(ProductsApiContext context, IPublishEndpoint publishEndpoint) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts(CancellationToken cancellationToken)
        => Ok(await context.Products.AsNoTracking().ToListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id, CancellationToken cancellationToken)
    {
        var product = await context.Products.FindAsync([id], cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> PostProduct(Product product, CancellationToken cancellationToken)
    {
        context.Products.Add(product);
        await context.SaveChangesAsync(cancellationToken);
        await publishEndpoint.Publish(new ProductCreated(product.ProductId, product.Name), cancellationToken);
        return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutProduct(int id, Product product, CancellationToken cancellationToken)
    {
        if (id != product.ProductId)
        {
            return BadRequest("The route id and product id must match.");
        }

        if (!await context.Products.AnyAsync(item => item.ProductId == id, cancellationToken))
        {
            return NotFound();
        }

        context.Entry(product).State = EntityState.Modified;
        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id, CancellationToken cancellationToken)
    {
        var product = await context.Products.FindAsync([id], cancellationToken);
        if (product is null)
        {
            return NotFound();
        }

        context.Products.Remove(product);
        await context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
