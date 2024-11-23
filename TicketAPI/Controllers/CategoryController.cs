using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketAPI.Controllers.Helper;
using TicketAPI.Data.Models;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Scoped;

namespace TicketAPI.Controllers;

public class CategoryController(CategoryService categoryService, UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet("api/[controller]")]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAllCategories()
    {
        var categories = await categoryService.GetAllCategories();
        return Ok(categories);
    }

    [HttpGet("api/[controller]/{id}")]
    public async Task<OkObjectResult> GetProductsOfCategory(Guid id)
    {
        return Ok(await categoryService.GetProductsOfCategory(id));
    }

    [ValidateModel]
    [Authorize(Roles = "Admin")]
    [HttpPost("api/[controller]")]
    public async Task<ActionResult> CreateCategory(
        [FromBody] CategoryCreateDTO category)
    {
        await categoryService.CreateCategory(category);
        return Created();
    }

    [HttpDelete("api/[controller]/{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        await categoryService.DeleteCategory(id);
        return NoContent();
    }
}