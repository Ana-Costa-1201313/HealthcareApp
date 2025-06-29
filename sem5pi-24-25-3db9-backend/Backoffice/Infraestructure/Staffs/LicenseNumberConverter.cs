using Backoffice.Domain.Staffs;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backoffice.Infraestructure.Staffs
{
    public class LicenseNumberConverter : ValueConverter<LicenseNumber, int>
    {
        public LicenseNumberConverter()
            : base(
                Licnum => Licnum.LicenseNum,
                LicNumInt => new LicenseNumber(LicNumInt))
        {
            
        }
    }
}