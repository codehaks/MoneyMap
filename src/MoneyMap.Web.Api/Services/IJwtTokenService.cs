using MoneyMap.Infrastructure.Data;

namespace MoneyMap.Web.Api.Services;

public interface IJwtTokenService
{
    Task<string> GenerateAccessTokenAsync(ApplicationUser user);
}
