namespace Backoffice.Domain.Appointments
{
    public class AppointmentDto
    {
        public Guid AppointmentId { get; set; }
        public Guid OpRequestId { get; set; }
        //public List<string> StaffIds { get; set; }
        public Guid SurgeryRoomId { get; set; }
        public Int32 SurgeryRoomNumber { get; set; }
        public string DateTime { get; set; }
        public string Status { get; set; }
    }
}