using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TicketAPI.Data.Models;
using TicketAPI.Data.Repositories;
using TicketAPI.Services.DTO;

namespace TicketAPI.Services.Scoped;

public class ShoppingCartService(ShoppingCartRepository shoppingCartRepository, IMapper mapper)
{
    public async Task<IEnumerable<ShoppingCartItemCreateDTO>> AddCartItem(ShoppingCartItemCreateDTO cartItemDto, string userId)
    {
        ShoppingCartItem shoppingCartItemCreated;
        try
        {
            var existingCartItem = await shoppingCartRepository.GetShoppingCartItemWhereUserIdAndProductId(userId, cartItemDto.ProductId);
            existingCartItem.Quantity += cartItemDto.Quantity;
            shoppingCartItemCreated = await shoppingCartRepository.UpdateAsync(existingCartItem);
        }
        catch (KeyNotFoundException)
        {
            var shoppingCartItem = mapper.Map<ShoppingCartItem>(cartItemDto);
            shoppingCartItemCreated = await shoppingCartRepository.AddAsync(shoppingCartItem);
        }
        return mapper.Map<IEnumerable<ShoppingCartItemCreateDTO>>(shoppingCartItemCreated);
    }

    public async Task<IEnumerable<ShoppingCartItemDTO>> GetItemsOfUser(string userId)
    {
        var shoppingCartItems = await shoppingCartRepository.GetShoppingCartItemWhereUserIdJoinProduct(userId);
        return mapper.Map<IEnumerable<ShoppingCartItemDTO>>(shoppingCartItems);
    }

    public async Task RemoveItemFromCart(string userId, Guid productId)
    {
        var shoppingCartItem = await shoppingCartRepository.GetShoppingCartItemWhereUserIdAndProductId(userId, productId);
        await shoppingCartRepository.DeleteEntityAsync(shoppingCartItem);
    }

    public async void EditCartItem(Guid productId, ShoppingCartItemEditDTO cartItemDto, string userId)
    {
        var cartItem = await shoppingCartRepository.GetShoppingCartItemWhereUserIdAndProductId(userId, productId);
        cartItem.Quantity = cartItemDto.Quantity;
        await shoppingCartRepository.UpdateAsync(cartItem);
    }
}