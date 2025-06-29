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
    public class ICD11Service
    {

        private readonly Random Random = new();


        public ICD11Service()
        {

        }


        public string GenerateICD11Code()
        {
            char chapter = (char)Random.Next('A', 'Z' + 1);
            int subCode = Random.Next(0, 1000);

            string extension = Random.Next(0, 2) == 1
                ? $".{Random.Next(0, 10)}{(char)Random.Next('A', 'Z' + 1)}"
                : string.Empty;

            return $"{chapter}{subCode:D3}{extension}";
        }


    }
}