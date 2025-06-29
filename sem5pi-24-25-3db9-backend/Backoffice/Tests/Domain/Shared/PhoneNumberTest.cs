using Backoffice.Domain.Shared;
using Backoffice.Domain.Staffs;
using Xunit;

namespace Backoffice.Tests
{
    public class PhoneNumberTest
    {
        [Fact]
        public void ValidPhone()
        {
            string phone = "999999999";

            PhoneNumber phoneNumber = new PhoneNumber(phone);

            Assert.NotNull(phoneNumber);
            Assert.Equal(phone, phoneNumber.PhoneNum);
        }

        [Fact]
        public void SmallPhone()
        {
            string phone = "9999";

            var exception = Assert.Throws<BusinessRuleValidationException>(() => new PhoneNumber(phone));

            Assert.Equal("Error: The phone number is invalid!", exception.Message);
        }

        [Fact]
        public void LongPhone()
        {
            string phone = "9999999999";

            var exception = Assert.Throws<BusinessRuleValidationException>(() => new PhoneNumber(phone));

            Assert.Equal("Error: The phone number is invalid!", exception.Message);
        }

        [Fact]
        public void InvalidStartPhone()
        {
            string phone = "199999999";

            var exception = Assert.Throws<BusinessRuleValidationException>(() => new PhoneNumber(phone));
            
            Assert.Equal("Error: The phone number is invalid!", exception.Message);
        }
    }
}