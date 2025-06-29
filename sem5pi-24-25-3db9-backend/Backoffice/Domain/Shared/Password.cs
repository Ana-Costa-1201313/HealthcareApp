using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Backoffice.Domain.Shared
{
    [Owned]
    public class Password : IValueObject

    {
        public string Passwd { get; private set; }

        private Password()
        {

        }

        public Password(string password)
        {
            if (password.Length < 10)
            {
                throw new BusinessRuleValidationException("Error: The password must be at least 10 characters long.");
            }

            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                throw new BusinessRuleValidationException("Error: The password must have a digit.");
            }

            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                throw new BusinessRuleValidationException("Error: The password must have a capital letter.");
            }

            if (!Regex.IsMatch(password, @"[_!@#$%^&*(),.?""{}|<>]"))
            {
                throw new BusinessRuleValidationException("Error: The password must have a special character.");
            }

            this.Passwd = password;
        }
    }
}