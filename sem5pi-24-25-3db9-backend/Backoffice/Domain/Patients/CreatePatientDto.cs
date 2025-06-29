namespace Backoffice.Domain.Patients
{
    public class CreatePatientDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string EmergencyContact {get; set;}
       // public int MedicalRecordNumber { get; set; }
        //public List<Appointment> Appointments { get; set; }
    }
}