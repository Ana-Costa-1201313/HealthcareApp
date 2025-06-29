namespace Backoffice.Domain.Specializations
{
    public class CreatingSpecializationDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public CreatingSpecializationDto(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }
    }
}