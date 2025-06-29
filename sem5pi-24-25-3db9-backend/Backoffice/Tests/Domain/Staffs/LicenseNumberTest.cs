using Backoffice.Domain.Shared;
using Backoffice.Domain.Staffs;
using Xunit;

namespace Backoffice.Tests
{
    public class LicenseNumberTest
    {
        [Fact]
        public void ValidLicenseNum()
        {
            int licenseNum = 123;

            LicenseNumber newLicenseNumber = new LicenseNumber(licenseNum);

            Assert.NotNull(newLicenseNumber);
            Assert.Equal(licenseNum, newLicenseNumber.LicenseNum);
        }

        [Fact]
        public void zeroLicenseNum()
        {
            int licenseNum = 0;

             var exception = Assert.Throws<BusinessRuleValidationException>(() => new LicenseNumber(licenseNum));

            Assert.Equal("Error: The staff's license number can't be zero!", exception.Message);
        }
    }
}