using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Backoffice.Domain.Shared;

namespace Backoffice.Infraestructure.Shared
{
    public class EmailConverter : ValueConverter<Email, string>
    {
        public EmailConverter()
            : base(
                email => email.ToString(),
                emailString => new Email(emailString))
        {

        }
    }
}
