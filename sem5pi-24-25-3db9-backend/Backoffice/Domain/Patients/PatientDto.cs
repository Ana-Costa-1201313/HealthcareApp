using Backoffice.Domain.Appointments;

namespace Backoffice.Domain.Patients
{
    public class PatientDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string EmergencyContact{get;set;}
        public string[] Allergies { get; set; }
        public string MedicalRecordNumber { get; set; }
        public List<AppointmentDto> AppointmentHistory { get; set; }
    }
}