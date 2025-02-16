using MoneyMap.Infrastructure.Data;

namespace MoneyMap.Application.Services;
public interface IUserService
{
    List<ApplicationUser> GetAll();
}