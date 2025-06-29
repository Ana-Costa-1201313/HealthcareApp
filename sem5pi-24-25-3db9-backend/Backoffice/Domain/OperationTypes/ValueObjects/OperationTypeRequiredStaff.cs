using System;
using System.ComponentModel.DataAnnotations.Schema;
using Backoffice.Domain.Shared;
using Backoffice.Domain.Specializations;
using Microsoft.EntityFrameworkCore;

namespace Backoffice.Domain.OperationTypes
{
    [Owned]
    public class OperationTypeRequiredStaff : IValueObject
    {

        public SpecializationId SpecializationId { get; private set; }
        public Specialization Specialization { get; private set; }
        public int Total { get; private set; }

        public OperationTypeRequiredStaff()
        {
        }

        public OperationTypeRequiredStaff(Specialization specialization, int total)
        {
            if (specialization == null)
            {
                throw new BusinessRuleValidationException("Error: The specialization can't be null.");
            }
            this.SpecializationId = specialization.Id;
            this.Specialization = specialization;
            if (total <= 0)
            {
                throw new BusinessRuleValidationException("Error: The total number of required staff of a specialization can't be lower or equal to 0.");
            }
            this.Total = total;
        }
    }
}