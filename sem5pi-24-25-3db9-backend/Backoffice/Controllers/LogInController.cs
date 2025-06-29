using Microsoft.AspNetCore.Mvc;
using System.Text;
using Backoffice.Domain.Users;
using Microsoft.IdentityModel.Tokens;
using Backoffice.Domain.Shared;

namespace Backoffice.Controllers
{

    [Route("Backoffice/[Controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private readonly LogInServices _service;

        public LoginController(LogInServices service) 
        {
            _service = service;
        }

        [HttpPost("userExists")]
        public async Task<IActionResult> UserExists()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var tokenHeader))
            {
                return BadRequest("Authorization header is missing");
            }

            // Remove the 'Basic ' prefix from the token
            var token = tokenHeader.ToString().Replace("Basic ", string.Empty);
            byte[] credentialsBytes = Convert.FromBase64String(token);
            string credentials = Encoding.UTF8.GetString(credentialsBytes);

            (string email, string password) = SeparateCredentials(credentials);            
            // Validate parameters  present
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return BadRequest("email and password must be provided.");
            }

            // query database
            bool userExists;
            try
            {
                userExists = await CheckUserCredentials(email, password);
            }
            catch (Exception ex)
            {
                return Unauthorized($"Invalid email or password. {email} {password}\n {ex.Message}");
            }
                LoginDTO loginDTO = await getLoginDTO(email);

            if (userExists)
            {
                return Ok(loginDTO);
            }
            else
            {
                return Unauthorized($"Invalid email or password. {email} {password}");
            }
        }

        private async Task<bool> CheckUserCredentials(string username, string password) 
        { 
            return await _service.CheckUserCredentials1(username, password);
        }

        private async Task<LoginDTO> getLoginDTO(string email)
        {
            return await _service.getLoginDTO(email);
        }

        private (string email, string password) SeparateCredentials( string credentials)
        {
            if (string.IsNullOrEmpty(credentials))
            {
                throw new ArgumentException("Credentials cannot be null or empty.", nameof(credentials));
            }

            // Split the string by ':'
            var parts = credentials.Split(':');

            // Ensure we have exactly two parts
            if (parts.Length != 2)
            {
                throw new FormatException("Credentials format must be 'email:password'.");
            }

            // Trim whitespace and return as a tuple
            return (parts[0].Trim(), parts[1].Trim());
        }
    }
}