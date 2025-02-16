using MoneyMap.Infrastructure.Data;

namespace MoneyMap.Application;
public interface IUserService
{
    List<ApplicationUser> GetAll();
    Task<IList<string>> GetRoles(string userId);
}