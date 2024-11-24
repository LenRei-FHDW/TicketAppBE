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
public class ProductController(
        IFileService fileService,
        ProductService productService, 
        UserManager<ApplicationUser> userManager, 
        ILogger<ProductController> logger
    ) : ControllerBase
{

    /// <summary>
    /// Lists all articles if called.
    /// </summary>
    /// <returns>A list of all products.</returns>
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<ProductPreviewDTO>>> GetProducts()
    {
        return Ok(await productService.GetAllProductsAsync());
    }

    /// <summary>
    /// Returns only the article specified.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDTO>> GetProductById(Guid id)
    {
        var product = await productService.GetProductById(id);
        return Ok(product);
    }
    
    /// <summary>
    /// Adds a new article to the db.
    /// </summary>
    /// <param name="productDTO">Name, description, price and Image of the article.</param>
    /// <returns>A response based on success or failure.</returns>
    
    [HttpPost]
    [Authorize (Roles = "Admin")]
    public async Task<IActionResult> AddProduct([FromForm] ProductCreateDTO productDTO)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("productDTO is invalid: {ModelState}", ModelState.ToString());
            return BadRequest(ModelState);
        } 
        
        var userId = userManager.GetUserId(User);
        if (userId == null)
        {
            logger.LogWarning("UserId is null");
            return Unauthorized();
        }
        
        var imageName = string.Empty;
        if (productDTO.ImageFile != null)
        {
            if (!productService.CheckImageSize(productDTO.ImageFile))
            {
                logger.LogWarning("Image file size is invalid");
                return BadRequest(); 
            }
                
            
            imageName = await fileService.SaveFileAsync(productDTO.ImageFile);
        }
        
        var created = await productService.AddProduct(productDTO, userId, imageName);
        return Created(nameof(AddProduct), created);
    }

    /// <summary>
    /// Edit a new article to the db.
    /// </summary>
    /// <param name="id">ProductId</param>
    /// <param name="productDTO">ProductId, Name, description, price, ImageName and ImageFile of the product.</param>
    /// <returns>A response based on success or failure.</returns>
    [HttpPut("{id}")]
    [Authorize (Roles = "Admin")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] ProductEditDTO productDTO)
    {
        if (id != productDTO.ProductId)
        {
            logger.LogWarning("Id not match with product");
            return BadRequest();
        }
        
        var existingProduct = await productService.GetProductById(productDTO.ProductId);
        if (existingProduct == null)
        {
            logger.LogWarning("Product not found with ID: {productID}", productDTO.ProductId);
            return NotFound();
        }
            
        
        if (productDTO.ImageFile != null)
        {
            if (!productService.CheckImageSize(productDTO.ImageFile))
            {
                logger.LogWarning("Image file size is invalid\"");
                return BadRequest();
            }
                
            
            var newImageName = await fileService.SaveFileAsync(productDTO.ImageFile);
            if (string.IsNullOrEmpty(newImageName))
            {
                logger.LogWarning("New Image Name is empty");
                return BadRequest();
            }
                
            
            if(!string.IsNullOrEmpty(productDTO.ImageName))
                fileService.DeleteFile(productDTO.ImageName);
            
            productDTO.ImageName = newImageName;
        }
        
        var result = await productService.EditProduct(productDTO);
        
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
        await productService.DeleteProduct(id);
        
        return Ok();
    }
}