using Domain.DTOs.User;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IUserService
{
    Task<Response<List<UserDto>>> GetAllAsync();
    Task<Response<UserDto>> GetByIdAsync(string id);
    Task<Response<string>> CreateAsync(CreateUserDto dto);
    Task<Response<string>> UpdateAsync(string id, UpdateUserDto dto);
    Task<Response<string>> DeleteAsync(string id);
    Task<Response<string>> ChangePasswordAsync(ChangePasswordDto dto);
    Task<Response<string>> AddToRoleAsync(string userId, string role);
    Task<Response<string>> RemoveFromRoleAsync(string userId, string role);
}
