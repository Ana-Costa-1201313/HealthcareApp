using Backoffice.Domain.Shared;
using System;
using System.Configuration;

namespace Backoffice.Domain.Users
{
    public class User : Entity<UserId>, IAggregateRoot
    {
        public Role Role { get; private set; }

        public Email Email { get; private set; }

        public Password Password { get; private set; }

        public bool Active { get; private set; }

        private User()
        {

        }

        public User(Role role, string email)
        {
            this.Id = new UserId(Guid.NewGuid());
            this.Role = role;
            this.Email = new Email(email);
            this.Active = false;
        }

        public void ActivateUser(string passwd)
        {
            if (this.Active)
            {
                throw new BusinessRuleValidationException("Error: This user is already active!");
            }
            this.Password = new Password(passwd);

            this.Active = true;
        }

        public void ResetPassword(string password)
        {
            this.Password = new Password(password);
        }
    }
}