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
}