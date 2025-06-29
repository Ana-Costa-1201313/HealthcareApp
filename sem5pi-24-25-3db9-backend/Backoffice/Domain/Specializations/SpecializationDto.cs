using System;

namespace Backoffice.Domain.Specializations
{
    public class SpecializationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}