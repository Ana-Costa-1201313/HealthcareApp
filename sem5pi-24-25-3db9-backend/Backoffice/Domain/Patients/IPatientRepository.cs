using Backoffice.Domain.Shared;

namespace Backoffice.Domain.Patients
{
    public interface IPatientRepository : IRepository<Patient, PatientId>
    {
        Task<Patient> GetPatientByEmailAsync(Email email);
       
        Task<List<Patient>> SearchPatientsAsync(string name, string email,DateTime? dateOfBirth
        , string medicalRecordNumber);

        Task<Patient> GetLatestPatientByMonthAsync();
        Task<Patient> GetWithDetailsAsync(PatientId id);
        Task<List<Patient>> GetAllWithDetailsAsync();
    }
}