using System.Threading.Tasks;

namespace Backoffice.Domain.Shared
{
    public interface IEmailService
    {
        public Task<bool> SendEmail(string emailAddress, string message, string subject);
    }
}