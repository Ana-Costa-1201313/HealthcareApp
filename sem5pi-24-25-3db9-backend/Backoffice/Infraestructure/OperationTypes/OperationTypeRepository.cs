using Backoffice.Domain.OperationTypes;
using Backoffice.Infraestructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace Backoffice.Infraestructure.OperationTypes
{
    public class OperationTypeRepository : BaseRepository<OperationType, OperationTypeId>, IOperationTypeRepository
    {

        private readonly BDContext _context;
        public OperationTypeRepository(BDContext context) : base(context.OperationTypes)
        {
            _context = context;
        }

        public async Task<List<OperationType>> GetAllWithDetailsAsync()
        {
            return await _context.OperationTypes
            .Include(rs => rs.RequiredStaff)
            .ThenInclude(s => s.Specialization)
            .ToListAsync();
        }

        public async Task<OperationType> GetByIdWithDetailsAsync(OperationTypeId id)
        {
            return await _context.OperationTypes
            .Include(rs => rs.RequiredStaff)
            .ThenInclude(s => s.Specialization)
            .FirstOrDefaultAsync(rs => rs.Id == id);
        }

        public async Task<OperationType> GetByOperationTypeName(string name)
        {
            return await this._context.OperationTypes
            .Where(x => name.Equals(x.Name.Name))
            .Include(rs => rs.RequiredStaff)
            .ThenInclude(s => s.Specialization)
            .FirstOrDefaultAsync();
        }

        public async Task<bool> OperationTypeNameExists(string name)
        {

            OperationType operationType = await GetByOperationTypeName(name);
            return operationType != null;
        }

        public async Task<List<OperationType>> FilterOperationTypesAsync(string name, string specialization, bool? status)
        {
            var query = _context.OperationTypes
                    .Include(op => op.RequiredStaff)
                    .ThenInclude(rs => rs.Specialization)
                    .AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(op => op.Name.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(specialization))
            {
                query = query.Where(op => op.RequiredStaff.Any(rs => rs.Specialization.Name.Name.Contains(specialization)));
            }

            if (status.HasValue)
            {
                query = query.Where(op => op.Active == status.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<OperationType> GetByIdWithDetailsSuperAsync(OperationTypeId id)
        {
            return await _context.OperationTypes
            .Include(rs => rs.RequiredStaff)
            .ThenInclude(s => s.Specialization)
            .Include(d => d.Duration)
            .FirstOrDefaultAsync(rs => rs.Id == id);
        }

    }
}