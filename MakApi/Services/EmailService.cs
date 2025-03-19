using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MakApi.Data.Models;

namespace MakApi.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendUserCreatedEmailAsync(User user)
    {
        try
        {
            _logger.LogInformation("Preparing to send user created email to {Email}", _configuration["Email:To"]);
            
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("MakApi System", _configuration["Email:From"]));
            message.To.Add(new MailboxAddress("Admin", _configuration["Email:To"]));
            message.Subject = "New User Created";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <h2>New User Created</h2>
                    <p>A new user has been created with the following details:</p>
                    <ul>
                        <li>Username: {user.Username}</li>
                        <li>Email: {user.Email}</li>
                        <li>Created At: {user.CreatedAt}</li>
                    </ul>"
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            _logger.LogInformation("Connecting to SMTP server {Host}:{Port}", 
                _configuration["Email:Host"], 
                _configuration["Email:Port"]);

            await client.ConnectAsync(
                _configuration["Email:Host"],
                int.Parse(_configuration["Email:Port"]),
                SecureSocketOptions.StartTls
            );
            
            _logger.LogInformation("Authenticating with SMTP server");
            await client.AuthenticateAsync(_configuration["Email:Username"], _configuration["Email:Password"]);
            
            _logger.LogInformation("Sending email");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            
            _logger.LogInformation("Email sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email");
            throw;
        }
    }

    public async Task SendUserUpdatedEmailAsync(User user)
    {
        try
        {
            _logger.LogInformation("Preparing to send user updated email to {Email}", _configuration["Email:To"]);
            
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("MakApi System", _configuration["Email:From"]));
            message.To.Add(new MailboxAddress("Admin", _configuration["Email:To"]));
            message.Subject = "User Updated";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <h2>User Updated</h2>
                    <p>A user has been updated with the following details:</p>
                    <ul>
                        <li>Username: {user.Username}</li>
                        <li>Email: {user.Email}</li>
                        <li>Updated At: {user.UpdatedAt}</li>
                    </ul>"
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            _logger.LogInformation("Connecting to SMTP server {Host}:{Port}", 
                _configuration["Email:Host"], 
                _configuration["Email:Port"]);

            await client.ConnectAsync(
                _configuration["Email:Host"],
                int.Parse(_configuration["Email:Port"]),
                SecureSocketOptions.StartTls
            );
            
            _logger.LogInformation("Authenticating with SMTP server");
            await client.AuthenticateAsync(_configuration["Email:Username"], _configuration["Email:Password"]);
            
            _logger.LogInformation("Sending email");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            
            _logger.LogInformation("Email sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email");
            throw;
        }
    }
} 