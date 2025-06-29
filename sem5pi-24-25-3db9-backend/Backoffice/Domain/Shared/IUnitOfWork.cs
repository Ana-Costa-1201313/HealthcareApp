using System.Threading.Tasks;

namespace Backoffice.Domain.Shared
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync();
    }
}