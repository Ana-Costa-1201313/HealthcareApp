using Backoffice.Domain.SurgeryRooms;
using Backoffice.Infraestructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace Backoffice.Infraestructure.SurgeryRooms
{
    public class SurgeryRoomRepository : BaseRepository<SurgeryRoom, SurgeryRoomId>, ISurgeryRoomRepository
    {
        private readonly BDContext _context;
        public SurgeryRoomRepository(BDContext context) : base(context.SurgeryRooms)
        {
            _context = context;
        }

        public async Task<List<SurgeryRoom>> GetAllSurgeryRoomsAsync()
        {
            return await _context.SurgeryRooms
                .Include(x => x.SurgeryRoomType)
                .ToListAsync()
            ;
        }

        public async Task<SurgeryRoom> GetSurgeryRoomByIdAsync(SurgeryRoomId id)
        {
            return await _context.SurgeryRooms
                .Where(x => id.Equals(x.Id))
                .Include(x => x.SurgeryRoomType)
                .FirstOrDefaultAsync();
        }

        public async Task<SurgeryRoom> GetSurgeryRoomByRoomNumberAsync(int roomNumber)
        {
            return await _context.SurgeryRooms
                .Where(x => x.RoomNumber == roomNumber)
                .Include(x => x.SurgeryRoomType)
                .FirstOrDefaultAsync();
        }
    }
}