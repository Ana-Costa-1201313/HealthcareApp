using System;
using Backoffice.Domain.SurgeryRooms;
using Backoffice.Domain.SurgeryRooms.ValueObjects;

namespace Backoffice.Domain.SurgeryRooms
{
    public static class SurgeryRoomMapper
    {
        public static SurgeryRoomDto ToDto(SurgeryRoom surgeryRoom, SurgeryRoomType type)
        {
            if (surgeryRoom == null) return null;

            if (type == null) throw new Exception("type null.");

            List<string> maintenanceSlots = new List<string>();

            foreach (var slot in surgeryRoom.MaintenanceSlots)
            {
                maintenanceSlots.Add(slot.ToString());
            }

            try{
                var roomNumber = surgeryRoom.RoomNumber.ToString();
            } catch (Exception)
            {
                throw new Exception("roomNumber error.");
            }

            string surgeryRoomType = "";

            try{
                surgeryRoomType = type.ToString();
            } catch (Exception)
            {
                throw new Exception("surgeryRoomType error.");
            }

            return new SurgeryRoomDto
            {
                RoomNumber = surgeryRoom.RoomNumber.ToString(),
                SurgeryRoomType = surgeryRoomType,
                Capacity = surgeryRoom.Capacity,
                SurgeryRoomStatus = surgeryRoom.SurgeryRoomStatus.ToString(),
                MaintenanceSlots = maintenanceSlots
            };
        }
        /*
        public static SurgeryRoom ToDomain(SurgeryRoomDto surgeryRoomDto)
        {
            if (surgeryRoomDto == null) return null;

            return new SurgeryRoom
            {
                Id = surgeryRoomDto.Id,
                Name = surgeryRoomDto.Name,
                Location = surgeryRoomDto.Location,
                Capacity = surgeryRoomDto.Capacity,
                IsAvailable = surgeryRoomDto.IsAvailable
            };
        }*/
    }
}