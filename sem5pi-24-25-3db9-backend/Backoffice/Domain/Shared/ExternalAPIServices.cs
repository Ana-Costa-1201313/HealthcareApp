using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Backoffice.Domain.Users;
using Backoffice.Domain.Shared;
using System.Text;

namespace Backoffice.Domain.Shared
{
    public class ExternalApiServices : IExternalApiServices
    {

        protected string validateUrl = "Auth/Login/validate";

        private readonly IConfiguration _iconfig;

        public ExternalApiServices(IConfiguration iConfig)
        {
            this._iconfig = iConfig;
        }

        public async Task<LoginDTO> validateToken(LoginDTO loginDTO)
        {
            // Create a POST request message
            var request = new HttpRequestMessage(HttpMethod.Post, _iconfig["Auth:urlLocal"]+validateUrl);

            // Add the token to the headers
            request.Headers.Add("Authorization", $"Bearer {loginDTO.jwt}");
            request.Content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "application/json");


            //  Send
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.SendAsync(request);
            
            //throw new Exception($"Forced Exception in ExternalApiServices:\n{_iconfig["Auth:urlLocal"] + validateUrl}");
            try
            {   
                response.EnsureSuccessStatusCode(); // Throw an exception if the response status code is not successful
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Failed to authenticate user: {ex.Message}\n {loginDTO.jwt}\n");
            }
            // Deserialize the JSON response content to a LoginDTO object
            string responseBody = await response.Content.ReadAsStringAsync();
            LoginDTO resultLoginDTO = JsonSerializer.Deserialize<LoginDTO>(responseBody);

            return resultLoginDTO;
        }

        public async Task<bool> checkHeader(List<String> roles, String tokenHeader) 
        {
            var token = tokenHeader.ToString().Replace("Bearer ", string.Empty);

            LoginDTO loginDTO = new LoginDTO()
            {
                jwt = token
            };

            LoginDTO loginDTOResult = await validateToken(loginDTO);

            try
            {
                if (loginDTOResult == null || !roles.Contains(loginDTOResult.role)) throw new UnauthorizedAccessException("User does not have necessary roles!");
            }
            catch (Exception e)
            {
                throw new UnauthorizedAccessException($"User does not have necessary roles! {e.Message}");
                //return false;
            }
            return true;
        }

        public async Task<bool> checkHeaderEmail(string email, String tokenHeader)
        {
            var token = tokenHeader.ToString().Replace("Bearer ", string.Empty);

            LoginDTO loginDTO = new LoginDTO()
            {
                jwt = token
            };

            LoginDTO loginDTOResult = await validateToken(loginDTO);

            try
            {
                if (loginDTOResult == null || !email.Equals(loginDTOResult.username)) throw new UnauthorizedAccessException("User does not correspond to request!");
            }
            catch (Exception e)
            {
                throw new UnauthorizedAccessException($"User does not have necessary roles! {e.Message}");
                //return false;
            }
            return true;
        }
    }
}
