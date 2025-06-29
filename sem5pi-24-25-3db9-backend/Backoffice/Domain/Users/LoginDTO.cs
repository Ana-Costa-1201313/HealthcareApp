#nullable enable

namespace Backoffice.Domain.Users {

    public class LoginDTO {
        public string? username{ get; set; }

        public string? password { get; set; }

        public string? googleCredentials { get; set; }

        public string? jwt {get;set; }
        
        public string? role { get; set; }

        public DateTime? tokenCreated { get; set; }

        public int? a {get;set;}

    }   
}