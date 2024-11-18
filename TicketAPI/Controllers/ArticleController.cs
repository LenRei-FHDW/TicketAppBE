using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketAPI.Data;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Scoped;

namespace TicketAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticleController : ControllerBase
{
    private readonly ProductService _productService;
    private readonly TicketApiDbContext _context;

    public ArticleController(ProductService productService, TicketApiDbContext context)
    {
        _productService = productService;
        _context = context;
    }

    [HttpGet("listArticles")]
    public async Task<ActionResult<IEnumerable<ProductPreview>>> GetArticles()
    {
        return Ok(await _productService.GetAllProductsAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetArticleById(Guid id)
    {
        var product = await _productService.GetArticleById(id);
        return Ok(product);
    }
    
    [Authorize (Roles = "Admin")]
    [HttpPost("add")]
    public async Task<IActionResult> AddArticle([FromBody] ProductPostDTO product)
    {
        var created = await _productService.AddProduct(product);
        UriBuilder uriBuilder = new UriBuilder($"http://localhost:5000/api/Article/{created.ProductId}");
        return Created(uriBuilder.Uri, product);
    }
    
    // PUT: api/Product/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    //[Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> PutProduct(Guid id, [FromBody]ProductEditDTO productDTO)
    {
        if (id != productDTO.ProductId)
        {
            return BadRequest();
        }
            
        var editProduct = await _context.Products.FindAsync(id);
            
        editProduct.Name = productDTO.Name;
        editProduct.Description = productDTO.Description;
        editProduct.Price = productDTO.Price;

        _context.Entry(editProduct).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }
    
    // DELETE: api/Product/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Seller,Admin")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
            
        product.IsDeleted = true;
            
        _context.Entry(product).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }
    
    private bool ProductExists(Guid id)
    {
        return _context.Products.Any(e => e.ProductId == id);
    }
}