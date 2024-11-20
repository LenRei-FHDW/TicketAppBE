using TicketAPI.Data.Models;
using TicketAPI.Data.Models.DTO;
using TicketAPI.Data.Repositories;

namespace TicketAPI.Services.Scoped;

interface IUserService
{
    Task<UserDataResultDTO?> GetUserDataAsync(string userId);
    Task<UserDataEditDTO?> UpdateUserDataAsync(string userId, UserDataEditDTO userDataDTO);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
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
            Street = user.Addresse?.Street ?? string.Empty,
            City = user.Addresse?.City ?? string.Empty,
            Zip = user.Addresse?.Zip ?? string.Empty,
        };
    }

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
                Street = userDataDTO.Street,
                City = userDataDTO.City,
                Zip = userDataDTO.Zip
            };
        }
        else
        {
            user.Addresse.Street = userDataDTO.Street;
            user.Addresse.City = userDataDTO.City;
            user.Addresse.Zip = userDataDTO.Zip;
        }

        await _userRepository.UpdateUserAsync(user);
        return userDataDTO;
    }
}