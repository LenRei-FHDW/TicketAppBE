using AutoMapper;
using NuGet.Protocol.Core.Types;
using TicketAPI.Data;
using TicketAPI.Data.Models;
using TicketAPI.Data.Repositories;
using TicketAPI.Services.DTO;

namespace TicketAPI.Services.Scoped;

/// <summary>
/// Manages product interaction.
/// </summary>
public class ProductService
{
    private readonly IRepository<Product, Guid> _repository;
    private readonly IMapper _mapper;

    public ProductService(IRepository<Product, Guid> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <summary>
    /// Collects all products and returns them.
    /// </summary>
    /// <returns>A List with all products (id, name, price)</returns>
    public async Task<IEnumerable<ProductPreview>> GetAllProductsAsync()
    {
        var productList = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductPreview>>(productList);
    }

    /// <summary>
    /// Finds a specific product and returns it.
    /// </summary>
    /// <param name="id">Id of the wanted product.</param>
    /// <returns>The specified product.</returns>
    public async Task<Product?> GetArticleById(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    /// <summary>
    /// Adds a new product.
    /// </summary>
    /// <param name="product">The product to be added.</param>
    /// <returns>The newly added product.</returns>
    public async Task<ProductDTO> AddProduct(ProductPostDTO product)
    {
        var productEntity = _mapper.Map<Product>(product);
        var result =  await _repository.AddAsync(productEntity);
        
        return _mapper.Map<ProductDTO>(result);
    }
}