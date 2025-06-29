using System;
using Backoffice.Domain.Shared;
using Newtonsoft.Json;

namespace Backoffice.Domain.OperationTypes
{
    public class OperationType : Entity<OperationTypeId>, IAggregateRoot
    {

        public virtual OperationTypeName Name { get; private set; }
        public virtual OperationTypeDuration Duration { get; private set; }
        public virtual List<OperationTypeRequiredStaff> RequiredStaff { get; private set; }

        public virtual bool Active { get; private set; }

        public OperationType()
        {
            this.Active = true;
        }

        public OperationType(OperationTypeName name, OperationTypeDuration duration, List<OperationTypeRequiredStaff> requiredStaff)
        {
            this.Id = new OperationTypeId(Guid.NewGuid());
            if (name == null)
            {
                throw new BusinessRuleValidationException("Error: The operation type name can't be null.");
            }
            this.Name = name;
            if (duration == null)
            {
                throw new BusinessRuleValidationException("Error: The operation type duration can't be null.");
            }
            this.Duration = duration;
            if (requiredStaff == null)
            {
                throw new BusinessRuleValidationException("Error: The operation type can't have null required staff.");
            }
            if (!requiredStaff.Any())
            {
                throw new BusinessRuleValidationException("Error: The operation type must have at least one required staff.");
            }
            this.RequiredStaff = requiredStaff;
            this.Active = true;
        }

        public void MarkAsInative()
        {
            if (!this.Active)
            {
                throw new BusinessRuleValidationException("Error: The operation type is already inactive.");
            }
            this.Active = false;
        }

        public void ChangeName(OperationTypeName name)
        {
            if (!this.Active)
            {
                throw new BusinessRuleValidationException("Error: Can't update an inactive operation type.");
            }
            this.Name = name;
        }

        public void ChangeAnesthesiaPatientPreparationDuration(int duration)
        {
            if (!this.Active)
            {
                throw new BusinessRuleValidationException("Error: Can't update an inactive operation type.");
            }
            if (duration <= 0)
            {
                throw new BusinessRuleValidationException("Error: The anesthesia/preparation duration must be more than 0 minutes.");
            }
            this.Duration.AnesthesiaPatientPreparationInMinutes = TimeSpan.FromMinutes(duration);
        }

        public void ChangeSurgeryDuration(int duration)
        {
            if (!this.Active)
            {
                throw new BusinessRuleValidationException("Error: Can't update an inactive operation type.");
            }
            if (duration <= 0)
            {
                throw new BusinessRuleValidationException("Error: The surgery duration must be more than 0 minutes.");
            }
            this.Duration.SurgeryInMinutes = TimeSpan.FromMinutes(duration);
        }

        public void ChangeCleaningDuration(int duration)
        {
            if (!this.Active)
            {
                throw new BusinessRuleValidationException("Error: Can't update an inactive operation type.");
            }
            if (duration <= 0)
            {
                throw new BusinessRuleValidationException("Error: The cleaning duration must be more than 0 minutes.");
            }
            this.Duration.CleaningInMinutes = TimeSpan.FromMinutes(duration);
        }

        public void ChangeRequiredStaff(List<OperationTypeRequiredStaff> requiredStaff)
        {
            if (!this.Active)
            {
                throw new BusinessRuleValidationException("Error: Can't update an inactive operation type.");
            }
            if (!requiredStaff.Any())
            {
                throw new BusinessRuleValidationException("Error: Required staff can't be empty.");
            }

            List<OperationTypeRequiredStaff> list = new List<OperationTypeRequiredStaff>();

            foreach (OperationTypeRequiredStaff rs in requiredStaff)
            {
                list.Add(rs);
            }
            this.RequiredStaff = list;
        }

        public void ChangeAll(OperationTypeName name, int duration1, int duration2, int duration3, List<OperationTypeRequiredStaff> requiredStaff)
        {
            if (!this.Active)
            {
                throw new BusinessRuleValidationException("Error: Can't update an inactive operation type.");
            }
            ChangeName(name);
            ChangeAnesthesiaPatientPreparationDuration(duration1);
            ChangeSurgeryDuration(duration2);
            ChangeCleaningDuration(duration3);
            ChangeRequiredStaff(requiredStaff);
        }


        public string ToJSON()
        {
            var jsonRepresentation = new
            {
                Id = this.Id.Value,
                Name = new { this.Name.Name },
                Duration = new
                {
                    AnesthesiaPatientPreparationInMinutes = this.Duration.AnesthesiaPatientPreparationInMinutes,
                    SurgeryInMinutes = this.Duration.SurgeryInMinutes,
                    CleaningInMinutes = this.Duration.CleaningInMinutes
                },
                RequiredStaff = this.RequiredStaff.Select(rs => new
                {
                    Specialization = new { rs.Specialization.Name.Name },
                    Total = rs.Total
                }).ToList(),
                Active = this.Active
            };

            return JsonConvert.SerializeObject(jsonRepresentation, Formatting.Indented);
        }
    }
}