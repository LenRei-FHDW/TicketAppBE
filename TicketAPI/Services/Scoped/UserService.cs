using TicketAPI.Data.Models;
using TicketAPI.Data.Models.DTO;
using TicketAPI.Data.Repositories;

namespace TicketAPI.Services.Scoped;

public interface IUserService
{
    Task<UserDataResultDTO?> GetUserDataAsync(string userId);
    Task<UserDataEditDTO?> UpdateUserDataAsync(string userId, UserDataEditDTO userDataDTO);
}

/// <summary>
/// Manages UserData.
/// </summary>
public class UserService(IUserRepository _userRepository) : IUserService
{
    /// <summary>
    /// Call the Database and maps the result to UserDataResultDTO
    /// </summary>
    /// <param name="userId">UserId von User</param>
    /// <returns>ApplicationUserId, FirstName, LastName, Street, City, Zip for User</returns>
    public async Task<UserDataResultDTO?> GetUserDataAsync(string userId)
    {
        var user = await _userRepository.GetUserWithAddressAsync(userId);
        if (user == null)
            return null;

        return new UserDataResultDTO
        {
            ApplicationUserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Street = user.Addresse?.StreetLine1 ?? string.Empty,
            City = user.Addresse?.City ?? string.Empty,
            Zip = user.Addresse?.Zip ?? string.Empty,
        };
    }

    /// <summary>
    /// maps UserDataEditDTO to DatabseModels and Update Database
    /// </summary>
    /// <param name="userId">UserId von User</param>
    /// <param name="userDataDTO">ApplicationUserId, FirstName, LastName, Street, City, Zip</param>
    /// <returns>ApplicationUserId, FirstName, LastName, Street, City, Zip for User</returns>
    public async Task<UserDataEditDTO?> UpdateUserDataAsync(string userId, UserDataEditDTO userDataDTO)
    {
        var user = await _userRepository.GetUserWithAddressAsync(userId);
        if (user == null)
            return null;

        user.FirstName = userDataDTO.FirstName;
        user.LastName = userDataDTO.LastName;
        
        if (user.Addresse == null)
        {
            user.Addresse = new Address
            {
                ApplicationUserId = userId,
                StreetLine1 = userDataDTO.Street,
                City = userDataDTO.City,
                Zip = userDataDTO.Zip
            };
        }
        else
        {
            user.Addresse.StreetLine1 = userDataDTO.Street;
            user.Addresse.City = userDataDTO.City;
            user.Addresse.Zip = userDataDTO.Zip;
        }

        await _userRepository.UpdateAsync(user);
        return userDataDTO;
    }
}