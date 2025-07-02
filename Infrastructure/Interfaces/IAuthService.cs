using Domain.DTOs.Auth;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IAuthService
{
    Task<Response<TokenDto>> Login(LoginDto loginDto);
    Task<Response<string>> Register(RegisterDto registerDto);
    Task<Response<string>> RequestResetPassword(RequestResetPassword requestResetPassword);
    Task<Response<string>> ResetPassword(ResetPasswordDto resetPasswordDto);
}
