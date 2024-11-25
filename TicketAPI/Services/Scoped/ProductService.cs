using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NuGet.Protocol.Core.Types;
using TicketAPI.Data;
using TicketAPI.Data.Models;
using TicketAPI.Data.Repositories;
using TicketAPI.Services.DTO;

namespace TicketAPI.Services.Scoped;

/// <summary>
/// Manages product interaction.
/// </summary>
public class ProductService(
        IRepository<Product, Guid> repository, 
        IProductRepository productRepository, 
        IMapper mapper, 
        ILogger<ProductService> _logger
    )
{
    /// <summary>
    /// Collects all products and returns them.
    /// </summary>
    /// <returns>A List with all products (id, name, price)</returns>
    public async Task<IEnumerable<ProductPreviewDTO>> GetAllProductsAsync()
    {
        var productList = await productRepository.GetAllAsync();
        return mapper.Map<IEnumerable<ProductPreviewDTO>>(productList);
    }

    /// <summary>
    /// Finds a specific product and returns it.
    /// </summary>
    /// <param name="id">Id of the wanted product.</param>
    /// <returns>The specified product.</returns>
    public async Task<ProductDTO> GetProductById(Guid id)
    {
        var result = await repository.GetByIdAsync(id);
        return mapper.Map<ProductDTO>(result);    
    }

    /// <summary>
    /// Adds a new product.
    /// </summary>
    /// <param name="productDTO">The productDTO to be added.</param>
    /// <param name="userId">UserId from User</param>
    /// <param name="ImageName">Name of Image</param>
    /// <returns>The newly added product.</returns>
    public async Task<ProductDTO> AddProduct(ProductCreateDTO productDTO, string userId, string? ImageName)
    {
        var productEntity = new Product
        {
            Name = productDTO.Name,
            Description = productDTO.Description,
            Price = productDTO.Price,
            ImageName = ImageName,
            CreaterId = userId,
            CategoryId = productDTO.CategoryId,
        };
        
        //var productEntity = _mapper.Map<Product>(productDTO);
        var result =  await repository.AddAsync(productEntity);
        
        return mapper.Map<ProductDTO>(result);
    }

    /// <summary>
    /// Update product.
    /// </summary>
    /// <param name="productDTO">The productDTO to be update.</param>
    /// <returns>The updates ProductDTO</returns>
    public async Task<ProductDTO?> EditProduct(ProductEditDTO productDTO)
    {
        var produkt = await repository.GetByIdAsync(productDTO.ProductId);
        
        produkt.Name = productDTO.Name;
        produkt.Description = productDTO.Description;
        produkt.Price = productDTO.Price;
        produkt.ImageName = productDTO.ImageName;
        produkt.CategoryId = productDTO.CategoryId;
        
        var result = await repository.UpdateAsync(produkt);
        
        return mapper.Map<ProductDTO>(produkt);
    }

    /// <summary>
    /// Set DeleteValue in Produkt
    /// </summary>
    /// <param name="productId">Id of the Product</param>
    public async Task DeleteProduct(Guid productId)
    {
        var existingProduct = await repository.GetByIdAsync(productId);
        
        existingProduct.IsDeleted = true;
        
        await repository.UpdateAsync(existingProduct);
    }

    /// <summary>
    /// Check Image Size
    /// </summary>
    /// <param name="file">Image from Product</param>
    /// <returns>A response based on success or failure.</returns>
    public bool CheckImageSize(IFormFile? file)
    {
        if (file != null && file.Length > 1 * 1024 * 1024)
            return false;
        
        return true;
    }
}