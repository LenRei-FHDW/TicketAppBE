using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketAPI.Data.Models;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Scoped;

namespace TicketAPI.Controllers;

/// <summary>
/// This controller manages all calls coresponding to articles management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductController(IFileService _fileService, ProductService _productService, UserManager<ApplicationUser> _userManager, ILogger<ProductController> _logger) : ControllerBase
{

    /// <summary>
    /// Lists all articles if called.
    /// </summary>
    /// <returns>A list of all products.</returns>
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<ProductPreviewDTO>>> GetProducts()
    {
        return Ok(await _productService.GetAllProductsAsync());
    }

    /// <summary>
    /// Returns only the article specified.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var product = await _productService.GetProductById(id);
        return Ok(product);
    }
    
    /// <summary>
    /// Adds a new article to the db.
    /// </summary>
    /// <param name="productDTO">Name, description, price and Image of the article.</param>
    /// <returns>A response based on success or failure.</returns>
    [Authorize (Roles = "Seller, Admin")]
    [HttpPost]
    public async Task<IActionResult> AddProduct([FromForm] ProductCreateDTO productDTO)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogTrace("productDTO is invalid: {ModelState}", ModelState.ToString());
            return BadRequest(ModelState);
        } 
        
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            _logger.LogTrace("UserId is null");
            return Unauthorized();
        }
        
        var imageName = string.Empty;
        if (productDTO.ImageFile != null)
        {
            if (!_productService.CheckImageSize(productDTO.ImageFile))
            {
                _logger.LogTrace("Image file size is invalid");
                return BadRequest(); 
            }
                
            
            imageName = await _fileService.SaveFileAsync(productDTO.ImageFile);
        }
        
        var created = await _productService.AddProduct(productDTO, userId, imageName);
        return Created(nameof(AddProduct), created);
    }

    /// <summary>
    /// Edit a new article to the db.
    /// </summary>
    /// <param name="id">ProductId</param>
    /// <param name="productDTO">ProductId, Name, description, price, ImageName and ImageFile of the product.</param>
    /// <returns>A response based on success or failure.</returns>
    [Authorize (Roles = "Seller, Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] ProductEditDTO productDTO)
    {
        if (id != productDTO.ProductId)
        {
            _logger.LogTrace("Id not match with product");
            return BadRequest();
        }
        
        var existingProduct = await _productService.GetProductById(productDTO.ProductId);
        if (existingProduct == null)
        {
            _logger.LogTrace("Product not found");
            return NotFound();
        }
            
        
        if (productDTO.ImageFile != null)
        {
            if (!_productService.CheckImageSize(productDTO.ImageFile))
            {
                _logger.LogTrace("Product not found");
                return BadRequest();
            }
                
            
            var newImageName = await _fileService.SaveFileAsync(productDTO.ImageFile);
            if (string.IsNullOrEmpty(newImageName))
            {
                _logger.LogTrace("New Image Name is empty");
                return BadRequest();
            }
                
            
            if(!string.IsNullOrEmpty(productDTO.ImageName))
                _fileService.DeleteFile(productDTO.ImageName);
            
            productDTO.ImageName = newImageName;
        }
        
        var result = await _productService.EditProduct(productDTO);
        
        return Ok(result);
    }

    /// <summary>
    /// Delete Product with Id
    /// </summary>
    /// <param name="id">ProductId</param>
    /// <returns>A response based on success or failure.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        await _productService.DeleteProduct(id);
        
        return Ok();
    }
}