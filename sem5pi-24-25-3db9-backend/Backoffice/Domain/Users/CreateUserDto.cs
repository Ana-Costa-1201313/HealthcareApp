using System.Text.Json.Serialization;
using Backoffice.Domain.Shared;

namespace Backoffice.Domain.Users
{
    public class CreateUserDto
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Role Role { get; set; }

        public string Email { get; set; }

        public string ConfirmationEmail { get; set; }

    }
}