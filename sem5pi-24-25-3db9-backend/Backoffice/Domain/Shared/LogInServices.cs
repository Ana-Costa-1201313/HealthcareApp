using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Backoffice.Domain.Users;
using Backoffice.Domain.Shared;
using Backoffice.Infraestructure.Users;
using System.Text;

namespace Backoffice.Domain.Shared
{
    public class LogInServices
    {
        protected string validateUrl = "Login/validate";

        private readonly IConfiguration _iconfig;
        private readonly IUserRepository _userRepository;
        private readonly HttpClient _httpClient;


        public LogInServices(IConfiguration iConfig, IUserRepository userRepository, HttpClient httpClient)
        {
            this._iconfig = iConfig;
            this._userRepository = userRepository;
            this._httpClient = httpClient;
        }

        public async Task<bool> CheckUserCredentials1(string email, string password)
        {
            User user = await _userRepository.getUserByEmail(email);
            //if (user == null) return false;
            if (user == null) throw new NullReferenceException("user � null em CheckUserCredentials1");
            if (password.Equals(user.Password.Passwd)) return true;
            //if (password.Equals(user.Password.Passwd)) throw new Exception("password � null");
            return false;
        }

        public async Task<LoginDTO> getLoginDTO(string email)
        {
            LoginDTO loginDTO = new LoginDTO();

            User user = await _userRepository.getUserByEmail(email);
            if (user == null) return null;

            loginDTO.username = user.Email.ToString();
            loginDTO.password = user.Password.Passwd;
            loginDTO.role = user.Role.ToString();

            return loginDTO;
        }

        public async Task<LoginDTO> checkTokenValidaty(LoginDTO loginDTO)
        {
            if (loginDTO.jwt == "")
            {
                return null;
            }
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, _iconfig["Auth:urlLocal"] + validateUrl);

                request.Headers.Add("Authorization", $"Bearer {loginDTO.jwt}");

                HttpResponseMessage responseMessage = await _httpClient.SendAsync(request);

                responseMessage.EnsureSuccessStatusCode();

                string responseBody = await responseMessage.Content.ReadAsStringAsync();

                LoginDTO resultLoginDTO = JsonSerializer.Deserialize<LoginDTO>(responseBody);

                return resultLoginDTO;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}