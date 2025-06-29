using Microsoft.EntityFrameworkCore;

namespace Backoffice.Domain.Shared
{
    [Owned]
    public class Email : IValueObject

    {
        public string _Email { get; private set;}

        private Email()
        {

        }

        public Email(string email)
        {
             if(string.IsNullOrEmpty(email) || !email.Contains("@")) {
                throw new BusinessRuleValidationException("Error: The email is invalid!");
            }
            this._Email = email;
        }

        public override string ToString()
        {
            return this._Email;
        }
    }
}