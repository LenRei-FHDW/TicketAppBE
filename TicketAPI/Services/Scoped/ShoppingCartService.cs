using AutoMapper;
using TicketAPI.Data.Models;
using TicketAPI.Data.Models.DTO;
using TicketAPI.Data.Repositories;

namespace TicketAPI.Services.Scoped;

public interface IShoppingCartService
{
    Task<ShoppingCartItemCreateDTO> AddCartItem(ShoppingCartItemCreateDTO cartItemDto, string userId);
    Task<IEnumerable<ShoppingCartItemDTO>> GetItemsOfUser(string userId);
    Task<IEnumerable<ShoppingCartItem>> GetModelItemsOfUser(string userId);
    Task RemoveItemFromCart(string userId, Guid productId);
    Task RemoveAllFromCart(string userId);
    Task<ShoppingCartItem> EditCartItem(Guid productId, ShoppingCartItemEditDTO cartItemDto, string userId);
}

public class ShoppingCartService(ShoppingCartRepository _shoppingCartRepository, IMapper _mapper) : IShoppingCartService
{
    /// <summary>
    /// Add Item to ShoppingCart
    /// </summary>
    /// <param name="cartItemDto">Item to add</param>
    /// <param name="userId">Id of User</param>
    /// <returns>Create ShoppingCartItemCreateDTO</returns>
    public async Task<ShoppingCartItemCreateDTO> AddCartItem(ShoppingCartItemCreateDTO cartItemDto, string userId)
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
            shoppingCartItem.ApplicationUserId = userId;
            shoppingCartItemCreated = await _shoppingCartRepository.AddAsync(shoppingCartItem);
        }
        return _mapper.Map<ShoppingCartItemCreateDTO>(shoppingCartItemCreated);
    }

    /// <summary>
    /// Get IEnumerable ShoppingCartItem of User
    /// </summary>
    /// <param name="userId">Id from User</param>
    /// <returns>IEnumerable ShoppingCartItem of User</returns>
    public async Task<IEnumerable<ShoppingCartItemDTO>> GetItemsOfUser(string userId)
    {
        var shoppingCartItems = await _shoppingCartRepository.GetShoppingCartItemWhereUserIdJoinProduct(userId);
        return _mapper.Map<IEnumerable<ShoppingCartItemDTO>>(shoppingCartItems);
    }
    
    /// <summary>
    /// Get List ShoppingCartItem of User
    /// </summary>
    /// <param name="userId">Id from User</param>
    /// <returns>IEnumerable ShoppingCartItem of User</returns>
    public async Task<IEnumerable<ShoppingCartItem>> GetModelItemsOfUser(string userId)
    {
        return await _shoppingCartRepository.GetShoppingCartItemWhereUserIdJoinProduct(userId);
    }

    /// <summary>
    /// Remove ShoppingCartItem from User
    /// </summary>
    /// <param name="userId">Id of User</param>
    /// <param name="productId">Id of Product</param>
    public async Task RemoveItemFromCart(string userId, Guid productId)
    {
        var shoppingCartItem = await _shoppingCartRepository.GetShoppingCartItemWhereUserIdAndProductId(userId, productId);
        await _shoppingCartRepository.DeleteEntityAsync(shoppingCartItem);
    }
    
    public async Task RemoveAllFromCart(string userId)
    {
        await _shoppingCartRepository.RemoveShoppingCartItemWhereUserId(userId);
    }

    /// <summary>
    /// Edit ShoppingCartItem of User
    /// </summary>
    /// <param name="productId">Id of Product</param>
    /// <param name="cartItemDto">Item to edit</param>
    /// <param name="userId">Id of User</param>
    /// <returns>Updatet ShoppingCartItem</returns>
    public async Task<ShoppingCartItem> EditCartItem(Guid productId, ShoppingCartItemEditDTO cartItemDto, string userId)
    {
        var cartItem = await _shoppingCartRepository.GetShoppingCartItemWhereUserIdAndProductId(userId, productId);
        cartItem.Quantity = cartItemDto.Quantity;
        await _shoppingCartRepository.UpdateAsync(cartItem);
        return cartItem;
    }   
}