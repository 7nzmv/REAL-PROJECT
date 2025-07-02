using Domain.Constants;
using Domain.DTOs.User;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = Roles.Admin)]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await userService.GetAllAsync();
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var response = await userService.GetByIdAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        var response = await userService.CreateAsync(dto);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto dto)
    {
        var response = await userService.UpdateAsync(id, dto);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var response = await userService.DeleteAsync(id);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var response = await userService.ChangePasswordAsync(dto);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPost("{userId}/add-role/{role}")]
    public async Task<IActionResult> AddToRole(string userId, string role)
    {
        var response = await userService.AddToRoleAsync(userId, role);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpDelete("{userId}/remove-role/{role}")]
    public async Task<IActionResult> RemoveFromRole(string userId, string role)
    {
        var response = await userService.RemoveFromRoleAsync(userId, role);
        return StatusCode((int)response.StatusCode, response);
    }
}
