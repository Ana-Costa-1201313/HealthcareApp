namespace Backoffice.Domain.Appointments
{
    public class UpdateAppointmentDto
    {
        //public Guid AppointmentId { get; set; }
        public string DateTime { get; set; }
        public string SurgeryRoomNumber { get; set; }
        public string AppointmentStatus { get; set; }
        public List<string> StaffList { get; set; }
    }
}