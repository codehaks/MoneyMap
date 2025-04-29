using Microsoft.Extensions.Logging;
using MoneyMap.Infrastructure.Data;

namespace MoneyMap.Application;

public class LoggingUserServiceDecorator : IUserService
{
    private readonly IUserService _inner;
    private readonly ILogger<LoggingUserServiceDecorator> _logger;

    public LoggingUserServiceDecorator(IUserService inner, ILogger<LoggingUserServiceDecorator> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public List<ApplicationUser> GetAll()
    {
        return _inner.GetAll();
    }

    public async Task<IList<string>> GetRoles(string userId)
    {
        _logger.LogInformation("Fetching roles for user: {UserId}", userId);
        var roles = await _inner.GetRoles(userId);
        _logger.LogInformation("User {UserId} has roles: {Roles}", userId, string.Join(", ", roles));
        return roles;
    }
}
