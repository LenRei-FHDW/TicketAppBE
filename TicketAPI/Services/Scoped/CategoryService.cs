using System.Collections;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using TicketAPI.Data.Models;
using TicketAPI.Data.Repositories;
using TicketAPI.Services.DTO;

namespace TicketAPI.Services.Scoped;
public class CategoryService(
    IRepository<Category, Guid> categoryRepository,
    IMapper mapper,
    IProductRepository productRepository)
{
    /// <summary>
    /// Retrieves all categories from the repository.
    /// </summary>
    /// <returns>A collection of <see cref="CategoryDTO"/> representing all categories.</returns>
    public async Task<IEnumerable<CategoryDTO>> GetAllCategories()
    {
        var categories = await categoryRepository.GetAllAsync();
        return mapper.Map<IEnumerable<CategoryDTO>>(categories);
    }

    /// <summary>
    /// Retrieves all products associated with a specific category.
    /// </summary>
    /// <param name="categoryId">The unique identifier of the category.</param>
    /// <returns>A collection of <see cref="ProductPreviewDTO"/> representing the products of the category.</returns>
    public async Task<IEnumerable<ProductPreviewDTO>> GetProductsOfCategory(Guid categoryId)
    {
        var products = await productRepository.GetProductsWhereCategoryIdAsync(categoryId);
        return mapper.Map<IEnumerable<ProductPreviewDTO>>(products);
    }

    /// <summary>
    /// Creates a new category in the repository.
    /// </summary>
    /// <param name="category">The <see cref="CategoryCreateDTO"/> containing details of the category to create.</param>
    public async Task CreateCategory(CategoryCreateDTO category)
    {
        await categoryRepository.AddAsync(mapper.Map<Category>(category));
    }

    /// <summary>
    /// Deletes a category from the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the category to delete.</param>
    public async Task DeleteCategory(Guid id)
    {
        await categoryRepository.DeleteAsync(id);
    }
}
