using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Domain.Constants;
using Domain.DTOs.Auth;
using Domain.DTOs.Email;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class AuthService(
    UserManager<IdentityUser> userManager,
    IConfiguration config,
    IEmailService emailService) : IAuthService
{
    public async Task<Response<TokenDto>> Login(LoginDto loginDto)
    {
        var user = await userManager.FindByNameAsync(loginDto.Email);
        if (user == null)
        {
            return new Response<TokenDto>(HttpStatusCode.BadRequest, "Username or password is incorrect");
        }

        var checkPassword = await userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!checkPassword)
        {
            return new Response<TokenDto>(HttpStatusCode.BadRequest, "Username or password is incorrect");
        }

        var token = await GenerateJwt(user);
        return new Response<TokenDto>(new TokenDto { Token = token });
    }

    public async Task<Response<string>> Register(RegisterDto registerDto)
    {
        var existingUserName = await userManager.FindByNameAsync(registerDto.UserName);
        if (existingUserName != null)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "Username is already taken");
        }

        var existingUser = await userManager.FindByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "Email is already registered");
        }

        var user = new IdentityUser
        {
            UserName = registerDto.UserName,
            PhoneNumber = registerDto.PhoneNumber,
            Email = registerDto.Email
        };


        var result = await userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            return new Response<string>(HttpStatusCode.InternalServerError, "Failed to create user");
        }

        await userManager.AddToRoleAsync(user, Roles.User);
        return new Response<string>("User created");

    }

    public async Task<Response<string>> RequestResetPassword(RequestResetPassword requestResetPassword)
    {
        var user = await userManager.FindByNameAsync(requestResetPassword.Email);
        if (user == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        var emailDto = new EmailDto()
        {
            To = requestResetPassword.Email,
            Subject = "Reset Password",
            Body = $"Your token is {token}",
        };

        var result = await emailService.SendEmailAsync(emailDto);

        return !result
            ? new Response<string>(HttpStatusCode.InternalServerError, "Failed to send email")
            : new Response<string>("Token sent successfully to email");
    }

    public async Task<Response<string>> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        var user = await userManager.FindByNameAsync(resetPasswordDto.Email);
        if (user == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        var resetResult =
            await userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

        return resetResult.Succeeded
            ? new Response<string>("Password reset successfully")
            : new Response<string>(HttpStatusCode.InternalServerError, "Failed to reset password");
    }

    private async Task<string> GenerateJwt(IdentityUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //var roles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? "")
        };

        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        //claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}



