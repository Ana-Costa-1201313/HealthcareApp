using Backoffice.Domain.Patients;

namespace Backoffice.Domain.Shared
{
    public interface IPatientService
    {

        Task<List<PatientDto>> GetAllAsync();
        Task<SearchPatientDto> GetByEmailAsync(Email email);
        Task<PatientDto> GetByIdAsync(Guid id);
        Task<PatientDto> AddAsync(CreatePatientDto dto, string medicalRecordNumber);
        Task<PatientDto> UpdateAsync(Guid id, EditPatientDto dto);
        Task<PatientDto> PatchAsync(Guid id, EditPatientDto dto);
        Task<PatientDto> DeleteAsync(Guid id);
        Task<List<SearchPatientDto>> SearchPatientsAsync(string name, string email, DateTime? dateOfBirth, string medicalRecordNumber);
        void DeletePacientProfileAsync(string email);
        Task<string> GenerateNextMedicalRecordNumber();
    }
}

