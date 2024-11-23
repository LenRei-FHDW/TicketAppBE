using System.Collections;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using TicketAPI.Data.Models;
using TicketAPI.Data.Repositories;
using TicketAPI.Services.DTO;

namespace TicketAPI.Services.Scoped;

public class CategoryService(IRepository<Category, Guid> categoryRepository, IMapper mapper, IProductRepository productRepository)
{
    public async Task<IEnumerable<CategoryDTO>> GetAllCategories()
    {
        var categories = await categoryRepository.GetAllAsync();
        return mapper.Map<IEnumerable<CategoryDTO>>(categories);
    }

    public async Task<IEnumerable<ProductPreviewDTO>> GetProductsOfCategory(Guid categoryId)
    {
        var products = await productRepository.GetProductsWhereCategoryIdAsync(categoryId);
        return mapper.Map<IEnumerable<ProductPreviewDTO>>(products);
    }

    public async Task CreateCategory(CategoryCreateDTO category)
    {
            await categoryRepository.AddAsync(mapper.Map<Category>(category));
    }

    public async Task DeleteCategory(Guid id)
    {
        await categoryRepository.DeleteAsync(id);
    }
}