using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketAPI.Controllers.Helper;
using TicketAPI.Data.Models;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Scoped;

namespace TicketAPI.Controllers;

/// <summary>
/// Controller for managing categories and their associated products.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CategoryController(
    CategoryService categoryService,
    UserManager<ApplicationUser> userManager)
    : ControllerBase
{

    /// <summary>
    /// Retrieves all categories.
    /// </summary>
    /// <returns>
    /// An <see cref="ActionResult"/> containing a collection of <see cref="CategoryDTO"/> objects.
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetAllCategories()
    {
        var categories = await categoryService.GetAllCategories();
        return Ok(categories);
    }

    /// <summary>
    /// Retrieves all products associated with a specific category.
    /// </summary>
    /// <param name="id">The unique identifier of the category.</param>
    /// <returns>
    /// An <see cref="ActionResult"/> containing a collection of <see cref="ProductPreviewDTO"/> objects.
    /// </returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<IEnumerable<ProductPreviewDTO>>> GetProductsOfCategory(Guid id)
    {
        return Ok(await categoryService.GetProductsOfCategory(id));
    }

    /// <summary>
    /// Creates a new category.
    /// </summary>
    /// <param name="category">The <see cref="CategoryCreateDTO"/> containing the details of the category to create.</param>
    /// <returns>
    /// An <see cref="ActionResult"/> indicating that the category has been created.
    /// </returns>
    [ValidateModel]
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDTO category)
    {
        await categoryService.CreateCategory(category);
        return Created();
    }

    /// <summary>
    /// Deletes a category.
    /// </summary>
    /// <param name="id">The unique identifier of the category to delete.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> indicating that the category has been deleted.
    /// </returns>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        await categoryService.DeleteCategory(id);
        return NoContent();
    }
}
