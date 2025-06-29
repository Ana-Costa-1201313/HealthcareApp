using System;
using Backoffice.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Backoffice.Domain.OperationTypes
{
    [Owned]
    public class OperationTypeName : IValueObject
    {

        public virtual string Name {get; private set;}

        public OperationTypeName()
        {
        }

        public OperationTypeName(string name)
        {
            
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new BusinessRuleValidationException("Error: The operation type name can't be null, empty or consist in only white spaces.");
            }
            this.Name = name;
        }
    }
}