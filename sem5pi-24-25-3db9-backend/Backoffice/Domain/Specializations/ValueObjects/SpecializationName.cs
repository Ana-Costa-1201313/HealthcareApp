using System;
using Backoffice.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Backoffice.Domain.Specializations
{
    [Owned]
    public class SpecializationName : IValueObject
    {

        public virtual string Name { get; set; }

        public SpecializationName()
        {
        }

        public SpecializationName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new BusinessRuleValidationException("Error: The specialization name can't be null, empty or consist in only white spaces.");
            }
            this.Name = name;

        }

    }
}