using MakApi.Data.Models;

namespace MakApi.Services;

public interface IEmailService
{
    Task SendUserCreatedEmailAsync(User user);
    Task SendUserUpdatedEmailAsync(User user);
} 