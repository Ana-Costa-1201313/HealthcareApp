using Backoffice.Domain.Logs;
using Backoffice.Infraestructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace Backoffice.Infraestructure.Logs
{
    public class LogRepository : BaseRepository<Log, LogId>, ILogRepository
    {
        public LogRepository(BDContext context) : base(context.Logs)
        {

        }



    }
}