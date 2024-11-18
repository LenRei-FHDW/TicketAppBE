using AutoMapper;
using NuGet.Protocol.Core.Types;
using TicketAPI.Data;
using TicketAPI.Data.Models;
using TicketAPI.Data.Repositories;
using TicketAPI.Services.DTO;

namespace TicketAPI.Services.Scoped;

public class ProductService(IRepository<Product, Guid> repository, IMapper mapper)
{
    public async Task<IEnumerable<ProductPreview>> GetAllProductsAsync()
    {
        var productList = await repository.GetAllAsync();
        return mapper.Map<IEnumerable<ProductPreview>>(productList);
    }

    public async Task<Product?> GetArticleById(Guid id)
    {
        return await repository.GetByIdAsync(id);
    }

    public async Task<ProductDTO> AddProduct(ProductPostDTO product)
    {
        var productEntity = mapper.Map<Product>(product);
        var result =  await repository.AddAsync(productEntity);
        
        return mapper.Map<ProductDTO>(result);
    }
}