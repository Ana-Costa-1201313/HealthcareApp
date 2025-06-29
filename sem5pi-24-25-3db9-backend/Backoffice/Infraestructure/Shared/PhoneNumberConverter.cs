using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Backoffice.Domain.Shared;

namespace Backoffice.Infraestructure.Shared
{
    public class PhoneNumberConverter : ValueConverter<PhoneNumber, string>
    {
        public PhoneNumberConverter()
            : base(
                phoneNumber => phoneNumber.ToString(),
                phoneNumberString => new PhoneNumber(phoneNumberString))
        {

        }
    }
}
