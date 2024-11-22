using AutoMapper;
using TicketAPI.Data.Models;
using TicketAPI.Data.Models.DTO;
using TicketAPI.Data.Repositories;

namespace TicketAPI.Services.Scoped;

public interface IShoppingCartService
{
    
}

public class ShoppingCartService(ShoppingCartRepository _shoppingCartRepository, IMapper _mapper)
{
    public async Task<IEnumerable<ShoppingCartItemCreateDTO>> AddCartItem(ShoppingCartItemCreateDTO cartItemDto, string userId)
    {
        ShoppingCartItem shoppingCartItemCreated;
        try
        {
            var existingCartItem = await _shoppingCartRepository.GetShoppingCartItemWhereUserIdAndProductId(userId, cartItemDto.ProductId);
            existingCartItem.Quantity += cartItemDto.Quantity;
            shoppingCartItemCreated = await _shoppingCartRepository.UpdateAsync(existingCartItem);
        }
        catch (KeyNotFoundException)
        {
            var shoppingCartItem = _mapper.Map<ShoppingCartItem>(cartItemDto);
            shoppingCartItemCreated = await _shoppingCartRepository.AddAsync(shoppingCartItem);
        }
        return _mapper.Map<IEnumerable<ShoppingCartItemCreateDTO>>(shoppingCartItemCreated);
    }

    public async Task<IEnumerable<ShoppingCartItemDTO>> GetItemsOfUser(string userId)
    {
        var shoppingCartItems = await _shoppingCartRepository.GetShoppingCartItemWhereUserIdJoinProduct(userId);
        return _mapper.Map<IEnumerable<ShoppingCartItemDTO>>(shoppingCartItems);
    }

    public async Task RemoveItemFromCart(string userId, Guid productId)
    {
        var shoppingCartItem = await _shoppingCartRepository.GetShoppingCartItemWhereUserIdAndProductId(userId, productId);
        await _shoppingCartRepository.DeleteEntityAsync(shoppingCartItem);
    }

    public async void EditCartItem(Guid productId, ShoppingCartItemEditDTO cartItemDto, string userId)
    {
        var cartItem = await _shoppingCartRepository.GetShoppingCartItemWhereUserIdAndProductId(userId, productId);
        cartItem.Quantity = cartItemDto.Quantity;
        await _shoppingCartRepository.UpdateAsync(cartItem);
    }   
}