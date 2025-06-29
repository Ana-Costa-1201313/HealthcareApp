using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Backoffice.Domain.Shared;

namespace Backoffice.Infraestructure.Shared
{
    public class RoleConverter : ValueConverter<Role, string>
    {
        public RoleConverter()
            : base(
                role => role.ToString(),
                roleString => (Role) Enum.Parse(typeof(Role), roleString))
        {

        }
    }
}
