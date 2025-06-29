using Backoffice.Domain.Patients;
using Backoffice.Domain.Users;

namespace Backoffice.Domain.Shared
{
    public class CreatePatientRequestDto
    {
        public CreateUserDto UserDto { get; set; }
        public CreatePatientDto PatientDto { get; set; }
    }
}
