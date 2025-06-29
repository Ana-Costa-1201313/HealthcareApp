using Backoffice.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Backoffice.Domain.Staffs
{
    [Owned]
    public class LicenseNumber : IValueObject
    {
        public int LicenseNum {get; private set; }

        private LicenseNumber() {

        }

        public LicenseNumber(int licenseNum) {
            if (licenseNum == 0)
            {
                throw new BusinessRuleValidationException("Error: The staff's license number can't be zero!");
            }
            this.LicenseNum = licenseNum;
        }
    }
}