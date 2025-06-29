using Backoffice.Domain.Shared;
using Backoffice.Domain.Staffs;
using Xunit;

namespace Backoffice.Tests
{
    public class EmailTest
    {
        [Fact]
        public void ValidEmail()
        {
            string email = "aaa@gmail.com";

            Email newEmail = new Email(email);

            Assert.NotNull(newEmail);
            Assert.Equal(email, newEmail._Email);
        }

        [Fact]
        public void InvalidEmail()
        {
            var exception = Assert.Throws<BusinessRuleValidationException>(() => new Email("aaagmail.com"));

            Assert.Equal("Error: The email is invalid!", exception.Message);
        }

        [Fact]
        public void EmptyEmail()
        {
            var exception = Assert.Throws<BusinessRuleValidationException>(() => new Email(""));

            Assert.Equal("Error: The email is invalid!", exception.Message);
        }

        [Fact]
        public void NullEmail()
        {
            var exception = Assert.Throws<BusinessRuleValidationException>(() => new Email(null));

            Assert.Equal("Error: The email is invalid!", exception.Message);
        }
    }
}