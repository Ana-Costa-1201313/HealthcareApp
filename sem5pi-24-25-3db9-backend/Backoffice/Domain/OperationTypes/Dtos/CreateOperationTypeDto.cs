namespace Backoffice.Domain.OperationTypes
{
    public class CreatingOperationTypeDto
    {
        public string Name { get; set; }
        public int AnesthesiaPatientPreparationInMinutes { get; set; }
        public int SurgeryInMinutes { get; set; }
        public int CleaningInMinutes { get; set; }
        public List<RequiredStaffDto> RequiredStaff { get; set; }

        public CreatingOperationTypeDto(string name, int anesthesiaPatientPreparationInMinutes, 
        int surgeryInMinutes, int cleaningInMinutes, List<RequiredStaffDto> requiredStaff)
        {
            this.Name = name;
            this.AnesthesiaPatientPreparationInMinutes = anesthesiaPatientPreparationInMinutes;
            this.SurgeryInMinutes = surgeryInMinutes;
            this.CleaningInMinutes = cleaningInMinutes;
            this.RequiredStaff = requiredStaff;
        }
    }
}