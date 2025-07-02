using Domain.DTOs.Email;

namespace Infrastructure.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailDto emailDto);
}
