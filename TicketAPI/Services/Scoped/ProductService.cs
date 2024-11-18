using AutoMapper;
using NuGet.Protocol.Core.Types;
using TicketAPI.Data;
using TicketAPI.Data.Models;
using TicketAPI.Data.Repositories;
using TicketAPI.Services.DTO;

namespace TicketAPI.Services.Scoped;

public class ProductService(ProductRepository productRepository, IMapper mapper)
{
    public async Task<IEnumerable<ProductPreviewDTO>> GetAllProductsAsync()
    {
        var productList = await productRepository.GetAllWhereNotDeletedAsync();
        return mapper.Map<IEnumerable<ProductPreviewDTO>>(productList);
    }

    public async Task<Product?> GetArticleById(Guid id)
    {
        return await productRepository.GetByIdAsync(id);
    }

    public async Task<ProductDTO> AddProduct(string userId, ProductPostDTO product)
    {
        var productEntity = mapper.Map<Product>(product);
        productEntity.ApplicationUserId = userId;
        var result =  await productRepository.AddAsync(productEntity);
        return mapper.Map<ProductDTO>(result);
    }
}