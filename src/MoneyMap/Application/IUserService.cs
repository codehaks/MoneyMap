using MoneyMap.Infrastructure.Data;

namespace MoneyMap.Application;

public interface IUserService
{
    Task<IReadOnlyList<ApplicationUser>> GetAllAsync(CancellationToken ct = default);
    Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken ct = default);
    Task<IList<string>> GetRolesAsync(string userId, CancellationToken ct = default);
}
