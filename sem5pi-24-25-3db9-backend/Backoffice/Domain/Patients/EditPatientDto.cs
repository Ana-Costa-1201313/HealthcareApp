namespace Backoffice.Domain.Patients
{
    public class EditPatientDto
    {
       
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string[] Allergies { get; set; }
        public string EmergencyContact { get; set; }  
    }
}
