using Backoffice.Domain.Shared;
using Backoffice.Domain.SurgeryRooms.ValueObjects;
using Microsoft.IdentityModel.Tokens;

namespace Backoffice.Domain.SurgeryRooms
{
    public class SurgeryRoom : Entity<SurgeryRoomId>, IAggregateRoot
    {
        public SurgeryRoomId SurgeryRoomId { get; set; }
        public Int32 RoomNumber { get; set; }
        public SurgeryRoomType SurgeryRoomType { get; private set; }
        public Int16 Capacity { get; set; }
        public SurgeryRoomStatus SurgeryRoomStatus { get; set; }
        public List<TimeSlot> MaintenanceSlots { get; set; }

        public SurgeryRoom(){}

        public void ChangeOccupiedStatus()
        {
            if (this.SurgeryRoomStatus == SurgeryRoomStatus.Available)
            {
                this.SurgeryRoomStatus = SurgeryRoomStatus.Occupied;
            }
            else
            {
                this.SurgeryRoomStatus = SurgeryRoomStatus.Available;
            }
        }

        public void ChangeUnderMaintenanceStatus()
        {
            if (this.SurgeryRoomStatus == SurgeryRoomStatus.Available)
            {
                this.SurgeryRoomStatus = SurgeryRoomStatus.UnderMaintenance;
            }
            else
            {
                this.SurgeryRoomStatus = SurgeryRoomStatus.Available;
            }
        }

        public SurgeryRoom(Int32 roomNumber, SurgeryRoomType surgeryRoomType, Int16 capacity, List<TimeSlot> maintenanceSlots)
        {
            this.Id = new SurgeryRoomId(Guid.NewGuid());

            if (roomNumber == default || roomNumber <= 0) throw new BusinessRuleValidationException("RoomNumber can't be 0 nor negative.");
            this.RoomNumber = roomNumber;

            if (surgeryRoomType == null) throw new BusinessRuleValidationException("SurgeryRoomType can't be null.");
            this.SurgeryRoomType = surgeryRoomType;
            // this.SurgeryRoomType = null ;

            if (capacity == default || capacity <= 0) throw new BusinessRuleValidationException("Capacity can't be 0 nor negative.");
            this.Capacity = capacity;

            //if (maintenanceSlots == null || !maintenanceSlots.Any()) throw new BusinessRuleValidationException("MaintenanceSlots can't be null or empty.");
            this.SurgeryRoomStatus = SurgeryRoomStatus.Available;
            this.MaintenanceSlots = maintenanceSlots;
        }

        public void AddMaintenanceSlot(TimeSlot timeSlot)
        {
            if (timeSlot == null) throw new BusinessRuleValidationException("TimeSlot can't be null.");
            this.MaintenanceSlots.Add(timeSlot);
        }
    }
}