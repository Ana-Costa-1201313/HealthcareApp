using Backoffice.Domain.Shared;

namespace Backoffice.Domain.SurgeryRooms;

public class SurgeryRoomTypeDto
{
    public string surgeryRoomType { get; set; }
    public string description { get; set; }
    public Boolean fitForSurgery { get; set; }
}