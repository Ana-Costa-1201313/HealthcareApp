using Backoffice.Domain.OperationRequests;

namespace Backoffice.Domain.Appointments
{
    public class AppointmentWithDetailsDto
    {
        public Guid AppointmentId { get; set; }
        public OperationRequestDto OperationRequest { get; set; }
        //public List<string> StaffIds { get; set; }
        public Guid SurgeryRoomId { get; set; }
        public Int32 SurgeryRoomNumber { get; set; }
        public string DateTime { get; set; }
        public string Status { get; set; }
    }
}