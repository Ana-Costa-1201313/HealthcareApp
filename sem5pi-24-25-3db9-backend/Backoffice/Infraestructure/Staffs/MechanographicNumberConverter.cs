using Backoffice.Domain.Staffs;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backoffice.Infraestructure.Staffs
{
    public class MechanographicNumConverter : ValueConverter<MechanographicNumber, string>
    {
        public MechanographicNumConverter()
            : base(
                num => num.ToString(),
                mecNumString => new MechanographicNumber(mecNumString))
        {
            
        }
    }
}