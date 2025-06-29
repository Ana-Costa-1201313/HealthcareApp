using System;
using Backoffice.Domain.Shared;

namespace Backoffice.Domain.Specializations
{
    public class Specialization : Entity<SpecializationId>, IAggregateRoot
    {

        public virtual SpecializationName Name { get; private set; }
        public virtual string Code { get; private set; }
        public virtual string Description { get; private set; }

        public bool Active { get; private set; }

        public Specialization()
        {
            this.Active = true;
        }

        public Specialization(SpecializationName name, string code, string description)
        {
            this.Id = new SpecializationId(Guid.NewGuid());
            if (name == null)
            {
                throw new BusinessRuleValidationException("Error: The specialization name can't be null.");
            }
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new BusinessRuleValidationException("Error: The specialization code can't be null, empty or consist in only white spaces.");
            }
            this.Name = name;
            this.Code = code;
            this.Description = description;
            this.Active = true;
        }

        public void ChangeName(SpecializationName name)
        {
            if (!this.Active)
                throw new BusinessRuleValidationException("Error: It is not possible to change the name of an inactive specialization.");
            this.Name = name;
        }

        public void ChangeDescription(string description){
            this.Description = description;
        }
        
        public void MarkAsInative()
        {

            if (!this.Active)
            {
                throw new BusinessRuleValidationException("Error: This specialization is already inactive.");
            }
            this.Active = false;
        }
    }
}