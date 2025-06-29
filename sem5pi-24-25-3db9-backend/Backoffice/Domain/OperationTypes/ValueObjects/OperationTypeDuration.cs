using System;
using Backoffice.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Backoffice.Domain.OperationTypes
{
    [Owned]
    public class OperationTypeDuration : IValueObject
    {

        public virtual TimeSpan AnesthesiaPatientPreparationInMinutes { get; set; }
        public virtual TimeSpan SurgeryInMinutes { get; set; }
        public virtual TimeSpan CleaningInMinutes { get; set; }

        public OperationTypeDuration()
        {
        }

        public OperationTypeDuration(int anesthesiaPatientPreparationInMinutes, int surgeryInMinutes, int cleaningInMinutes)
        {
            if (anesthesiaPatientPreparationInMinutes <= 0)
            {
                throw new BusinessRuleValidationException("Error: The anesthesia/preparation duration must be more than 0 minutes.");
            }
            if (surgeryInMinutes <= 0)
            {
                throw new BusinessRuleValidationException("Error: The surgery duration must be more than 0 minutes.");
            }
            if (cleaningInMinutes <= 0)
            {
                throw new BusinessRuleValidationException("Error: The cleaning duration must be more than 0 minutes.");
            }
            this.AnesthesiaPatientPreparationInMinutes = TimeSpan.FromMinutes(anesthesiaPatientPreparationInMinutes);
            this.SurgeryInMinutes = TimeSpan.FromMinutes(surgeryInMinutes);
            this.CleaningInMinutes = TimeSpan.FromMinutes(cleaningInMinutes);

        }

    }
}