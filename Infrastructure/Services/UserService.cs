using System.Net;
using Domain.Constants;
using Domain.DTOs.User;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class UserService(
    UserManager<IdentityUser> userManager,
    RoleManager<IdentityRole> roleManager,
    ILogger<UserService> logger) : IUserService
{
    public async Task<Response<List<UserDto>>> GetAllAsync()
    {
        var users = userManager.Users.ToList();

        var result = users.Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.UserName,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber
        }).ToList();

        return new Response<List<UserDto>>(result);
    }

    public async Task<Response<UserDto>> GetByIdAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
            return new Response<UserDto>(HttpStatusCode.NotFound, "User not found");

        var dto = new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

        return new Response<UserDto>(dto);
    }

    public async Task<Response<string>> CreateAsync(CreateUserDto dto)
    {
        var user = new IdentityUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber
        };

        var result = await userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return new Response<string>(HttpStatusCode.BadRequest, "Failed to create user");

        await userManager.AddToRoleAsync(user, Roles.User); // или другой дефолтный
        return new Response<string>("User created successfully");
    }


    public async Task<Response<string>> UpdateAsync(string id, UpdateUserDto dto)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
            return new Response<string>(HttpStatusCode.NotFound, "User not found");

        user.UserName = dto.UserName;
        user.Email = dto.Email;
        user.PhoneNumber = dto.PhoneNumber;

        var result = await userManager.UpdateAsync(user);
        return result.Succeeded
            ? new Response<string>("User updated successfully")
            : new Response<string>(HttpStatusCode.InternalServerError, "Failed to update user");
    }

    public async Task<Response<string>> DeleteAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
            return new Response<string>(HttpStatusCode.NotFound, "User not found");

        var result = await userManager.DeleteAsync(user);
        return result.Succeeded
            ? new Response<string>("User deleted successfully")
            : new Response<string>(HttpStatusCode.InternalServerError, "Failed to delete user");
    }

    public async Task<Response<string>> ChangePasswordAsync(ChangePasswordDto dto)
    {
        var user = await userManager.FindByIdAsync(dto.UserId);
        if (user == null)
            return new Response<string>(HttpStatusCode.NotFound, "User not found");

        var result = await userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
        return result.Succeeded
            ? new Response<string>("Password changed successfully")
            : new Response<string>(HttpStatusCode.BadRequest, "Invalid password");
    }

    public async Task<Response<string>> AddToRoleAsync(string userId, string role)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return new Response<string>(HttpStatusCode.NotFound, "User not found");

        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));

        var result = await userManager.AddToRoleAsync(user, role);
        return result.Succeeded
            ? new Response<string>($"User added to role {role}")
            : new Response<string>(HttpStatusCode.InternalServerError, "Failed to add to role");
    }

    public async Task<Response<string>> RemoveFromRoleAsync(string userId, string role)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return new Response<string>(HttpStatusCode.NotFound, "User not found");

        var result = await userManager.RemoveFromRoleAsync(user, role);
        return result.Succeeded
            ? new Response<string>($"User removed from role {role}")
            : new Response<string>(HttpStatusCode.InternalServerError, "Failed to remove from role");
    }
}