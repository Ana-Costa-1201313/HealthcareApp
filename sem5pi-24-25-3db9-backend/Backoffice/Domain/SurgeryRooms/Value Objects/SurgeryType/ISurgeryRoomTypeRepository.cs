using Backoffice.Domain.Shared;

namespace Backoffice.Domain.SurgeryRooms.ValueObjects
{
    public interface ISurgeryRoomTypeRepository : IRepository<SurgeryRoomType, SurgeryRoomTypeId>
    {
        Task<bool> SurgeryRoomTypeExists(string type);
        Task<List<string>> getTypeList();
        public Task<List<SurgeryRoomTypeDto>> getTypeListDTO();
    }
}