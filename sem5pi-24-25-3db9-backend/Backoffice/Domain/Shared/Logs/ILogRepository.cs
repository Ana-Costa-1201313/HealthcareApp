using Backoffice.Domain.Shared;

namespace Backoffice.Domain.Logs
{
    public interface ILogRepository : IRepository<Log, LogId>
    {
    }
}