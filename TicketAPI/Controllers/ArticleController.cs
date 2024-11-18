using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Scoped;

namespace TicketAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticleController : ControllerBase
{
    private readonly ProductService _productService;

    public ArticleController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("listArticles")]
    public async Task<ActionResult<IEnumerable<ProductPreviewDTO>>> GetArticles()
    {
        return Ok(await _productService.GetAllProductsAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetArticleById(Guid id)
    {
        var product = await _productService.GetArticleById(id);
        return Ok(product);
    }
    
    [Authorize (Roles = "Admin, Seller")]
    [HttpPost("add")]
    public async Task<IActionResult> AddArticle([FromBody] ProductPostDTO product)
    {
        var userId = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var created = await _productService.AddProduct(userId, product);
        UriBuilder uriBuilder = new UriBuilder($"https://localhost:44378/api/Article/{created.ProductId}");
        return Created(uriBuilder.Uri, product);
    }
}