using Backoffice.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Backoffice.Domain.Staffs
{
    [Owned]
    public class MechanographicNumber : IValueObject
    {
        public string MechanographicNum { get; private set; }

        private MechanographicNumber()
        {

        }

        public MechanographicNumber(string role, int recruitmentYear, int mecNum)
        {
            if (string.IsNullOrEmpty(role))
            {
                throw new BusinessRuleValidationException("Error: The role is needed!");
            }

            if (recruitmentYear <= 0)
            {
                throw new BusinessRuleValidationException("Error: The year must be bigger than zero!");
            }

            if (mecNum <= 0)
            {
                throw new BusinessRuleValidationException("Error: The mechanographic number must be bigger than zero!");
            }

            this.MechanographicNum = role.Substring(0, 1) + recruitmentYear + mecNum;
        }

        public MechanographicNumber(string mecNum)
        {
            if (string.IsNullOrEmpty(mecNum))
            {
                throw new BusinessRuleValidationException("Error: The mechanographic number is invalid!");
            }
            this.MechanographicNum = mecNum;
        }

        public override string ToString()
        {
            return this.MechanographicNum;
        }
    }
}