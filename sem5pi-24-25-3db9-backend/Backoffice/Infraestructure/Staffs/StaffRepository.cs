using Backoffice.Domain.Staffs;
using Backoffice.Domain.Shared;
using Backoffice.Infraestructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace Backoffice.Infraestructure.Staffs
{
    public class StaffRepository : BaseRepository<Staff, StaffId>, IStaffRepository
    {
        private readonly BDContext _context;

        public StaffRepository(BDContext context) : base(context.Staff)
        {
            this._context = context;
        }

        public async Task<int> GetLastMechanographicNumAsync()
        {
            return await _context.Staff
                .OrderByDescending(u => u.MecNumSequence)
                .Select(u => u.MecNumSequence)
                .FirstOrDefaultAsync();
        }

        public async Task<Staff> GetStaffByEmailAsync(Email email)
        {
            return await _context.Staff
                .Where(s => s.Email == email)
                .Select(s => s)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Staff>> GetAllWithDetailsAsync(int pageNum, int pageSize)
        {
            return await _context.Staff
            .Include(s => s.Specialization)
            .OrderBy(s => s.Id)
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        }

        public async Task<Staff> GetByIdWithDetailsAsync(StaffId id)
        {
            return await _context.Staff
            .Include(s => s.Specialization)
            .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Staff>> FilterStaffAsync(string name, string email, string specialization, int pageNum, int pageSize)
        {
            var query = _context.Staff.Include(s => s.Specialization).AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(s => s.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(specialization))
            {
                query = query.Where(s => s.Specialization.Name.Name.Contains(specialization));
            }

            var result = await query.ToListAsync();

            if (!string.IsNullOrEmpty(email))
            {
                result = result.Where(s => s.Email._Email.Contains(email)).ToList();
            }

            return result.OrderBy(s => s.Id).Skip((pageNum - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Staff.CountAsync();
        }
    }
}