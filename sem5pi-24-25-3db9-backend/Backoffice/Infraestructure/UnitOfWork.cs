using System.Threading.Tasks;
using Backoffice.Domain.Shared;

namespace Backoffice.Infraestructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BDContext _context;

        public UnitOfWork(BDContext context)
        {
            this._context = context;
        }

        public async Task<int> CommitAsync()
        {
            return await this._context.SaveChangesAsync();
        }
    }
}