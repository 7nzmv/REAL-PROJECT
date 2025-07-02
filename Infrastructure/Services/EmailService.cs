using System.Net;
using System.Net.Mail;
using Domain.DTOs.Email;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(EmailDto emailDto)
    {
        try
        {
            var senderAddress = _configuration["SMTPConfig:SenderAddress"]
                                ?? throw new InvalidOperationException("SMTP sender address is not configured");

            var password = _configuration["SMTPConfig:Password"]
                           ?? throw new InvalidOperationException("SMTP password is not configured");

            var host = _configuration["SMTPConfig:Host"]
                       ?? throw new InvalidOperationException("SMTP host is not configured");

            var port = int.TryParse(_configuration["SMTPConfig:Port"], out var smtpPort)
                ? smtpPort
                : throw new InvalidOperationException("Invalid SMTP port configuration");

            var mailMessage = new MailMessage
            {
                Subject = emailDto.Subject,
                Body = emailDto.Body,
                IsBodyHtml = true,
                From = new MailAddress(senderAddress)
            };

            mailMessage.To.Add(new MailAddress(emailDto.To));

            var smtpClient = new SmtpClient
            {
                Credentials = new NetworkCredential(senderAddress, password),
                Host = host,
                Port = port,
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent to {Email}", emailDto.To);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", emailDto.To);
            return false;
        }
    }
}
