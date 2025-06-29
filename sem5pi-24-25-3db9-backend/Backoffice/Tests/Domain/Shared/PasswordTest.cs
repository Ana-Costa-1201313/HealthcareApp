using Backoffice.Domain.Shared;
using Backoffice.Domain.Staffs;
using Xunit;

namespace Backoffice.Tests
{
    public class PasswordTest
    {
        [Fact]
        public void ValidPassword()
        {
            string passwd = "aaaaaaaaaaA1_";

            Password password = new Password(passwd);

            Assert.NotNull(password);
            Assert.Equal(passwd, password.Passwd);
        }

        [Fact]
        public void SmallPassword()
        {
            string passwd = "aaaaaA1_";

            var exception = Assert.Throws<BusinessRuleValidationException>(() => new Password(passwd));

            Assert.Equal("Error: The password must be at least 10 characters long.", exception.Message);
        }

        [Fact]
        public void WithoutDigitPassword()
        {
            string passwd = "aaaaaaaaaaA_";

            var exception = Assert.Throws<BusinessRuleValidationException>(() => new Password(passwd));

            Assert.Equal("Error: The password must have a digit.", exception.Message);
        }

        [Fact]
        public void WithoutUpperCasePassword()
        {
            string passwd = "aaaaaaaaaa1_";

            var exception = Assert.Throws<BusinessRuleValidationException>(() => new Password(passwd));

            Assert.Equal("Error: The password must have a capital letter.", exception.Message);
        }

        [Fact]
        public void WithoutSpecialCharPassword()
        {
            string passwd = "aaaaaaaaaa1A";

            var exception = Assert.Throws<BusinessRuleValidationException>(() => new Password(passwd));

            Assert.Equal("Error: The password must have a special character.", exception.Message);
        }
    }
}