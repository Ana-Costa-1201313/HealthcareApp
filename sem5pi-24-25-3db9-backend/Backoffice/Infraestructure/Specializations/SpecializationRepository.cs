using Backoffice.Domain.Specializations;
using Backoffice.Infraestructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace Backoffice.Infraestructure.Specializations
{
    public class SpecializationRepository : BaseRepository<Specialization, SpecializationId>, ISpecializationRepository
    {

        private readonly BDContext _context;

        public SpecializationRepository(BDContext context) : base(context.Specializations)
        {
            this._context = context;
        }

        public async Task<Specialization> GetBySpecializationName(string name)
        {
            return await this._context.Specializations.Where(x => name.Equals(x.Name.Name)).FirstOrDefaultAsync();
        }

        public async Task<Specialization> GetBySpecializationCode(string code)
        {
            return await this._context.Specializations.Where(x => code.Equals(x.Code)).FirstOrDefaultAsync();
        }

        public async Task<bool> SpecializationNameExists(string name)
        {
            Specialization specialization = await GetBySpecializationName(name);
            return specialization != null;
        }

        public async Task<bool> SpecializationCodeExists(string code)
        {
            Specialization specialization = await GetBySpecializationCode(code);
            return specialization != null;
        }

    }
}