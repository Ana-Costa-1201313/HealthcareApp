using Backoffice.Domain.Users;
using Backoffice.Domain.Shared;
using Backoffice.Infraestructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace Backoffice.Infraestructure.Users
{
    public class UserRepository : BaseRepository<User, UserId>, IUserRepository
    {
        private readonly BDContext _context;

        public UserRepository(BDContext context) : base(context.Users)
        {
            this._context = context;

        }

        public async Task<User> getUserByEmail(string email)
        {
            Email em = new Email(email);

            User user = null;
            user = await this._context.Users.Where(x => x.Email.Equals(em)).Select(x => x).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new Exception("Invalid email or password. User not found.");
            }
            return user;
        }
    }
}