using Backoffice.Domain.Users;

namespace Backoffice.Domain.Shared
{

    public interface IExternalApiServices
    {
        public Task<LoginDTO> validateToken(LoginDTO loginDTO);
        public Task<bool> checkHeader(List<string> roles, string tokenHeader);
        public Task<bool> checkHeaderEmail(string roles, string tokenHeader);
    }
}