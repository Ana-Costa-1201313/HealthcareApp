using Backoffice.Domain.Shared;

namespace Backoffice.Domain.SurgeryRooms;

public class SurgeryRoomDto
{
    public string RoomNumber { get; set; }
    public string SurgeryRoomType { get; set; }
    public Int16 Capacity { get; set; }
    public string SurgeryRoomStatus { get; set; }
    public List<string> MaintenanceSlots { get; set; }
}