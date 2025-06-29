using Backoffice.Domain.SurgeryRooms.ValueObjects;
using Backoffice.Infraestructure.Shared;
using Backoffice.Infraestructure;
using Microsoft.EntityFrameworkCore;
using Backoffice.Domain.SurgeryRooms;

namespace Backoffice.Infrastructure.SurgeryRooms
{
    public class SurgeryRoomTypeRepository : BaseRepository<SurgeryRoomType, SurgeryRoomTypeId>, ISurgeryRoomTypeRepository
    {
        private readonly BDContext _context;

        public SurgeryRoomTypeRepository(BDContext context) : base(context.SurgeryRoomTypes)
        {
            _context = context;
        }

        public async Task<List<string>> getTypeList(){
            List<SurgeryRoomType> values = await this.GetAllAsync();
            
            List<string> res = new List<string>();
            foreach (SurgeryRoomType item in values)
            {
                res.Add(item.Type);
            }
            return res;
        }

        public async Task<List<SurgeryRoomTypeDto>> getTypeListDTO(){
            List<SurgeryRoomType> values = await this.GetAllAsync();
            
            List<SurgeryRoomTypeDto> res = new List<SurgeryRoomTypeDto>();
            foreach (SurgeryRoomType item in values)
            {
                SurgeryRoomTypeDto s = new SurgeryRoomTypeDto();
                s.surgeryRoomType = item.Type;
                s.description = item.Description;
                s.fitForSurgery = item.fitForSurgery;

                res.Add(s);
            }
            return res;
        }

        public async Task<bool> GetSurgeryRoomTypeByIdAsync(string tipo)
        {
            if( await _context.SurgeryRoomTypes
                .Where(x => tipo.Equals(x.Type)).FirstOrDefaultAsync() != null) return true;
            return false;
        }

        public async Task<bool> SurgeryRoomTypeExists(string type){
            if((await this._context.SurgeryRoomTypes.Where(x => type.Equals(x.Type)).FirstOrDefaultAsync()) != null) return true;
            return false;
        }
    }
}