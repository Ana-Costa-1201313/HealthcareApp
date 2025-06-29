using Backoffice.Domain.Shared;

namespace Backoffice.Domain.SurgeryRooms
{
    public interface ISurgeryRoomRepository : IRepository<SurgeryRoom, SurgeryRoomId>
    {
        public Task<List<SurgeryRoom>> GetAllSurgeryRoomsAsync();
        public Task<SurgeryRoom> GetSurgeryRoomByIdAsync(SurgeryRoomId id);
        public Task<SurgeryRoom> GetSurgeryRoomByRoomNumberAsync(int roomNumber);
        //public Task<SurgeryRoom> GetOpRequestByIdAsync(SurgeryRoomId id);
        //public Task<List<SurgeryRoom>> GetOpRequestsByDoctorIdAsync(StaffId doctorId);
    }
}