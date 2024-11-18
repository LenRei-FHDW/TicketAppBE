using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Scoped;

namespace TicketAPI.Controllers;

/// <summary>
/// This controller manages all calls coresponding to articles.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ArticleController : ControllerBase
{
    private readonly ProductService _productService;

    public ArticleController(ProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Lists all articles if called.
    /// </summary>
    /// <returns>A list of all products.</returns>
    [HttpGet("listArticles")]
    public async Task<ActionResult<IEnumerable<ProductPreview>>> GetArticles()
    {
        return Ok(await _productService.GetAllProductsAsync());
    }

    /// <summary>
    /// Returns only the article specified.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetArticleById(Guid id)
    {
        var product = await _productService.GetArticleById(id);
        return Ok(product);
    }
    
    /// <summary>
    /// Adds a new article to the db.
    /// </summary>
    /// <param name="product">Name, description, price and rating of the article.</param>
    /// <returns>A response based on success or failure.</returns>
    [Authorize (Roles = "Admin")]
    [HttpPost("add")]
    public async Task<IActionResult> AddArticle([FromBody] ProductPostDTO product)
    {
        var created = await _productService.AddProduct(product);
        UriBuilder uriBuilder = new UriBuilder($"http://localhost:5000/api/Article/{created.ProductId}");
        return Created(uriBuilder.Uri, product);
    }
}