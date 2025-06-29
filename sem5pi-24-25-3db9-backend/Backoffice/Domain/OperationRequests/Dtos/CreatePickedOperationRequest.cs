namespace Backoffice.Domain.OperationRequests
{
    public class CreatePickedOperationRequestDto
    {
        public string OpTypeName { get; set; }
        public string DeadlineDate { get; set; }
        public string Priority { get; set; }
        public string PatientEmail { get; set; }
        public string DoctorEmail { get; set; }
        public string Description { get; set; }
        public List<string> StaffList { get; set; }
    }
}