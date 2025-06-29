using Backoffice.Domain.Shared;

namespace Backoffice.Domain.Users
{
    public interface IUserRepository : IRepository<User, UserId>
    {
    Task<User> getUserByEmail(string email);
    }
}